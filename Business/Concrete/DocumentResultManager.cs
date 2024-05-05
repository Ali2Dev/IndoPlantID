using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;

namespace Business.Concrete
{
    public class DocumentResultManager : IDocumentResultService
    {
        IDocumentResultDal _documentResultDal;

        public DocumentResultManager(IDocumentResultDal documentResultDal)
        {
            _documentResultDal = documentResultDal;
        }

        public List<DocumentResult> GetAll()
        {
            return _documentResultDal.GetAll();
        }

        public DocumentResult GetById(int id)
        {
            return _documentResultDal.Get(i => i.Id == id);
        }

        public void Add(DocumentResult entity)
        {
            _documentResultDal.Add(entity);
        }

        public void Delete(DocumentResult entity)
        {
            _documentResultDal.Delete(entity);
        }
    }
}
