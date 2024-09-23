using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;

namespace Business.Concrete
{
    public class PlantNetManager : IPlantNetService
    {
        private readonly IConfiguration _configuration;

        public PlantNetManager(IConfiguration configuration)
        {
            _configuration = configuration;

            Api_Key = _configuration["PlantNetService:Key"];
            Project = _configuration["PlantNetService:Project"];
            Url = _configuration["PlantNetService:Endpoint"];
            ApiEndpoint = $"{Url}{Project}?api-key={Api_Key}";

        }
        private readonly string Api_Key;
        private readonly string Project;
        private readonly string Url;
        private readonly string ApiEndpoint;

        //private async Task<byte[]> ConvertFileToBytesAsync(IFormFile file)
        //{
        //    using var memoryStream = new MemoryStream();
        //    await file.CopyToAsync(memoryStream);
        //    return memoryStream.ToArray();
        //}

        [ValidationAspect(typeof(PlantNetValidator))]
        public async Task<PlantNetResponse> IdentifyPlant(PlantNetImageInfo info)
        {

            byte[] fileBytes = info.FileBytes;

            var response = new PlantNetResponse();


            using var httpClient = new HttpClient();
            using var multipartFormContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(fileBytes);

            multipartFormContent.Add(imageContent, "images", "image.jpg");

            multipartFormContent.Add(new StringContent("flower"), "organs");

            try
            {
                var httpResponse = await httpClient.PostAsync(ApiEndpoint, multipartFormContent);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    response.IsPlantDetected = true;

                    response.JsonResponse = await httpResponse.Content.ReadAsStringAsync();

                    // JSON yanıtını nesnelere dönüştür
                    var queryResult = JsonConvert.DeserializeObject<PlantNetQueryResult>(response.JsonResponse);

                    // En yüksek skorlu bitkiyi al
                    var highestScoringSpecies = queryResult.Results.OrderByDescending(r => r.Score).FirstOrDefault();
                    if (highestScoringSpecies != null)
                    {
                        // Global adı al
                        response.GlobalName = highestScoringSpecies.Species.ScientificNameWithoutAuthor;
                    }

                }
                else
                {
                    response.IsPlantDetected = false;
                    response.JsonResponse = "Bu fotoğraf bir bitki değil!";
                }
            }
            catch (Exception ex)
            {
                response.JsonResponse = $"HATA!: {ex.Message}";
            }

            return response;
        }
    }

}
