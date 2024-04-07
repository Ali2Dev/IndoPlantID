using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantNetSpecies
    {
        public string ScientificNameWithoutAuthor { get; set; }
        public string ScientificNameAuthorship { get; set; }
        public PlantNetGenus Genus { get; set; }
        public PlantNetFamily Family { get; set; }
        public List<string> CommonNames { get; set; }
        public string ScientificName { get; set; }
    }

}
