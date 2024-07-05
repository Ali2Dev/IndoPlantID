using FluentValidation;
using Web.Identity.ViewModels;

namespace Web.Identity.CustomValidations.ViewModelValidations
{

    public class SignUpValidator : AbstractValidator<SignUpViewModel>
    {
        public SignUpValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .MinimumLength(2).WithMessage("Bu alan 2 karakterden az olamaz.")
                .MaximumLength(15).WithMessage("Bu alan 15 karakterden fazla olamaz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .EmailAddress().WithMessage("E-mail adresiniz doğru formatta değil.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .MinimumLength(2).WithMessage("Bu alan 2 karakterden az olamaz.")
                .MaximumLength(25).WithMessage("Bu alan 25 karakterden fazla olamaz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .MinimumLength(2).WithMessage("Bu alan 2 karakterden az olamaz.")
                .MaximumLength(25).WithMessage("Bu alan 25 karakterden fazla olamaz.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$").WithMessage("Geçersiz telefon numarası formatı.")
                .Length(10).WithMessage("Bu alan 10 karakter uzunluğunda olmalıdır.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.");

            RuleFor(x => x.PasswordConfirmed)
                .NotEmpty().WithMessage("Bu kısım boş olamaz.")
                .Equal(x => x.Password).WithMessage("Şifreler uyuşmuyor.");
        }
    }
}
