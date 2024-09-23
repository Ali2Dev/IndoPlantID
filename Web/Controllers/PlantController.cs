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
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Net.Mime;
using System.Drawing;
using System.Text.Json.Nodes;

namespace Web.Controllers
{
    [Authorize]
    public class PlantController : BaseController
    {
        private static IConfiguration Configuration { get; set; }

        private IDocumentService _documentService;
        private IDocumentResultService _documentResultService;
        private IFileProvider _fileProvider;
        private readonly IRoboflowService _roboflowService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPlantNetService _plantNetService;
        private readonly IOpenStreetMapService _openStreetMapService;
        private readonly ITrefleIOService _trefleIOService;
        private readonly IChatGPTService _chatGPTService;
        private readonly IRegionCoordinateService _regionCoordinateService;


        public PlantController(UserManager<AppUser> userManager, IDocumentService documentService, IFileProvider fileProvider, IRoboflowService roboflowService, IPlantNetService plantNetService, IOpenStreetMapService openStreetMapService, ITrefleIOService trefleIOService, IChatGPTService chatGPTService, IDocumentResultService documentResultService, IRegionCoordinateService regionCoordinateService) : base(userManager)
        {
            _documentService = documentService;
            _fileProvider = fileProvider;
            _roboflowService = roboflowService;
            _userManager = userManager;
            _plantNetService = plantNetService;
            _openStreetMapService = openStreetMapService;
            _trefleIOService = trefleIOService;
            _chatGPTService = chatGPTService;
            _documentResultService = documentResultService;
            _regionCoordinateService = regionCoordinateService;
        }

        public async Task<IActionResult> Index()
        {
            await GetUserPicture();

            RoboflowResponse response = null;

            // Geçici veriden JSON formatındaki modeli al
            var jsonModel = TempData["RoboflowTempData"] as string;

            if (!string.IsNullOrEmpty(jsonModel))
            {
                // JSON formatındaki modeli RoboflowResponseModel türüne dönüştür
                response = JsonSerializer.Deserialize<RoboflowResponse>(jsonModel);
            }

            // ViewModel'i yarat ve response ile doldur
            //var viewModel = new DocumentResponseViewModel
            //{
            //    RoboflowResponse = response,
            //};


            return View(response);
        }

        private async Task<byte[]> ConvertFileToBytesAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public class FileSaveResult
        {
            public string NewPicturePath { get; set; }
            public string RandomFileName { get; set; }
        }

        private async Task<FileSaveResult> SaveFileAsync(IFormFile file)
        {
            var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot/uploads");
            var randomFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "images").PhysicalPath!, randomFileName);

            using var stream = new FileStream(newPicturePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return new FileSaveResult
            {
                NewPicturePath = newPicturePath,
                RandomFileName = randomFileName
            };
        }

        private async Task ProcessCoordinates(List<(string Name, string Lat, string Lon)> coordinates, string storagePath, string userId)
        {
            if (coordinates == null) return;

            var regionCoordinateList = coordinates
                .Select(item => new RegionCoordinate
                {
                    Name = item.Name,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    StoragePath = storagePath,
                    UserId = userId
                })
                .ToList();

            _regionCoordinateService.AddRange(regionCoordinateList);
        }

        private async Task<string> TranslatePlantName(string textToTranslate)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string key = configuration["TranslatorService:Key"];
            string endpoint = configuration["TranslatorService:Endpoint"];
            string location = configuration["TranslatorService:Location"];

