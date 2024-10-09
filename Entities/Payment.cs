namespace FreelancePay.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }        
        public decimal Amount { get; set; }          
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public DateTime PaymentDate { get; set; } 
        public PaymentStatus Status { get; set; }
        public string Reference { get; set; }
    }

    public enum PaymentStatus
    {
        New,
        Paid,
        Transferred,
        Refunded
    }
}
