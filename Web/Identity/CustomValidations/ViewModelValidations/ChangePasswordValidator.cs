using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .Equal(x => x.NewPassword).WithMessage("Şifreler uyuşmuyor.");
        }
    }
}
