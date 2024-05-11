using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Identity;

namespace Web.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;

        public ErrorController(UserManager<AppUser> userManager) : base(userManager)
        {
            _userManager = userManager;
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ShowNavbar = false;
            return View();
        }

        public IActionResult PageNotFound()
        {
            ViewBag.ShowNavbar = false;
            ViewBag.Is404Page = true;
            return View();
        }
    }
}
