using Entities.Concrete;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    public class PlantNetValidator : AbstractValidator<PlantNetImageInfo>
    {
        public PlantNetValidator()
        {
            RuleFor(p => GetExtension(p.Title)).Must(IsImage).WithMessage("Uploaded file has to be an image");
        }

        public bool IsImage(string docExtension)
        {
            var docExtensionUpper = docExtension.ToUpper();
            switch (docExtensionUpper)
            {
                case ".WEBP":
                //case ".PNG":
                //case ".JPG":
                    return false;
                default:
                    return true;
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
