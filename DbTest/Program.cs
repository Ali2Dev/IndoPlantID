// See https://aka.ms/new-console-template for more information


using Business.Concrete;
using DataAccess.Concrete;
using Entities.Concrete;

DocumentManager documentManager = new DocumentManager(new EfDocumentDal());
documentManager.Add(new Document { Title = "Selam8" });

foreach (var item in documentManager.GetAll())
{
    Console.WriteLine(item.Title);
}
