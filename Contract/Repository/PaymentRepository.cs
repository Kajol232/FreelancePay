using FreelancePay.Entities;


namespace FreelancePay.Contract.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async  Task<Payment> CreatePayment(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    

        public async Task<Payment> GetPaymentById(Guid id)
        {
            return await _context.Payments.FindAsync(id);
        }

        public async Task<bool> UpdatePaymentStatus(Guid invoiceId, PaymentStatus status)
        {
            var payments = _context.Payments.SingleOrDefault(p => p.InvoiceId == invoiceId);
            if (payments == null) 
            {
                throw new Exception("Payment not found");
            
            }
            payments.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
