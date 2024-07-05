using System.ComponentModel.DataAnnotations;
using Web.Enums;

namespace Web.Identity.ViewModels
{
    public class TwoFactorSignInViewModel
    {
        [Display(Name = "Doğrulama Kodu:")]
        public string VerificationCode { get; set; }

        public bool RememberMe { get; set; }
        public bool IsRecoveryCodeRequested { get; set; }
        public TwoFactor TwoFactorType { get; set; }

    }
}
