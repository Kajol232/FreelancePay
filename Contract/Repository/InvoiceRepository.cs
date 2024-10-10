using FreelancePay.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelancePay.Contract.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<Invoice> Create(Invoice invoice)
        {
            if(invoice == null) throw new ArgumentNullException(nameof(invoice));

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<bool> Cancel(Guid id)
        {
            var invoice = await Get(id);
            if(invoice == null)
            {
                return false;
            }
            invoice.Status = InvoiceStatus.Deleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice> Get(Guid id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoice()
        {
            return await _context.Invoices.ToListAsync();
                
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoiceByClient(Guid clientId)
        {

            if (clientId == null)
            {
                throw new ArgumentNullException("Client Id cannot be null");
            }
            return await _context.Invoices
               .Include(i => i.Freelancer)
               .Where(i => i.ClientId == clientId)
               .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoiceByFreelancer(Guid freelancerId)
        {

            if (freelancerId == null)
            {
                throw new ArgumentNullException("Freelancer Id cannot be null");
            }
            return await _context.Invoices
               .Include(i => i.Client)
               .Where(i => i.FreelancerId == freelancerId)
               .ToListAsync();
        }
        public async Task<Invoice> Update(Invoice invoice, Guid id)
        {
           var invoicedSave = await Get(id);

            if (invoicedSave == null)
            {
                throw new Exception($"Invoice with id {id} not found");
            }

           _context.Update(invoice);
           await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateInvoiceStatus(Guid id, InvoiceStatus status)
        {
            var invoice = await Get(id);
            if(invoice == null) { throw new Exception("Invoice not found"); }

            invoice.Status = status;
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
