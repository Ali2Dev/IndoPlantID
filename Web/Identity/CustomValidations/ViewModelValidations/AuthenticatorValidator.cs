using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class AuthenticatorValidator : AbstractValidator<AuthenticatorViewModel>
    {
        public AuthenticatorValidator()
        {
            RuleFor(x => x.VerificationCode)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .Matches(@"^\d+$").WithMessage("Sadece sayı girebilirsiniz.")
                .MaximumLength(8).WithMessage("En fazla 8 haneli olabilir."); ;

            RuleFor(x => x.TwoFactorType)
                .IsInEnum().WithMessage("Geçerli bir 2 adımlı güvenlik tipi seçmelisiniz.");
        }
    }
}
