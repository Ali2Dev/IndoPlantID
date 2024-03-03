using Business.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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


        [HttpPost]
        public ActionResult UploadImage([Required] IFormFile file)
        {
            _documentService.PostFileAsync(file);
            if (file == null) { throw new DocumentCtr_NullReferenceException(); }

            return RedirectToAction("Index", "Test");
        }
    }
}
public class DocumentCtr_NullReferenceException : NullReferenceException { };