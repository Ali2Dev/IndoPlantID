using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace Entities.Concrete
{
    public class RootObject : IEntity
    {
        public double time { get; set; }
        public Image image { get; set; }
        public List<Prediction> predictions { get; set; }
    }
}
