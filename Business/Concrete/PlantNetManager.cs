using Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;

namespace Business.Concrete
{
    public class PlantNetManager : IPlantNetService
    {

        private readonly string API_KEY;
        private readonly string PROJECT;
        private readonly string apiEndpoint;

        public PlantNetManager()
        {
            API_KEY = "2b101b2H2uEkZSdRkot8Nncq9";
            PROJECT = "all";
            apiEndpoint = $"https://my-api.plantnet.org/v2/identify/{PROJECT}?api-key={API_KEY}";
        }

        public async Task<PlantNetResponse> IdentifyPlant(byte[] image)
        {
            var response = new PlantNetResponse();


            using var httpClient = new HttpClient();
            using var multipartFormContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(image);

            multipartFormContent.Add(imageContent, "images", "image.jpg");

            multipartFormContent.Add(new StringContent("flower"), "organs");

            try
            {
                var httpResponse = await httpClient.PostAsync(apiEndpoint, multipartFormContent);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    response.IsPlantDetected = true;
                    response.JsonResponse = await httpResponse.Content.ReadAsStringAsync();
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
