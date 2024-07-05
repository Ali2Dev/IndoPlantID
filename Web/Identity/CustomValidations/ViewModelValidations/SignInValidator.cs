using FluentValidation;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{

    public class SignInValidator : AbstractValidator<SignInViewModel>
    {
        public SignInValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş bırakılamaz.")
                .EmailAddress().WithMessage("Email formatı yanlış.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı boş bırakılamaz.");
        }
    }
}
