using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class DocumentTypeDto : IDto
    {
        public string DocumentExtension { get; set; }
    }
}
