namespace FreelancePay.Entities
{
    public class Transfer
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string Recipient { get; set; }
        public string Reference { get; set; }
        public string TransferCode { get; set; }
        public string PaystackTransferId { get; set; }
        public decimal Amount { get; set; }
        public TransferStatus Status { get; set; }
        public DateTime TransferDate { get; set; }
    }

    public enum TransferStatus
    {
        Queued,
        TransferSuccessful,
        TransferFailed,
    }
}
