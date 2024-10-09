using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FreelancePay.Contract.Models
{
    public class RegisterUserViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string AccountNumber { get; set; }

        [Required]
        public string BankCode { get; set; }
    }

    public class Bank
    {
        public string BankCode { get; set; }
        public string BankName
        {
            get; set;

        }

    }
}
