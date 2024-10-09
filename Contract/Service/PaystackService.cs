using FreelancePay.Contract.Service;
using FreelancePay.Contract.Models;
using Newtonsoft.Json;
using RestSharp;
using FreelancePay.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography.Xml;

namespace FreelancePay.Contract.Service
{
    public class PaystackService : IPaystackService
    {
        private readonly RestClient _client;
        private readonly string _secretKey;
        private readonly string CallBackUrl;

        public PaystackService(IConfiguration configuration)
        {
            _secretKey = configuration["Paystack:SecretKey"];
            
            _client = new RestClient("https://api.paystack.co");

            //CallBackUrl = configuration["Paystack:CallBackUrl"];
        }

        public async Task<string> InitiatePaymentAsync(PaymentRequest paymentRequest)
        {
            var request = new RestRequest("/transaction/initialize", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_secretKey}");

            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                amount = (int)(paymentRequest.Amount * 100),
                email = paymentRequest.Email,
                callback_url = paymentRequest.CallbackUrl,
                reference = paymentRequest.Reference,
            };

            request.AddJsonBody(body);

            var response = await _client.ExecuteAsync(request);
            dynamic result = JsonConvert.DeserializeObject(response.Content);

            return result.data.authorization_url;
        }

        public async Task<bool> VerifyPaymentAsync(string reference)
        {
            var request = new RestRequest($"/transaction/verify/{reference}", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_secretKey}");
            var response = await _client.ExecuteAsync(request);
            dynamic result = JsonConvert.DeserializeObject(response.Content);

            return result.data.status == "success";
        }

        public async Task<Transfer> TransferFunds(decimal amount, string recipientCode, string reasons)
        {
            var request = new RestRequest("/transfer", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_secretKey}");
            request.AddHeader("Content-Type", "application/json");

            var refe = Guid.NewGuid().ToString();

            var body = new
            {
                source = "balance",
                amount = (int)(amount * 100),
                recipient = recipientCode,
                reason = reasons,
                reference = refe
            };

            request.AddJsonBody(body);

            //var response = await _client.ExecuteAsync(request);
            //dynamic result = JsonConvert.DeserializeObject(response.Content);

            /*if(result.data.status == "success")
            {
                var data = result.data;
            }*/

            return new Transfer
            {
                Amount = amount,
                Recipient = recipientCode,
                Reference = refe,
                TransferCode = "a290_befwer",
                PaystackTransferId =" 234506"
            };
        }

        public async Task<string> ResolveAccount(string bankCode, string accountNumber)
        {
            var request = new RestRequest($"/bank/resolve?account_number={accountNumber}&bank_code={bankCode}", Method.Get);

            request.AddHeader("Authorization", $"Bearer {_secretKey}");

            var response = await _client.ExecuteAsync(request);
            dynamic result = JsonConvert.DeserializeObject(response.Content);

            if(result.data.status == "success")
            {
                return result.data.account_name;

            }
            return null;
        }

        public async Task<IEnumerable<Bank>> FetchBanks()
        { 
          var request = new RestRequest("/bank?currency=NGN", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_secretKey}");

            var response = await _client.ExecuteAsync(request);

            dynamic result = JsonConvert.DeserializeObject(response.Content);


            // Convert to a list of simplified bank objects
            var simplifiedBanks = ((IEnumerable<dynamic>)result.data)
                .Select(b => new Bank
                {
                    BankName = (string)b.name,
                    BankCode = (string)b.code
                })
                .ToList();


            return simplifiedBanks;
        }

        public async Task<string> CreateRecipient(string accountName, string accountNumber, string bankCode)
        {
           var request = new RestRequest("/transferrecipient", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_secretKey}");

            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                type =  "nuban",
                name = accountName,
                account_number = accountNumber,
                bank_code = bankCode,
                currency ="NGN"
            };

            request.AddJsonBody(body);

            var response = await _client.ExecuteAsync(request);
            dynamic result = JsonConvert.DeserializeObject(response.Content);

            if(result.data.status == "success")
            {
                return null;
            }
            return result.data.recipient_code;
        }

        public async Task<bool> VerifyTransfer(string reference)
        {
            var request = new RestRequest($"/transfer/verify/{reference}", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_secretKey}");
            /*var response = await _client.ExecuteAsync(request);
            dynamic result = JsonConvert.DeserializeObject(response.Content);*/

            return true;//result.data.status == "success";
        }
    }
}
