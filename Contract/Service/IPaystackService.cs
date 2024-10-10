using FreelancePay.Contract.Models;
using FreelancePay.Entities;
namespace FreelancePay.Contract.Service
{
    public interface IPaystackService
    {
       public  Task<string> InitiatePaymentAsync(PaymentRequest paymentRequest);
       public  Task<bool> VerifyPaymentAsync(string reference);
       public Task<Transfer> TransferFunds(decimal amount, string recipientCode, string reason);
       public Task<IEnumerable<Bank>> FetchBanks();
       public Task<string> ResolveAccount(string bankCode, string accountNumber);
       public Task<string> CreateRecipient(string accountName, string accountNumber, string bankCode);
       public Task<bool> VerifyTransfer(string reference);
        //public Task<bool> Refund(string reference);

    }
}
