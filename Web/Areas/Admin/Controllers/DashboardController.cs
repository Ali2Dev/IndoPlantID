using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        IDocumentService _documentService;

        public DashboardController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public IActionResult Index()
        {
            var documents = _documentService.GetAll();
            return View(documents);
        }
    }
}
