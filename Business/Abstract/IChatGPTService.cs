using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IChatGPTService
    {
        Task<string> GetResponse(string plantName);
        Task<string> GetMaintenanceAndWatering(string plantName);


    }
}
