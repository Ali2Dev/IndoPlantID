using Business.Abstract;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using Entities.Concrete;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Web.Identity;
using Web.Models;


namespace Web.Controllers
{
    public class TestController : BaseController
    {
        private IDocumentService _documentService;
        private IFileProvider _fileProvider;
        private readonly IRoboflowService _roboflowService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPlantNetService _plantNetService;
        private readonly IOpenStreetMapService _openStreetMapService;
        private readonly ITrefleIOService _trefleIOService;
        private readonly IChatGPTService _chatGPTService;


        public TestController(UserManager<AppUser> userManager, IDocumentService documentService, IFileProvider fileProvider, IRoboflowService roboflowService, IPlantNetService plantNetService, IOpenStreetMapService openStreetMapService, ITrefleIOService trefleIOService, IChatGPTService chatGPTService) : base(userManager)
        {
            _documentService = documentService;
            _fileProvider = fileProvider;
            _roboflowService = roboflowService;
            _userManager = userManager;
            _plantNetService = plantNetService;
            _openStreetMapService = openStreetMapService;
            _trefleIOService = trefleIOService;
            _chatGPTService = chatGPTService;
        }

        public async Task<IActionResult> Index()
        {
            await GetUserPicture();

            var documents = _documentService.GetAll();

            // RoboflowResponse tipinde response değişkenini null olarak başlat
            RoboflowResponse response = null;

            // Geçici veriden JSON formatındaki modeli al
            var jsonModel = TempData["RoboflowTempData"] as string;

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

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var plantNetResult = await _plantNetService.IdentifyPlant(fileBytes);

            if (!plantNetResult.IsPlantDetected)
            {
                TempData["PlantNetTempData"] = plantNetResult.JsonResponse;

                return RedirectToAction("Index", "Test");                
            }

            else
            {
                using (var stream = new FileStream(newPicturePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                }

                byte[] imageArray = System.IO.File.ReadAllBytes(newPicturePath);

                _documentService.PostFileAsync(file, randomFileName);

                var roboflowResult = _roboflowService.GetResponse(imageArray, randomFileName);

                if (roboflowResult!=null)  
                {
                    var jsonModel = JsonSerializer.Serialize(roboflowResult);

                    TempData["RoboflowTempData"] = jsonModel;

                    var trefleIdResult = await _trefleIOService.SearchPlantIdsAsync(plantNetResult.GlobalName);

                    var firstId = trefleIdResult.First();

                    var regions = await _trefleIOService.GetPlantRegionsAsync(firstId);

                    var plantImages = await _trefleIOService.GetPlantImagesAsync(firstId);

                    TempData["PlantImages"] = Newtonsoft.Json.JsonConvert.SerializeObject(plantImages);


                    var coordinates = await _openStreetMapService.GetCoordinatesAsync(regions);

                    var locationJson = Newtonsoft.Json.JsonConvert.SerializeObject(coordinates.Select(x => new { name = x.Name, lat = x.Lat, lon = x.Lon }));

                    TempData["LocationData"] = locationJson;

                    var chatGPTResponse = await _chatGPTService.GetResponse(plantNetResult.GlobalName);

                    TempData["GPTResponse"] = chatGPTResponse;
                }
                else 
                {
                    TempData["RoboflowFailed"] = "Üzgünüz , modelimizde bu bitki bulunmuyor, sizin için en uygun bitkiyi bulduk";  

                    TempData["PlantNetTempData"] = plantNetResult.GlobalName;

                    var trefleIdResult = await _trefleIOService.SearchPlantIdsAsync(plantNetResult.GlobalName);

                    var firstId = trefleIdResult.First();

                    var regions = await _trefleIOService.GetPlantRegionsAsync(firstId);

                    var plantImages = await _trefleIOService.GetPlantImagesAsync(firstId);

                    TempData["PlantImages"] = Newtonsoft.Json.JsonConvert.SerializeObject(plantImages);


                    var coordinates = await _openStreetMapService.GetCoordinatesAsync(regions);

                    var locationJson = Newtonsoft.Json.JsonConvert.SerializeObject(coordinates.Select(x => new { name = x.Name, lat = x.Lat, lon = x.Lon }));

                    TempData["LocationData"] = locationJson;

                    var chatGPTResponse = await _chatGPTService.GetResponse(plantNetResult.GlobalName);

                    TempData["GPTResponse"] = chatGPTResponse;
                }


                return RedirectToAction("Index", "Test");
            }


            
        }
    }
}
public class DocumentCtr_NullReferenceException : NullReferenceException { };