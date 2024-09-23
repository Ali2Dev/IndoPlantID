using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class DocumentValidator : AbstractValidator<Document>
    {
        public DocumentValidator()
        {
            RuleFor(d => GetExtension(d.Title)).Must(IsImage).WithMessage("Uploaded file has to be an image");
        }

        public bool IsImage(string docExtension)
        {
            var docExtensionUpper = docExtension.ToUpper();
            switch(docExtensionUpper) 
            {
                case ".JPEG":
                case ".PNG":
                case ".JPG":
                    return true;
                default: 
                    return false;
            }
        }

        public static string GetExtension(string name)
        {
            int index = name.LastIndexOf('.');

            if (index == -1)
                return "";

            if (index == name.Length - 1)
                return "";

            return name.Substring(index);
        }


    }
}
