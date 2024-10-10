namespace FreelancePay.Entities
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }        
        public decimal Amount { get; set; }           
        public string Description { get; set; }      
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate {  get; set; }
        public DateTime? PaidDate { get; set; }
        public Guid ClientId { get; set; }
        public AppUser Client { get; set; }
        public Guid FreelancerId { get; set; }   
        public AppUser Freelancer {  get; set; }
        public bool isExtended { get; set; }       
    }

    public enum InvoiceStatus
    {
        Pending,//initial
        Paid,//transferrred to client
        Cancelled,// cancel
        Overdue,// can be extended
        PendingFreelancerVerification,// only paid invoice payments can be veried
        PendingClientVerification,
        Verified, // --raise disputes --later
        Deleted, 
        Extended,// pay or cancel
        ExtendedOverdue,// refund to client if paid
        PendingExtensionAcceptance,// accept extension proposed by clients
        PaymentInitiated

    }
}
