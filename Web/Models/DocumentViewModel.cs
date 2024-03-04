using Castle.Core.Resource;
using Entities.Concrete;

namespace Web.Models
{
    public class DocumentViewModel
    {
        public byte[] FileContent { get; set; }
        public List<Document> Documents { get; set; }
    }
}
