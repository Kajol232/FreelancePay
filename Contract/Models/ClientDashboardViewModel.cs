using FreelancePay.Entities;

namespace FreelancePay.Contract.Models
{
    public class ClientDashboardViewModel
    {
        public List<PaymentViewModel> Invoices { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalSpentCount {  get; set; }
        public int PendingPaymentsCount { get; set; } 
        public decimal PendingPayments { get; set; }
        public decimal TotalOverduePayments {  get; set; }
        public int TotalOverdue{  get; set; }
    }
}
