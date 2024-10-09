namespace FreelancePay.Contract.Models
{
    public class PaymentResponse
    {
        public bool Status { get; set; }  // True if the request was successful
        public string Message { get; set; }  // Message indicating the result
        public PaymentData Data { get; set; }  // Data contains transaction details
    }

    public class PaymentData
    {
        public string AuthorizationUrl { get; set; }  // URL to redirect for payment
        public string AccessCode { get; set; }  // Unique code for the transaction
        public string Reference { get; set; }  // Unique reference for this payment
        public string Status { get; set; }  // "success", "failed", etc.
        public decimal Amount { get; set; }  // Amount paid (in kobo, so divide by 100 for Naira)
    }

}
