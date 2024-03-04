using Business.Abstract;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Controllers
{
    public class TestController : Controller
    {
        private IDocumentService _documentService;
        private IFileProvider _fileProvider;

        public TestController(IDocumentService documentService, IFileProvider fileProvider)
        {
            _documentService = documentService;
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            var documents = _documentService.GetAll();

            //var uploadedFileBase64 = TempData["UploadedFile"] as string;
            //byte[] uploadedFile = null;

            //if (!string.IsNullOrEmpty(uploadedFileBase64))
            //{
            //    uploadedFile = Convert.FromBase64String(uploadedFileBase64);
            //}

            //var viewModel = new DocumentViewModel
            //{
            //    FileContent = uploadedFile,
            //    Documents = documents
            //};

            return View(documents);
        }


        [HttpPost]
        public async Task<ActionResult> UploadImage([Required] IFormFile file)
        {
            

            var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot//uploads");

            var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            //jpg,png vs
            var newPicturePath =
                Path.Combine(wwwrootFolder.First(x => x.Name == "images").PhysicalPath!, randomFileName);

            using var stream = new FileStream(newPicturePath, FileMode.Create);

            await file.CopyToAsync(stream);

            _documentService.PostFileAsync(file, newPicturePath);

            //using (var memoryStream = new MemoryStream())
            //{
            //    file.CopyTo(memoryStream);
            //    var fileBytes = memoryStream.ToArray();
            //    TempData["UploadedFile"] = Convert.ToBase64String(fileBytes);
            //}

            //if (file == null) { throw new DocumentCtr_NullReferenceException(); }

            return RedirectToAction("Index", "Test");
        }
    }
}
public class DocumentCtr_NullReferenceException : NullReferenceException { };