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

        //Plant Images
        //public PlantImages PlantImages { get; set; }

        public List<ImageDetail> PlantFlowerImages { get; set; }
        public List<ImageDetail> PlantLeafImages { get; set; }
        public List<ImageDetail> PlantBarkImages { get; set; }
        public List<ImageDetail> PlantFruitImages { get; set; }
        //--

        public List<PlantCoordinate> PlantCoordinates { get; set; }

        public string PlantChatGPTResponse { get; set; }
    }
}
