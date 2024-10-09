
using FreelancePay.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelancePay.Contract.Service
{
    public class TransferBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TransferBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await InitiateTransferAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task InitiateTransferAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var paystackService = scope.ServiceProvider.GetRequiredService<IPaystackService>();

                

                var verifiedInvoices = await dbContext.Invoices
                    .Where(i => i.Status == InvoiceStatus.Verified).ToListAsync();
                foreach(var invoice in verifiedInvoices)
                {
                    var freelancer = await dbContext.AppUsers.FindAsync(invoice.FreelancerId);
                    var client = await dbContext.AppUsers.FindAsync(invoice.ClientId);
                    var transfer = await paystackService.TransferFunds(invoice.Amount, freelancer.RecipientCode,
                        $"Invoice Payment of {invoice.Description} by {client.Username}");

                    if (transfer == null)
                    {
                       // log
                    }
                    transfer.Id = Guid.NewGuid();
                    transfer.InvoiceId = invoice.InvoiceId;
                    transfer.TransferDate = DateTime.Now;
                    transfer.Status = TransferStatus.Queued;

                    invoice.Status = InvoiceStatus.PaymentInitiated;
                    dbContext.Invoices.Update(invoice);

                    await dbContext.AddAsync(transfer);
                    await dbContext.SaveChangesAsync();

                }
            }

        }

        private async Task VerifyTransferAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var paystackService = scope.ServiceProvider.GetRequiredService<IPaystackService>();

                var queuedTransfers = await dbContext.Transfers
                    .Where(t => t.Status == TransferStatus.Queued).ToListAsync();

                

                foreach (var transfer in queuedTransfers)
                {
                    var invoice = await dbContext.Invoices.FindAsync(transfer.InvoiceId);
                    var payment = await dbContext.Payments.FirstOrDefaultAsync(p => p.InvoiceId == transfer.InvoiceId);

                    var status = await paystackService.VerifyTransfer(transfer.Reference);
                    if (status)
                    {
                        transfer.Status = TransferStatus.TransferSuccessful;
                        invoice.Status = InvoiceStatus.Paid;
                        payment.Status = PaymentStatus.Transferred;

                    }
                }

            }
        }
    }
}
