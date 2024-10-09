namespace FreelancePay.Contract.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string Reference { get; set; }
        public string CallbackUrl { get; set; }
    }

}
