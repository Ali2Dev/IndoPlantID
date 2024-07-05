using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel()
        {

        }

        public ResetPasswordViewModel(string password, string passwordConfirmed)
        {
            Password = password;
            PasswordConfirmed = passwordConfirmed;
        }



        [Display(Name = "Yeni şifre:")]
        public string Password { get; set; }


        [Display(Name = "Şifre tekrar:")]
        public string PasswordConfirmed { get; set; }
    }
}
