using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class SignUpViewModel
    {

        public SignUpViewModel()
        {

        }

        public SignUpViewModel(string username, string email, string phoneNumber, string password, string passwordConfirmed)
        {
            Username = username;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            PasswordConfirmed = passwordConfirmed;
        }


        [Display(Name = "Kullanıcı adı:")]
        public string Username { get; set; }


        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Display(Name = "İsim:")]
        public string FirstName { get; set; }


        [Display(Name = "Soy isim:")]
        public string LastName { get; set; }


        [Display(Name = "Telefon:")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Şifre:")]
        public string Password { get; set; }


        [Display(Name = "Şifre Tekrar:")]
        public string PasswordConfirmed { get; set; }
    }
}

