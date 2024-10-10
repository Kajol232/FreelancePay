
using FreelancePay.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelancePay.Contract.Service
{
    public class InvoiceBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public InvoiceBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckInvoicesAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CheckInvoicesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow.ToLocalTime();

                var overdueInvoices = await dbContext.Invoices
                    .Where(i => i.DueDate < now && 
                    (i.Status == InvoiceStatus.Pending || i.Status == InvoiceStatus.PendingFreelancerVerification 
                    || i.Status == InvoiceStatus.PendingClientVerification) && i.isExtended == false).ToListAsync();

                foreach (var invoice in overdueInvoices)
                {
                    invoice.Status = InvoiceStatus.Overdue;

                }

                var extendedOverdueInvoices = await dbContext.Invoices
                    .Where(i => i.DueDate < now && i.Status == InvoiceStatus.Extended).ToListAsync();

                foreach (var invoice in extendedOverdueInvoices)
                {
                    invoice.Status = InvoiceStatus.ExtendedOverdue;
                }

                await dbContext.SaveChangesAsync();

            }
        }
    }
}
