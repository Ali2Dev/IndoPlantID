using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class DocumentManager : IDocumentService
    {
        IDocumentDal _documentDal;


        public DocumentManager(IDocumentDal documentDal)
        {
            _documentDal = documentDal;

        }

        public void Add(Document entity)
        {
            _documentDal.Add(entity);
        }

        public void Delete(Document document)
        {
            _documentDal.Delete(document);
        }

        public List<Document> GetAll()
        {
            return _documentDal.GetAll();
        }

        public Document GetById(int id)
        {
            return _documentDal.Get(i => i.Id == id);
        }

        public void Update(Document document)
        {
            _documentDal.Update(document);
        }

        public async void PostFileAsync(IFormFile fileData, string path)
        {
            try
            {
                var document = new Document()
                {
                    StoragePath = path,
                    Title = fileData.FileName,
                    Content = fileData.ContentDisposition,
                    CreatedDate = DateTime.Now
                };

                document.DocumentExtension = Path.GetExtension(document.Title);

                //using (var stream = new MemoryStream())
                //{
                //    fileData.CopyTo(stream);

                //    document.Documentfile = stream.ToArray();
                //}

                _documentDal.Add(document);

            }
            catch (Exception)
            {
                //Güncellenecek
                throw new Document_UploadErrorException();
            }
        }
    }
}

public class Document_UploadErrorException : ApplicationException { };