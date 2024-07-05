using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class ForgotMyPasswordValidator : AbstractValidator<ForgotMyPasswordViewModel>
    {
        public ForgotMyPasswordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email alanı boş olamaz.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("E-mail doğru formatta değil.");
        }
    }
}
