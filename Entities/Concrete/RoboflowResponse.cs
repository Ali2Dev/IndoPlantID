using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Entities.Concrete
{
    public class RoboflowResponse : IEntity
    {
        public int RoboflowResponseId { get; set; }
        public string? UserId { get; set; }
        public int? ClassId { get; set; }

        public int? Confidence { get; set; }

        public string? PlantName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
