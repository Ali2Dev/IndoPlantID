using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class EditUserViewModel
    {

        [Display(Name = "İsim:")]
        public string FirstName { get; set; } = null!;


        [Display(Name = "Soyisim:")]
        public string LastName { get; set; } = null!;


        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; } = null!;


        [Display(Name = "Email :")]
        public string Email { get; set; } = null!;


        [Display(Name = "Telefon:")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Profil resmi :")]
        public IFormFile? File { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }
    }
}
