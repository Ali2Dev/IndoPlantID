using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantNetSpeciesResult
    {
        public double Score { get; set; }
        public PlantNetSpecies Species { get; set; }
        public PlantNetServiceInfo Gbif { get; set; }
        public PlantNetServiceInfo Powo { get; set; }
    }

}
