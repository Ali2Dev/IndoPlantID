using Castle.Core.Resource;
using Entities.Concrete;

namespace Web.Models
{
    public class DocumentResultViewModel
    {
        List<string>? PlantFlowerImgUrl { get; set; }
        List<string>? PlantBarkImgUrl { get; set; }
        List<string>? PlantLeafImgUrl { get; set; }
        List<string>? PlantFruitImgUrl { get; set; }
        public string StoragePath { get; set; }
        public string PlantGlobalName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? PlantChatGPTResponse { get; set; }



    }
}
