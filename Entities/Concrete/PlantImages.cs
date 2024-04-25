using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantImages
    {
        public List<ImageDetail> Flower { get; set; }
        public List<ImageDetail> Leaf { get; set; }
        public List<ImageDetail> Habit { get; set; }
        public List<ImageDetail> Bark { get; set; }
        public List<ImageDetail> Fruit { get; set; }
        public List<ImageDetail> Other { get; set; }
    }
}
