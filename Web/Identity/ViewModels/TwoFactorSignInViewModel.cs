using System.ComponentModel.DataAnnotations;
using Web.Enums;

namespace Web.Identity.ViewModels
{
    public class TwoFactorSignInViewModel
    {
        [Display(Name = "Doğrulama Kodu:")]
        [Required(ErrorMessage = "Bu kısım boş olamaz.")]
        [StringLength(8, ErrorMessage = "En fazla 8 haneli olabilir.")]
        public string VerificationCode { get; set; }

        public bool RememberMe { get; set; }
        public bool IsRecoveryCodeRequested { get; set; }
        public TwoFactor TwoFactorType { get; set; }

    }
}
