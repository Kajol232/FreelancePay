using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FreelancePay.Contract.Repository;
using FreelancePay.Entities;
using FreelancePay.Contract.Models;


namespace FreelancePay.Pages.Freelancers
{
    public class DashboardModel : PageModel
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public DashboardModel(IInvoiceRepository invoiceRepository, IUserRepository userRepository, IHttpContextAccessor contextAccessor)
        {
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
        }

        public decimal TotalInvoices { get; set; }
        public int TotalInvoicesCount { get; set; }

        public decimal PaidInvoices { get; set; }
        public decimal PaidInvoicesCount { get; set; } 

        public decimal PendingInvoices { get; set; }
        public int PendingInvoicesCount { get; set; }

        public decimal OverdueInvoices { get; set; }
        public int OverdueInvoicesCount { get; set; }

        public IEnumerable<Invoice> Invoices { get; set; }

        [BindProperty]
        public CreateInvoiceModel CreateInvoiceModel { get; set; }

        public SelectList Clients { get; set; }
        public string Username { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            var freelancer = await GetFreelancer(); 

            if(freelancer.Role != Role.Freelancer)
            {
                return Unauthorized();
            }

            // Fetch statistics
            var invoices = await _invoiceRepository.GetAllInvoiceByFreelancer(freelancer.Id);

            TotalInvoices = invoices.Sum(i => i.Amount);
            TotalInvoicesCount = invoices.Count();

            PaidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Amount);
            PaidInvoicesCount = invoices.Count(i => i.Status == InvoiceStatus.Paid);

            PendingInvoices = invoices.Where(i => i.Status == InvoiceStatus.Pending).Sum(i => i.Amount);
            PendingInvoicesCount = invoices.Count(i => i.Status == InvoiceStatus.Pending);

            OverdueInvoices = invoices.Where(i => i.Status == InvoiceStatus.Overdue).Sum(i => i.Amount);
            OverdueInvoicesCount = invoices.Count(i => i.Status == InvoiceStatus.Overdue);

            // Set data for listing and creation of invoices
            Invoices = invoices.Where(i => i.Status != InvoiceStatus.Cancelled && i.Status != InvoiceStatus.Deleted);
            var clients = await _userRepository.GetClientsAsync();

            Clients = new SelectList(clients, "ClientId", "ClientName");

           
            Username = freelancer.Username;

            ViewData["Clients"] = Clients;

            return Page();
        }

        public async Task<IActionResult> OnPostCreateInvoiceAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var freelancer = await GetFreelancer();
            var invoice = new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                Amount = CreateInvoiceModel.Amount,
                FreelancerId = freelancer.Id,
                ClientId = CreateInvoiceModel.ClientId,
                DueDate = CreateInvoiceModel.DueDate,
                Status = InvoiceStatus.Pending,
                CreatedAt = DateTime.Now,
                Description = CreateInvoiceModel.Description,
            };
            await _invoiceRepository.Create(invoice);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCancelInvoiceAsync(Guid id)
        {
            var freelancerId = await GetFreelancer();
            var invoice = await _invoiceRepository.Get(id);
            if(invoice == null)
            {
                return NotFound("invoice not found");
            }

            if(invoice.Status != InvoiceStatus.Pending && invoice.Status != InvoiceStatus.Extended)
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

        public async Task<IActionResult> OnGetVerifyPaymentAsync(Guid id)
        {
            var invoice = await _invoiceRepository.Get(id);

            if (invoice == null)
            {
                return NotFound("Invoice not found");
            }
            if (invoice.Status != InvoiceStatus.PendingFreelancerVerification)
            {
                return BadRequest("You can not verify an invoice that is not pending verification");
            }
            invoice.Status = InvoiceStatus.PendingClientVerification;
            await _invoiceRepository.Update(invoice, id);
            return RedirectToPage();


        }

        public async Task<IActionResult> OnGetAcceptExtensionAsync(Guid id)
        {
            var invoice = await _invoiceRepository.Get(id);

            if (invoice == null)
            {
                return NotFound("Invoice not found");
            }
            if (invoice.Status != InvoiceStatus.PendingExtensionAcceptance)
            {
                return BadRequest("You can only accept a proposed new date for an overdue invoice");
            }
            invoice.Status = InvoiceStatus.Extended;
            await _invoiceRepository.Update(invoice, id);
            return RedirectToPage();


        }
        private async Task<AppUser> GetFreelancer()
        {
            var username = _contextAccessor.HttpContext.User.Identity.Name;

            return await _userRepository.GetUserByUsernameAsync(username);

           

        }
    }
}
