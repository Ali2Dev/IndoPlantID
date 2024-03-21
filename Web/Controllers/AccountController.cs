using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Identity;

namespace Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager) : base(userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> AccessDenied(string returnUrl)
        {
            await GetUserPicture();
            return View();
        }
    }
}
