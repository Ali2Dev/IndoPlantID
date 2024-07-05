using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{

    public class ChangePasswordViewModel
    {
        [Display(Name = "Eski şifre:")]
        public string OldPassword { get; set; }



        [Display(Name = "Yeni şifre:")]
        public string NewPassword { get; set; }


        [Display(Name = "Yeni şifre tekrar:")]
        public string ConfirmPassword { get; set; }
    }
}
