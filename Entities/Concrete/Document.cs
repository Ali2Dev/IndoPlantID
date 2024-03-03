using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Document : IEntity
    {

        public int Id { get; set; }
        //public int Correspondent { get; set; }
        //public int DocumentType { get; set; }
        //public int? StoragePath { get; set; }
        //public string Key { get; set; }

        //public string? DocumentExtension { get; set; }

        public string Title { get; set; }
        //public string? Content { get; set; }
        ////public List<int?> Tags { get; set; } = new List<int?>();
        //public byte[] Documentfile { get; set; }
        //public DateTime Created { get; set; }

        //public DateTime CreatedDate { get; set; }
        //public DateTime Modified { get; set; }
        //public DateTime Added { get; set; }
        //public string? ArchiveSerialNumber { get; set; }

        //public string? OriginalFileName { get; set; }

        //public string? ArchivedFileName { get; set; }
        //public string? OcrResult { get; set; }
    }
}