            return await TranslateToTurkish(textToTranslate, key, endpoint, location);
        }

        private void BuildTempData(string jsonModel, string plantName, string roboflowFailedMessage, PlantImages plantImages, List<(string Name, string Lat, string Lon)> coordinates,string gptResponseMessage,string gptWateringMessage)
        {
            TempData["RoboflowTempData"] = jsonModel;
            TempData["PlantNetTempDataForJS"] = plantName;
            TempData["PlantImages"] = JsonConvert.SerializeObject(plantImages);

            if (coordinates != null)
            {
                var locationJson = JsonConvert.SerializeObject(coordinates.Select(x => new { x.Name, x.Lat, x.Lon }));
                TempData["LocationData"] = locationJson;
            }

            TempData["RoboflowFailed"] = roboflowFailedMessage;
            TempData["GPTResponse"] = gptResponseMessage;
            TempData["GPTResponseMaintenanceWatering"] = gptWateringMessage;
        }

        private DocumentResult GetDocumentResult(Document document, string plantFullName, string jsonModel, PlantImages plantImages, string chatGPTResponse, string gptResponseMaintenanceWatering)
        {
            return new DocumentResult
            {
                StoragePath = document.StoragePath,
                DocumentExtension = document.DocumentExtension,
                CreatedDate = document.CreatedDate,
                Content = document.Content,
                UserId = document.UserId,
                PlantGlobalName = plantFullName,
                RoboflowJsonModel = jsonModel,
                FlowerUrl = string.Join("", plantImages.Flower.Select(x => x.ImageUrl)),
                BarkUrl = string.Join("", plantImages.Bark.Select(x => x.ImageUrl)),
                FruitUrl = string.Join("", plantImages.Fruit.Select(x => x.ImageUrl)),
                LeafUrl = string.Join("", plantImages.Leaf.Select(x => x.ImageUrl)),
                HabitUrl = string.Join("", plantImages.Habit.Select(x => x.ImageUrl)),
                PlantChatGPTResponse = chatGPTResponse,
                PlantGPTResponseMaintenanceWatering = gptResponseMaintenanceWatering
            };
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([Required] IFormFile file)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!user.EmailConfirmed)
            {
                TempData["EmailNotConfirmedMsg"] = "Bitki sorgulamak için önce e-mail adresinizi doğrulayın.";
                return RedirectToAction("Index", "Plant");
            }

            byte[] fileBytes = await ConvertFileToBytesAsync(file);
            var plantNetResult = await _plantNetService.IdentifyPlant(new PlantNetImageInfo { FileBytes = fileBytes,Title = file.FileName});

            if (!plantNetResult.IsPlantDetected)
            {
                TempData["PlantNetTempData"] = plantNetResult.JsonResponse;
                return RedirectToAction("Index", "Plant");
            }

            var fileSaveResult = await SaveFileAsync(file);
            byte[] imageArray = System.IO.File.ReadAllBytes(fileSaveResult.NewPicturePath);

            var doc = new Document { StoragePath = fileSaveResult.RandomFileName, Title = file.FileName, Content = file.ContentDisposition, CreatedDate = DateTime.Now, UserId = user.Id };
            doc.DocumentExtension = Path.GetExtension(doc.Title);
            _documentService.Add(doc);

            var lastDocument = _documentService.GetAll().OrderByDescending(x => x.Id).FirstOrDefault();

            var roboflowResult = _roboflowService.GetResponse(imageArray, fileSaveResult.RandomFileName);
            string jsonModel = roboflowResult != null ? JsonSerializer.Serialize(roboflowResult) : null;

            var trefleIdResult = await _trefleIOService.SearchPlantIdsAsync(plantNetResult.GlobalName);
            var firstId = trefleIdResult.First();
            var regions = await _trefleIOService.GetPlantRegionsAsync(firstId);
            var plantImages = await _trefleIOService.GetPlantImagesAsync(firstId);
            var coordinates = await _openStreetMapService.GetCoordinatesAsync(regions);

            await ProcessCoordinates(coordinates, lastDocument.StoragePath, user.Id);

            //string translatedText = await TranslatePlantName(plantNetResult.GlobalName);
            //string plantFullName = plantNetResult.GlobalName != translatedText
            //    ? $"{plantNetResult.GlobalName} - {translatedText}"
            //    : plantNetResult.GlobalName;

            var chatGPTResponse = "GPT disabled!";
            //var chatGPTResponse = await _chatGPTService.GetResponse(plantNetResult.GlobalName);
            var gptResponseMaintenanceWatering = "GPT Maintenance and watering disabled!";
            //var gptResponseMaintenanceWatering = await _chatGPTService.GetMaintenanceAndWatering(plantNetResult.GlobalName);

            BuildTempData(jsonModel, plantNetResult.GlobalName, roboflowResult == null ? "Üzgünüz, modelimizde bu bitki bulunmuyor, sizin için en uygun bitkiyi bulduk" : null, plantImages, coordinates,chatGPTResponse,gptResponseMaintenanceWatering);
            var documentResult = GetDocumentResult(lastDocument, plantNetResult.GlobalName, jsonModel, plantImages, chatGPTResponse, gptResponseMaintenanceWatering);
            _documentResultService.Add(documentResult);

            return RedirectToAction("Index", "Plant");
        }


        [HttpGet]
        public async Task<IActionResult> PreviousDocuments()
        {
            await GetUserPicture();


            // Get UserId
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {
                string userId = user.Id;
                ViewBag.UserId = userId;

                var documentList = _documentService.GetAll()
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.CreatedDate)
                    .ToList();

                return View(documentList);
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Query(string userId, string storagePath)
        {
            var documentResult = _documentResultService.GetByStoragePathAndUserId(storagePath, userId);
            var model = new DocumentResultViewModel
            {
                PlantGlobalName = documentResult.PlantGlobalName,
                CreatedDate = documentResult.CreatedDate,
                PlantChatGPTResponse = documentResult.PlantChatGPTResponse,
                PlantGPTResponseMaintenanceWatering = documentResult.PlantGPTResponseMaintenanceWatering,
                StoragePath = documentResult.StoragePath
            };

            // FlowerUrl için URL'leri ayıkla
            model.PlantFlowerImgUrl = UrlHelper.ExtractUrlsFromString(documentResult.FlowerUrl);

            model.PlantBarkImgUrl = UrlHelper.ExtractUrlsFromString(documentResult.BarkUrl);

            model.PlantLeafImgUrl = UrlHelper.ExtractUrlsFromString(documentResult.LeafUrl);

            model.PlantFruitImgUrl = UrlHelper.ExtractUrlsFromString(documentResult.FruitUrl);

            model.PlantHabitImgUrl = UrlHelper.ExtractUrlsFromString(documentResult.HabitUrl);


            var regionCoordinates = _regionCoordinateService.GetAll(x => x.UserId == userId && x.StoragePath == storagePath);

            var locationJson = Newtonsoft.Json.JsonConvert.SerializeObject(regionCoordinates.Select(x => new { name = x.Name, lat = x.Lat, lon = x.Lon }));

            TempData["LocationData"] = locationJson;

            await GetUserPicture();


            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> Query()
        {

            return View();
        }

        [HttpGet]
        public IActionResult DetectWithCamera()
        {

            return View();
        }


        static async Task<string> TranslateToTurkish(string textToTranslate, string key, string endpoint, string location)
        {
            // Detect the language of the input text
            string detectedLanguage = await DetectLanguageAsync(textToTranslate, key, endpoint, location);

            // Translate the text to Turkish
            return await TranslateTextAsync(textToTranslate, detectedLanguage, "tr", key, endpoint, location);
        }

        static async Task<string> DetectLanguageAsync(string text, string key, string endpoint, string location)
        {
            string route = "/detect?api-version=3.0";
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                // Parse the detected language
                var detectedLanguages = JsonConvert.DeserializeObject<dynamic>(result);
                return detectedLanguages[0].language;
            }
        }

        static async Task<string> TranslateTextAsync(string text, string fromLanguage, string toLanguage, string key, string endpoint, string location)
        {
            string route = $"/translate?api-version=3.0&from={fromLanguage}&to={toLanguage}";
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                // Parse the translation result
                var translationResult = JsonConvert.DeserializeObject<dynamic>(result);
                return translationResult[0].translations[0].text;
            }
        }











        public class UrlHelper
        {
            public static List<string> ExtractUrlsFromString(string input)
            {
                List<string> extractedUrls = new List<string>();

                // input değişkenini kontrol et
                if (input != null)
                {
                    int startIndex = 0;

                    // 'https://bs' ile başlayan URL'leri ayıkla
                    while (startIndex < input.Length)
                    {
                        int bsIndex = input.IndexOf("https://bs", startIndex, StringComparison.OrdinalIgnoreCase);

                        if (bsIndex != -1)
                        {
                            // 'https://bs' ile başlayan kısmı bulundu, URL'yi al
                            int endIndex = input.IndexOf("https://bs", bsIndex + 1, StringComparison.OrdinalIgnoreCase);

                            if (endIndex == -1)
                            {
                                endIndex = input.Length;
                            }

                            string url = input.Substring(bsIndex, endIndex - bsIndex);
                            extractedUrls.Add(url);

                            // Başlangıç indeksini bir sonraki aramanın ötesine geçir
                            startIndex = endIndex;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // input değişkeni null ise, extractedUrls listesini boş olarak döndür
                    Console.WriteLine("input is null.");
                }

                return extractedUrls;
            }

        }
        //public static class UrlHelper
        //{
        //    public static List<string> ExtractUrls(string input)
        //    {
        //        List<string> extractedUrls = new List<string>();

        //        // Güncellenmiş Regex deseni kullanarak URL'leri bulma
        //        //Regex urlRegex = new Regex(@"\b(?:https?://|www\.)\S+\b");
        //        Regex urlRegex = new Regex(@"https?://[^\s/$.?#]+");

        //        MatchCollection matches = urlRegex.Matches(input);

        //        foreach (Match match in matches)
        //        {
        //            extractedUrls.Add(match.Value);
        //        }

        //        return extractedUrls;
        //    }
        //}


    }
}
public class DocumentCtr_NullReferenceException : NullReferenceException { };