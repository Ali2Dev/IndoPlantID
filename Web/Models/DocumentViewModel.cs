using Castle.Core.Resource;
using Entities.Concrete;

namespace Web.Models
{
    public class DocumentViewModel
    {
        public RoboflowResponse RoboflowResponse { get; set; }
        public List<Document> Documents { get; set; }
    }
}
