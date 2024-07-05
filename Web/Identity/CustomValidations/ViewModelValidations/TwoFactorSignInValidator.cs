using FluentValidation;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class TwoFactorSignInValidator : AbstractValidator<TwoFactorSignInViewModel>
    {
        public TwoFactorSignInValidator()
        {
            RuleFor(x => x.VerificationCode)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .MaximumLength(8).WithMessage("En fazla 8 haneli olabilir.");
        }
    }
}
