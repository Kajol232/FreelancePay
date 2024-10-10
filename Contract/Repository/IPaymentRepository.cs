using FreelancePay.Entities;

namespace FreelancePay.Contract.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment> GetPaymentById(Guid id);
        Task<bool> UpdatePaymentStatus(Guid id, PaymentStatus status);
        Task<Payment> CreatePayment(Payment payment);
        Task<Payment> GetPaymentByInvoiceId(Guid invoiceId);
    }
}
