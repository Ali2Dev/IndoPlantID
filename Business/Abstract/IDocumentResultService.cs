using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract
{
    public interface IDocumentResultService
    {
        List<DocumentResult> GetAll();

        DocumentResult GetById(int id);

        void Add(DocumentResult entity);
        void Delete(DocumentResult entity);


    }
}
