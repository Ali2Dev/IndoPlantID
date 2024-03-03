using Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class TestController : Controller
    {
        private IDocumentService _documentService;

        public TestController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public IActionResult Index()
        {
            var result = _documentService.GetAll();
            return View(result);
        }
    }
}
