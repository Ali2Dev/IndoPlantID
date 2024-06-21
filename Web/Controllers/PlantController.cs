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
            //Get UserId
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            string userId = user.Id;


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

                return RedirectToAction("Index", "Plant");
            }

            else
            {
                using (var stream = new FileStream(newPicturePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                }

                byte[] imageArray = System.IO.File.ReadAllBytes(newPicturePath);



                _documentService.PostFileAsync(file, randomFileName, userId);

                var document = _documentService.GetAll().OrderByDescending(x => x.Id).FirstOrDefault();
                var documentResult = new DocumentResult
                {
                    StoragePath = document.StoragePath,
                    DocumentExtension = document.DocumentExtension,
                    CreatedDate = document.CreatedDate,
                    Content = document.Content,
                    UserId = document.UserId
                };

                var roboflowResult = _roboflowService.GetResponse(imageArray, randomFileName);

                if (roboflowResult != null)
                {
                    var jsonModel = JsonSerializer.Serialize(roboflowResult);

                    TempData["RoboflowTempData"] = jsonModel;
                    TempData["PlantNetTempDataForJS"] = plantNetResult.GlobalName;

                    var trefleIdResult = await _trefleIOService.SearchPlantIdsAsync(plantNetResult.GlobalName);

                    var firstId = trefleIdResult.First();

                    var regions = await _trefleIOService.GetPlantRegionsAsync(firstId);

                    var plantImages = await _trefleIOService.GetPlantImagesAsync(firstId);

                    TempData["PlantImages"] = Newtonsoft.Json.JsonConvert.SerializeObject(plantImages);


                    var coordinates = await _openStreetMapService.GetCoordinatesAsync(regions);

                    if (coordinates != null)
                    {
                        var regionCoordinateList = new List<RegionCoordinate>();
                        foreach (var item in coordinates)
                        {
                            var regionCoordinate = new RegionCoordinate
                            {
                                Name = item.Name,
                                Lat = item.Lat,
                                Lon = item.Lon,
                                StoragePath = document.StoragePath,
                                UserId = userId
                            };
                            regionCoordinateList.Add(regionCoordinate);
                        }
                        _regionCoordinateService.AddRange(regionCoordinateList);
                    }

                    //- Translator
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    Configuration = builder.Build();

                    string key = Configuration["TranslatorService:Key"];
                    string endpoint = Configuration["TranslatorService:Endpoint"];
                    string location = Configuration["TranslatorService:Location"];

                    string textToTranslate = plantNetResult.GlobalName;
                    string translatedText = await TranslateToTurkish(textToTranslate, key, endpoint, location);

                    string plantFullName = plantNetResult.GlobalName;

                    if (plantNetResult.GlobalName != translatedText)
                    {
                        plantFullName = plantNetResult.GlobalName + " - " + translatedText;
                    }

                    //Translator final

                    var locationJson = Newtonsoft.Json.JsonConvert.SerializeObject(coordinates.Select(x => new { name = x.Name, lat = x.Lat, lon = x.Lon }));

                    TempData["LocationData"] = locationJson;

                    //GPT - DISABLED

                    //var chatGPTResponse = await _chatGPTService.GetResponse(plantNetResult.GlobalName);
                    var chatGPTResponse = "GPT disabled!";
                    //var gptResponseMaintenanceWatering = await _chatGPTService.GetMaintenanceAndWatering(plantNetResult.GlobalName);
                    var gptResponseMaintenanceWatering = "GPT Maintenance and watering disabled!";

                    TempData["GPTResponse"] = chatGPTResponse;
                    TempData["GPTResponseMaintenanceWatering"] = gptResponseMaintenanceWatering;



                    //-
                    documentResult.PlantGlobalName = plantFullName;
                    documentResult.RoboflowJsonModel = jsonModel;

                    plantImages.Flower.ForEach(x => documentResult.FlowerUrl += x.ImageUrl.ToString());
                    plantImages.Bark.ForEach(x => documentResult.BarkUrl += x.ImageUrl.ToString());
                    plantImages.Fruit.ForEach(x => documentResult.FruitUrl += x.ImageUrl.ToString());
                    plantImages.Leaf.ForEach(x => documentResult.LeafUrl += x.ImageUrl.ToString());
                    plantImages.Habit.ForEach(x => documentResult.HabitUrl += x.ImageUrl.ToString());



                    documentResult.PlantChatGPTResponse = chatGPTResponse;
                    documentResult.PlantGPTResponseMaintenanceWatering = gptResponseMaintenanceWatering;




                    //var coordinateList = new List<PlantCoordinate>();

                    //foreach (var item in coordinates)
                    //{
                    //    var plantCoordinate = new PlantCoordinate()
                    //    {
                    //        Lat = item.Lat,
                    //        Lon = item.Lon,
                    //        Name = item.Name
                    //    };

                    //    coordinateList.Add(plantCoordinate);
                    //}

                    //documentResult.PlantCoordinates = coordinateList;


                    _documentResultService.Add(documentResult);
                }
                else
                {
                    //- Translator
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    Configuration = builder.Build();

                    string key = Configuration["TranslatorService:Key"];
                    string endpoint = Configuration["TranslatorService:Endpoint"];
                    string location = Configuration["TranslatorService:Location"];

                    string textToTranslate = plantNetResult.GlobalName;
                    string translatedText = await TranslateToTurkish(textToTranslate, key, endpoint, location);


                    string plantFullName = plantNetResult.GlobalName;

                    if (plantNetResult.GlobalName != translatedText)
                    {
                        plantFullName = plantNetResult.GlobalName + " - " + translatedText;
                    }

                    //Translator final


                    TempData["RoboflowFailed"] = "Üzgünüz , modelimizde bu bitki bulunmuyor, sizin için en uygun bitkiyi bulduk";

                    TempData["PlantNetTempData"] = plantFullName;

                    var trefleIdResult = await _trefleIOService.SearchPlantIdsAsync(plantNetResult.GlobalName);

                    var firstId = trefleIdResult.First();

                    var regions = await _trefleIOService.GetPlantRegionsAsync(firstId);

                    var plantImages = await _trefleIOService.GetPlantImagesAsync(firstId);

                    TempData["PlantImages"] = Newtonsoft.Json.JsonConvert.SerializeObject(plantImages);


                    var coordinates = await _openStreetMapService.GetCoordinatesAsync(regions);
                    if (coordinates != null)
                    {
                        var regionCoordinateList = new List<RegionCoordinate>();
                        foreach (var item in coordinates)
                        {
                            var regionCoordinate = new RegionCoordinate
                            {
                                Name = item.Name,
                                Lat = item.Lat,
                                Lon = item.Lon,
                                StoragePath = document.StoragePath,
                                UserId = userId
                            };
                            regionCoordinateList.Add(regionCoordinate);
                        }
                        _regionCoordinateService.AddRange(regionCoordinateList);
                    }


                    var locationJson = Newtonsoft.Json.JsonConvert.SerializeObject(coordinates.Select(x => new { name = x.Name, lat = x.Lat, lon = x.Lon }));

                    TempData["LocationData"] = locationJson;

                    //GPT - DISABLED
                    //var chatGPTResponse = await _chatGPTService.GetResponse(plantNetResult.GlobalName);
                    var chatGPTResponse = "GPT disabled!";
                    //var gptResponseMaintenanceWatering = await _chatGPTService.GetMaintenanceAndWatering(plantNetResult.GlobalName);
                    var gptResponseMaintenanceWatering = "GPT Maintenance and watering disabled!";

                    TempData["GPTResponse"] = chatGPTResponse;
                    TempData["GPTResponseMaintenanceWatering"] = gptResponseMaintenanceWatering;




                    documentResult.PlantGlobalName = plantFullName;


                    plantImages.Flower.ForEach(x => documentResult.FlowerUrl += x.ImageUrl.ToString());
                    plantImages.Bark.ForEach(x => documentResult.BarkUrl += x.ImageUrl.ToString());
                    plantImages.Fruit.ForEach(x => documentResult.FruitUrl += x.ImageUrl.ToString());
                    plantImages.Leaf.ForEach(x => documentResult.LeafUrl += x.ImageUrl.ToString());
                    plantImages.Habit.ForEach(x => documentResult.HabitUrl += x.ImageUrl.ToString());

                    documentResult.PlantChatGPTResponse = chatGPTResponse;
                    documentResult.PlantGPTResponseMaintenanceWatering = gptResponseMaintenanceWatering;

                    //var coordinateList = new List<PlantCoordinate>();

                    //foreach (var item in coordinates)
                    //{
                    //    var plantCoordinate = new PlantCoordinate()
                    //    {
                    //        Lat = item.Lat,
                    //        Lon = item.Lon,
                    //        Name = item.Name
                    //    };

                    //    coordinateList.Add(plantCoordinate);
                    //}

                    //documentResult.PlantCoordinates = coordinateList;


                    _documentResultService.Add(documentResult);
                }


                return RedirectToAction("Index", "Plant");
            }



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