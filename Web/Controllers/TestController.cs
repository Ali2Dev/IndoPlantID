using Business.Abstract;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using Entities.Concrete;
using System.Text.Json;
using Web.Models;

namespace Web.Controllers
{
    public class TestController : Controller
    {
        private IDocumentService _documentService;
        private IFileProvider _fileProvider;
        private readonly IRoboflowService _roboflowService;

        public TestController(IDocumentService documentService, IFileProvider fileProvider, IRoboflowService roboflowService)
        {
            _documentService = documentService;
            _fileProvider = fileProvider;
            _roboflowService = roboflowService;
        }

        public IActionResult Index()
        {
            var documents = _documentService.GetAll();

            // RoboflowResponse tipinde response değişkenini null olarak başlat
            RoboflowResponse response = null;

            // Geçici veriden JSON formatındaki modeli al
            var jsonModel = TempData["ResultModel"] as string;

            if (!string.IsNullOrEmpty(jsonModel))
            {
                // JSON formatındaki modeli RoboflowResponseModel türüne dönüştür
                response = JsonSerializer.Deserialize<RoboflowResponse>(jsonModel);
            }

            // ViewModel'i yarat ve response ile doldur
            var viewModel = new DocumentViewModel
            {
                RoboflowResponse = response,
                Documents = documents
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UploadImage([Required] IFormFile file)
        {
            var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot//uploads");

            var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            //jpg,png vs
            var newPicturePath =
                Path.Combine(wwwrootFolder.First(x => x.Name == "images").PhysicalPath!, randomFileName);

            using (var stream = new FileStream(newPicturePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);

            }

            _documentService.PostFileAsync(file, randomFileName);

            byte[] imageArray = System.IO.File.ReadAllBytes(newPicturePath);

            var result = _roboflowService.GetResponse(imageArray, randomFileName);

            var jsonModel = JsonSerializer.Serialize(result);

            TempData["ResultModel"] = jsonModel;

            return RedirectToAction("Index", "Test");
        }
    }
}
public class DocumentCtr_NullReferenceException : NullReferenceException { };