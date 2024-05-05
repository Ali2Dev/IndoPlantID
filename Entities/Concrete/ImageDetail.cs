using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class ImageDetail
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Copyright { get; set; }

        // Foreign keys for each DocumentResult collection
        public int DocumentResultFlowerId { get; set; }
        public int DocumentResultLeafId { get; set; }
        public int DocumentResultBarkId { get; set; }
        public int DocumentResultFruitId { get; set; }
    }

}
