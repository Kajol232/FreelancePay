using FreelancePay.Contract.Models;
using FreelancePay.Contract.Repository;
using FreelancePay.Contract.Service;
using FreelancePay.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace FreelancePay.Pages.Clients
{
    public class DashboardModel : PageModel
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaystackService _paymentService;

        public decimal TotalSpent { get; set; }
        public int TotalSpentCount { get; set; }

        public int PendingPaymentsCount { get; set; }
        public decimal PendingPayments { get; set; }

        public decimal TotalOverduePayments { get; set; }
        public int TotalOverdue { get; set;}

        public decimal ExtendedPayments {  get; set; }
        public int Extended {  get; set; }

        public string Name { get; set; }
        public IEnumerable<Invoice> Invoices { get; set; }

        public DashboardModel(IHttpContextAccessor contextAccessor, IUserRepository userRepository, IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository,IPaystackService paymentService)
        {
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }
    
        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            var client = await GetClient();

            if (client.Role != Role.Client)
            {
                return Unauthorized();
            }
           

            var invoices = await _invoiceRepository.GetAllInvoiceByClient(client.Id);

            // Load client dashboard data with invoices and statistics
           
            TotalSpent = invoices.Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.PendingFreelancerVerification
            || i.Status == InvoiceStatus.PendingClientVerification || i.Status == InvoiceStatus.Verified 
            || i.Status == InvoiceStatus.PaymentInitiated).Sum(i => i.Amount);

            TotalSpentCount = invoices.Where(i => i.Status == InvoiceStatus.Paid || i.Status == InvoiceStatus.PendingFreelancerVerification
            || i.Status == InvoiceStatus.PendingClientVerification || i.Status == InvoiceStatus.Verified
            || i.Status == InvoiceStatus.PaymentInitiated).Count();

            PendingPaymentsCount = invoices.Count(i => i.Status == InvoiceStatus.Pending);
            PendingPayments = invoices.Where(i => i.Status == InvoiceStatus.Pending).Sum(i => i.Amount);

            TotalOverduePayments = invoices.Where(i => i.Status == InvoiceStatus.Overdue).Sum(i => i.Amount);
            TotalOverdue = invoices.Where(i => i.Status == InvoiceStatus.Overdue).Count();

            ExtendedPayments = invoices.Where(i => i.Status == InvoiceStatus.Extended).Sum(i => i.Amount);
            Extended = invoices.Count(i => i.Status == InvoiceStatus.Extended);

            Invoices = invoices.Where(i => i.Status != InvoiceStatus.Cancelled || i.Status == InvoiceStatus.Deleted); 
            Name = client.Username;
            return Page();
        }

        public async Task<IActionResult> OnGetPayAsync(Guid id)
        {
            var client = await GetClient();

            var invoice = await _invoiceRepository.Get(id);

            if (invoice == null)
            {
                return NotFound("Invoice Not found");
            }

            if(invoice.Status != InvoiceStatus.Pending && invoice.Status != InvoiceStatus.Extended)
            {
                var message = "Only extended or pending invoice can be paid";
                return BadRequest(message);
            }

            // Prepare payment request for Paystack
            var paymentRequest = new PaymentRequest
            {
                Amount = invoice.Amount, // Paystack processes payments in kobo
                Email = invoice.Client.Email,
                Reference = invoice.InvoiceId.ToString(),
                CallbackUrl = "https://localhost:7172/Clients/PaymentCallback",
                
            };

            // Initiate payment via Paystack
            var paymentUrl = await _paymentService.InitiatePaymentAsync(paymentRequest);

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                Amount = invoice.Amount,
                InvoiceId = invoice.InvoiceId,
                Status = Entities.PaymentStatus.New,
                Reference = paymentRequest.Reference,
                PaymentDate = DateTime.Now,

            };
            await _paymentRepository.CreatePayment(payment);


            // Redirect client to Paystack payment page
            return Redirect(paymentUrl);
        }

        public async Task<IActionResult> OnGetCancelAsync(Guid id)
        {
            var client = await GetClient();
            var invoice = await _invoiceRepository.Get(id);
            if (invoice == null)
            {
                return NotFound("invoice not found");
            }

            if (invoice.Status != InvoiceStatus.Pending && invoice.Status != InvoiceStatus.Extended)
            {
                return BadRequest("You can only cancel a pending or extended invoice");
            }
            var success = await _invoiceRepository.Cancel(id);

            if (!success)
            {
                ModelState.AddModelError("", "Unable to cancel invoice.");
            }

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostSetNewDueDateAsync(Guid invoiceId, DateTime newDueDate)
        {
            var invoice = await _invoiceRepository.Get(invoiceId);

            if (invoice == null)
            {
                return NotFound("Invoice not found");
            }
            if(invoice.Status != InvoiceStatus.Overdue)
            {
                return BadRequest("You can only extend an overdue invoice");
            }

            invoice.DueDate = newDueDate;
            invoice.Status = InvoiceStatus.PendingExtensionAcceptance;
            await _invoiceRepository.Update(invoice, invoiceId);

            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetVerifyPaymentAsync(Guid id)
        {
            var invoice = await _invoiceRepository.Get(id);

            if (invoice == null)
            {
                return NotFound("Invoice not found");
            }
            if (invoice.Status != InvoiceStatus.PendingClientVerification)
            {
                return BadRequest("You can not verify an invoice that is not pending verification");
            }
            invoice.Status = InvoiceStatus.Verified;
            await _invoiceRepository.Update(invoice,id);
            return RedirectToPage();


        }

        private async Task<AppUser> GetClient()
        {
            var username = _contextAccessor.HttpContext.User.Identity.Name;

            return await _userRepository.GetUserByUsernameAsync(username);

        }
    }
}
