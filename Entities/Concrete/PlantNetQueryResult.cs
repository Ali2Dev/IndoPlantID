using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantNetQueryResult
    {
        public PlantNetQuery Query { get; set; }
        public string? Language { get; set; }
        public string? PreferedReferential { get; set; }
        public string? BestMatch { get; set; }
        public List<PlantNetSpeciesResult> Results { get; set; }
        public string? Version { get; set; }
        public int RemainingIdentificationRequests { get; set; }
    }

}
