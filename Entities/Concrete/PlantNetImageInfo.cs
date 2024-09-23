using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantNetImageInfo : IEntity
    {
        public string Title { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
