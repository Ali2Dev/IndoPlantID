using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{
    public class EditUserValidator : AbstractValidator<EditUserViewModel>
    {
        public EditUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .MinimumLength(2).WithMessage("Bu alan 2 karakterden az olamaz.")
                .MaximumLength(25).WithMessage("Bu alan 25 karakterden fazla olamaz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .MinimumLength(2).WithMessage("Bu alan 2 karakterden az olamaz.")
                .MaximumLength(25).WithMessage("Bu alan 25 karakterden fazla olamaz.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .EmailAddress().WithMessage("Email formatı yanlış.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$").WithMessage("Geçersiz telefon numarası formatı.")
                .Length(10).WithMessage("Bu alan 10 karakter uzunluğunda olmalıdır.");
        }
    }
}
