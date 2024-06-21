using System.ComponentModel.DataAnnotations;
using Web.Enums;

namespace Web.Identity.ViewModels
{
    public class AuthenticatorViewModel
    {
        [Display(Name = "Key:")]
        public string SharedKey { get; set; }

        [Display(Name = "QR:")]
        public string AuthenticatorUri { get; set; }

        [Display(Name = "Doğrulama Kodu:")]
        [Required(ErrorMessage = "Bu kısım boş olamaz.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Sadece sayı girebilirsiniz.")]
        public string VerificationCode { get; set; }

        [Display(Name = "2 adımlı güvenlik tipi:")]
        public TwoFactor TwoFactorType { get; set; }


    }
}
