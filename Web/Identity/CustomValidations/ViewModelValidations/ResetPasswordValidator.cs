using FluentValidation;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.");

            RuleFor(x => x.PasswordConfirmed)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .Equal(x => x.Password).WithMessage("Şifreler uyuşmuyor.");
        }
    }
}
