using FreelancePay.Contract.Service;
using FreelancePay.Contract.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancePay.Pages.Clients
{
    public class PaymentCallbackModel : PageModel
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaystackService _paystackService;

        public PaymentCallbackModel(IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository, IPaystackService paystackService)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _paystackService = paystackService;
        }

        public async Task<IActionResult> OnGetAsync(string trxref, string reference)
        {
            // Verify payment status with Paystack
            var paymentStatus = await _paystackService.VerifyPaymentAsync(reference);
            var invoiceId = new Guid(reference);
            var invoice = await _invoiceRepository.Get(invoiceId);


            if (paymentStatus)
            {
                // Mark invoice as paid and hold funds in escrow
                await _invoiceRepository.UpdateInvoiceStatus(new Guid(reference), Entities.InvoiceStatus.PendingFreelancerVerification);

                await _paymentRepository.UpdatePaymentStatus(new Guid(reference), Entities.PaymentStatus.Paid);

                // Optionally notify the freelancer and client
            }
            else
            {
                // Handle payment failure
                return RedirectToPage("Error", new { message = "Payment Failed" });
            }

            return RedirectToPage("Dashboard");
        }
    }
}
