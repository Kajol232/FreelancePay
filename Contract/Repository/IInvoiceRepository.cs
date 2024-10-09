
using FreelancePay.Entities; 
namespace FreelancePay.Contract.Repository
{
    public interface IInvoiceRepository
    {
        public Task<IEnumerable<Invoice>> GetAllInvoiceByClient(Guid clientId);
        public Task<IEnumerable<Invoice>> GetAllInvoiceByFreelancer(Guid freelancerId);
        public Task<IEnumerable<Invoice>> GetAllInvoice();
        public Task<Invoice> Create(Invoice invoice);
        public Task<Invoice> Update(Invoice invoice, Guid id);
        public Task<bool> Cancel(Guid id);
        public Task<Invoice> Get(Guid id);
        public Task UpdateInvoiceStatus(Guid id, InvoiceStatus status);
    }
}
