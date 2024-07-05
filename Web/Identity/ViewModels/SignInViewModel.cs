using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {

        }

        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }


        [Display(Name = "Email:")]
        public string Email { get; set; }



        [Display(Name = "Şifre:")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
