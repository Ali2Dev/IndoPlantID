using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract
{
    public interface IDocumentService
    {
        List<Document> GetAll();

        Document GetById(int id);

        void Add(Document entity);

        void Delete(Document document);

        void Update(Document document);
        void PostFileAsync(IFormFile fileData);
    }
}
