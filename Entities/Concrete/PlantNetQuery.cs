using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantNetQuery
    {
        public string Project { get; set; }
        public List<string> Images { get; set; }
        public List<string> Organs { get; set; }
        public bool IncludeRelatedImages { get; set; }
        public bool NoReject { get; set; }
    }

}
