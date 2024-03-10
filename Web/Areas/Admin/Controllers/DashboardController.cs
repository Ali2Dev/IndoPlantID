using Business.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Identity.ViewModels;
using Web.Identity;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        IDocumentService _documentService;
        private readonly UserManager<AppUser> _userManager;


        public DashboardController(IDocumentService documentService, UserManager<AppUser> userManager)
        {
            _documentService = documentService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {

            return View();
        }


        public IActionResult GetAllDocuments()
        {
            var documents = _documentService.GetAll();
            return View(documents);
        }

        public async Task<IActionResult> GetAllUsers()
        {
            var userList = await _userManager.Users.ToListAsync();

            var userViewModelList = userList.Select(x => new UserViewModel
            {
                Id = x.Id,
                Username = x.UserName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber

            }).ToList();

            return View(userViewModelList);
        }
    }
}
