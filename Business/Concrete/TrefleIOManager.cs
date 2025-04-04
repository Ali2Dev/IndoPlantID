﻿using Business.Abstract;
using Entities.Concrete;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Business.Concrete
{
    public class TrefleIOManager : ITrefleIOService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public TrefleIOManager(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<int>> SearchPlantIdsAsync(string name)
        {
            try
            {
                var token = _configuration["TrefleIOService:Key"];
                var url = $"https://trefle.io/api/v1/plants?token={token}&filter[scientific_name]={Uri.EscapeDataString(name)}";
                //var url = $"https://trefle.io/api/v1/plants/search?token={token}&q={name}";

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(content);

                        var plantIds = new List<int>();

                        foreach (var plant in json["data"])
                        {
                            var id = (int)plant["id"];
                            plantIds.Add(id);
                        }

                        return plantIds;
                    }
                    else
                    {
                        // Handle unsuccessful response
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetPlantRegionsAsync(int plantId)
        {
            try
            {
                var token = _configuration["TrefleIOService:Key"];
                var url = $"https://trefle.io/api/v1/species/{plantId}?token={token}";

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(content);

                        // Extract regions where the plant naturally grows
                        var regions = json["data"]["distribution"]["native"];

                        var regionsList = new List<string>();

                        if (regions == null)
                        {
                            regionsList.Add("Bölge bilgisi bulunamadı.");
                        }
                        // Check if regions is an array
                        if (regions is JArray regionArray)
                        {
                            foreach (var regionToken in regionArray)
                            {
                                var regionName = (string)regionToken;
                                regionsList.Add(regionName);
                            }
                        }
                        else if (regions is JValue regionValue) // If it's not an array but a single value
                        {
                            var regionName = (string)regionValue;
                            regionsList.Add(regionName);
                        }

                        return regionsList;
                    }
                    else
                    {
                        // Handle unsuccessful response
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<PlantImages> GetPlantImagesAsync(int plantId)
        {
            try
            {
                var token = _configuration["TrefleIOService:Key"];
                var url = $"https://trefle.io/api/v1/species/{plantId}?token={token}";

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; };

                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(content);

                        // Extract images of the plant
                        var images = json["data"]["images"];

                        var plantImages = new PlantImages();

                        if (images != null)
                        {
                            plantImages.Leaf = GetImagesList(images["leaf"]);
                            plantImages.Flower = GetImagesList(images["flower"]);
                            plantImages.Fruit = GetImagesList(images["fruit"]);
                            plantImages.Bark = GetImagesList(images["bark"]);
                            plantImages.Habit = GetImagesList(images["habit"]);
                        }

                        return plantImages;
                    }
                    else
                    {
                        // Handle unsuccessful response
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error occurred: {ex.Message}");
                return null;
            }
        }

        private List<ImageDetail> GetImagesList(JToken imagesToken)
        {
            var imagesList = new List<ImageDetail>();

            if (imagesToken is JArray imagesArray)
            {
                foreach (var imageToken in imagesArray)
                {
                    var image = new ImageDetail
                    {
                        Id = (int)imageToken["id"],
                        ImageUrl = (string)imageToken["image_url"],
                        Copyright = (string)imageToken["copyright"]
                    };

                    imagesList.Add(image);
                }
            }

            return imagesList;
        }
    }
}
