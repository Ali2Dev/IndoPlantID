using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IOpenStreetMapService
    {
        Task<List<(string Name, string Lat, string Lon)>> GetCoordinatesAsync(List<string> queries);
    }
}
