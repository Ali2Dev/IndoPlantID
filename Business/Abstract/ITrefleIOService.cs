using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ITrefleIOService
    {
        Task<List<int>> SearchPlantIdsAsync(string name);

        Task<List<string>> GetPlantRegionsAsync(int plantId);
    }
}
