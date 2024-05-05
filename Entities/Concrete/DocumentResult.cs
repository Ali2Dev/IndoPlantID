using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Entities.Concrete
{
    public class DocumentResult : IEntity
    {
        public int Id { get; set; }


        public string StoragePath { get; set; }

        public string DocumentExtension { get; set; }

        public DateTime CreatedDate { get; set; }


        public string Content { get; set; }
        public string UserId { get; set; }

        //--
        public string PlantGlobalName { get; set; }
        public string? RoboflowJsonModel { get; set; }



        public string? FlowerUrl { get; set; }
        public string? LeafUrl { get; set; }
        public string? BarkUrl { get; set; }
        public string? FruitUrl { get; set; }
        //--

        //public List<PlantCoordinate> PlantCoordinates { get; set; }

        public string? PlantChatGPTResponse { get; set; }
    }
}
