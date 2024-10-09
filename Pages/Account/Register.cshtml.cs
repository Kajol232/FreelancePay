using FreelancePay.Contract.Repository;
using FreelancePay.Contract.Models;
using FreelancePay.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreelancePay.Contract.Service;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreelancePay.Pages.Account
{
   public class RegisterModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IPaystackService _paystackService;

        // This property will hold the list of banks
        public SelectList Banks { get; set; }
        public SelectList Roles { get; set; }

        public RegisterModel(IUserRepository userRepository, IPaystackService paystackService)
        {
            _userRepository = userRepository;
            _paystackService = paystackService;
        }        

        [BindProperty]
        public RegisterUserViewModel RegisterUserViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var banks = await _paystackService.FetchBanks();

            Banks = new SelectList(banks, "BankCode", "BankName");
          
            var roles = Enum.GetValues(typeof(Role))
                                                    .Cast<Role>()
                                                    .Select(e => new SelectListItem
                                                    {
                                                        Text = e.ToString(),
                                                        Value = ((int)e).ToString() 
                                                    }).ToList();
            Roles = new SelectList(roles, "Value", "Text");

            ViewData["Banks"] = Banks;
            ViewData["Roles"] = Roles;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var resolve = await _paystackService.ResolveAccount(RegisterUserViewModel.BankCode, RegisterUserViewModel.AccountNumber);

            if (resolve == null) 
            {
                return BadRequest("Name on account does not match inputted name");
            }

            var recipientCode = await _paystackService.CreateRecipient(RegisterUserViewModel.FullName, RegisterUserViewModel.AccountNumber,
                RegisterUserViewModel.BankCode);

            if (recipientCode == null) 
            {
                return BadRequest("Unable to register user, pls try again");
            }

            var hash = PasswordHash.GetHash(RegisterUserViewModel.Password);

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                FullName = RegisterUserViewModel.FullName,
                Role = (Role)RegisterUserViewModel.RoleId,
                Email = RegisterUserViewModel.Email,
                Username = RegisterUserViewModel.UserName,
                PasswordHash = hash,
                AccountNumber = RegisterUserViewModel.AccountNumber,
                BankCode = RegisterUserViewModel.BankCode,
                RecipientCode = recipientCode,

            };


            var result = await _userRepository.AddUserAsync(user);

            if (result != null)
            {
                // Redirect to login page after successful registration
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }
    }

}
