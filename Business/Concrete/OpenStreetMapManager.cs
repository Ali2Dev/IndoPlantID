using Business.Abstract;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OpenStreetMapManager : IOpenStreetMapService
    {
        private readonly HttpClient _httpClient;

        public OpenStreetMapManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<(string Name, string Lat, string Lon)>> GetCoordinatesAsync(List<string> queries)
        {
            var results = new List<(string Name, string Lat, string Lon)>();

            foreach (var query in queries)
            {
                var url = $"https://nominatim.openstreetmap.org/search?format=json&polygon=0&addressdetails=0&q={Uri.EscapeDataString(query)}";

                _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");

                var response = await _httpClient.GetStringAsync(url);

                var locations = JArray.Parse(response);

                if (locations.Count > 0)
                {
                    var firstLocation = locations[0];

                    var lat = firstLocation["lat"]?.ToString();

                    var lon = firstLocation["lon"]?.ToString();

                    // Lat veya Lon null değilse listeye ekle.
                    if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lon))
                    {
                        results.Add((query, lat, lon));
                    }
                }

            }
            results.RemoveAll(x => x.Lat == "Not Found" || x.Lon == "Not Found");

            return results;
        }
    }
}
