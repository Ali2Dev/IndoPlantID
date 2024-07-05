using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class ChangeEmailValidator : AbstractValidator<ChangeEmailViewModel>
    {
        public ChangeEmailValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.");

            RuleFor(x => x.OldEmail)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-mail girin.");

            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-mail girin.");

            RuleFor(x => x.NewEmailConfirmed)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-mail girin.")
                .Equal(x => x.NewEmail).WithMessage("E-mail uyuşmuyor.");
        }
    }
}
