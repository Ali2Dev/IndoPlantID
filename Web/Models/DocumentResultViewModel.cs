using Castle.Core.Resource;
using Entities.Concrete;

namespace Web.Models
{
    public class DocumentResultViewModel
    {
        public List<string>? PlantFlowerImgUrl { get; set; }
        public List<string>? PlantBarkImgUrl { get; set; }
        public List<string>? PlantLeafImgUrl { get; set; }
        public List<string>? PlantFruitImgUrl { get; set; }
        public string StoragePath { get; set; }
        public string PlantGlobalName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? PlantChatGPTResponse { get; set; }
        public string? PlantGPTResponseMaintenanceWatering { get; set; }



    }
}
