using FreelancePay.Contract.Repository;
using FreelancePay.Contract.Service;
using FreelancePay.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FreelancePay.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public LoginModel(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
           
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetUserByEmailAsync(Input.Email);
                if (user != null)
                {
                    var result = await _authService.LoginUserAsync(user.Username, Input.Password);

                    if (result)
                    {
                        if (user.Role.Equals(Role.Freelancer))
                        {
                            return LocalRedirect("/Freelancers/Dashboard");
                        }
                        else if (user.Role.Equals(Role.Client))
                        {
                            return LocalRedirect("/Clients/Dashboard");
                        }
                        return LocalRedirect(returnUrl ?? Url.Content("~/"));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }

                }
               
            }

            return Page();
        }
    }
}
