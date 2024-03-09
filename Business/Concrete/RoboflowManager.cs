using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.Concrete;
using static System.Net.Mime.MediaTypeNames;

namespace Business.Concrete
{
    public class RoboflowManager : IRoboflowService
    {
        public RoboflowResponse GetResponse(byte[] image, string fileName)
        {
            string encoded = Convert.ToBase64String(image);
            byte[] data = Encoding.ASCII.GetBytes(encoded);
            string API_KEY = "5SuSzgtvAHrdfyTSBKBX"; // Your API Key
            string MODEL_ENDPOINT = "ev-bitkisi-tanima/1"; // Set model endpoint

            // Construct the URL
            string uploadURL =
                "https://detect.roboflow.com/" + MODEL_ENDPOINT + "?api_key=" + API_KEY
                + "&name=" + fileName;

            // Send HTTP request and get response
            string responseContent = SendHttpRequest(uploadURL, data);

            if (!string.IsNullOrEmpty(responseContent))
            {
                try
                {
                    // Deserialize JSON response to RootObject model
                    var rootObject = JsonSerializer.Deserialize<RootObject>(responseContent);

                    // Get the first prediction
                    var prediction = rootObject.predictions[0];

                    // Create RoboFlowResponseModel from prediction
                    var roboFlowResponse = new RoboflowResponse
                    {
                        //Güncellenecek - UserId

                        Confidence = (int)((prediction.confidence - (int)prediction.confidence) * 100),
                        ClassId = prediction.class_id,
                        PlantName = prediction.@class,
                        UpdatedDate = DateTime.Now
                    };

                    return roboFlowResponse;
                }
                catch (JsonException ex)
                {
                    // Handle JSON parsing errors
                    throw new Exception("Error parsing JSON response", ex);
                }
            }

            throw new InvalidOperationException("Empty response received from server");

        }

        private string SendHttpRequest(string url, byte[] data)
        {
            // Configure Request
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            // Write Data
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            // Get Response
            string responseContent = null;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr99 = new StreamReader(stream))
                    {
                        responseContent = sr99.ReadToEnd();
                    }
                }
            }

            return responseContent; // Tüm kod yollarında bir değer döndürülüyor
        }
    }
}
