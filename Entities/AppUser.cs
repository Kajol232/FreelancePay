using System.Security.Cryptography;
using System.Text;

namespace FreelancePay.Entities
{
    public class AppUser
    {
        public Guid Id {  get; set; }
        public string FullName { get; set; }
        public string Email {  get; set; }
        public string PasswordHash {  get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string RecipientCode { get; set; }

    }

    public enum Role
    {
        Freelancer,
        Client,
        
    }

    public static class PasswordHash
    {
        public static string GetHash(string password)
        {
            if (password == null)
            {
                throw new Exception("Password cannot be null");
            }
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
