using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class PlantNetResponse : IEntity
    {
        public string? JsonResponse { get; set; }
        public bool IsPlantDetected { get; set; }
    }
}
