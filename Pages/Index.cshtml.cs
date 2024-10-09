using FreelancePay.Contract.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreelancePay.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserRepository _userRepository;

        public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor contextAccessor, IUserRepository userRepository)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = _contextAccessor.HttpContext.User.Identity.Name;

                var user = await _userRepository.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    throw new Exception("User cannot be null");
                }

                if (user.Role == Entities.Role.Freelancer)
                {
                    return RedirectToPage("/Freelancers/Dashboard");

                }else if(user.Role == Entities.Role.Client)
                {
                    return RedirectToPage("Clients/Dashboard");
                }

            }
            return Page();

        }
    }
}
