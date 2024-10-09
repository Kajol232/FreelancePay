namespace FreelancePay.Contract.Models
{
    public class PaymentViewModel
    {
        public Guid InvoiceId { get; set; }
        public string FreelancerName { get; set; }
        public decimal Amount { get; set; }
        public string InvoiceStatus { get; set; }
        public string ClientEmail { get; set; }
        public string ClientName { get; set; }
    }

}
