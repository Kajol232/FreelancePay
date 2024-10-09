using System.ComponentModel.DataAnnotations;

namespace FreelancePay.Contract.Models
{
    public class CreateInvoiceModel
    {
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
