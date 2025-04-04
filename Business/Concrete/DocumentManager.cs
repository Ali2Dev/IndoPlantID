﻿using Business.Abstract;
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
using System.Net.Mime;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;

namespace Business.Concrete
{
    public class DocumentManager : IDocumentService
    {
        IDocumentDal _documentDal;



        public DocumentManager(IDocumentDal documentDal)
        {
            _documentDal = documentDal;

        }

        [ValidationAspect(typeof(DocumentValidator))]
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

       
        public async void PostFileAsync(string userId,string fileName, string contentDisposition,string randomFileName)
        {
            try
            {
                var document = new Document()
                {
                    StoragePath = randomFileName,
                    Title = fileName,
                    Content = contentDisposition,
                    CreatedDate = DateTime.Now,
                    UserId = userId

                };

                document.DocumentExtension = Path.GetExtension(document.Title);

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