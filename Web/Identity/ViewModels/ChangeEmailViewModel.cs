using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class ChangeEmailViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }


        [Display(Name = " Şifre:")]
        public string Password { get; set; }

        [Display(Name = "Eski e-mail:")]
        public string OldEmail { get; set; }


        [Display(Name = "Yeni e-mail:")]
        public string NewEmail { get; set; }



        [Display(Name = "E-mail Tekrar:")]
        public string NewEmailConfirmed { get; set; }
    }
}
