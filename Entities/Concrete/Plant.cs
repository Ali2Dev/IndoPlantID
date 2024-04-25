using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Plant
    {
        public int Id { get; set; }
        public string CommonName { get; set; }
        public string Slug { get; set; }
        public string ScientificName { get; set; }
        public int Year { get; set; }
        public string Bibliography { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public string Rank { get; set; }
        public string FamilyCommonName { get; set; }
        public int GenusId { get; set; }
        public string Observations { get; set; }
        public bool Vegetable { get; set; }
        public string ImageUrl { get; set; }
        public string Genus { get; set; }
        public string Family { get; set; }
        public string Duration { get; set; }
        public string EdiblePart { get; set; }
        public bool Edible { get; set; }
        public PlantImages Images { get; set; }
    }
}
