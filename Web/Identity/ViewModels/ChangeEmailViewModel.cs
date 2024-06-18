using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class ChangeEmailViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Eski e-mail:")]
        public string OldEmail { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Geçerli bir e-mail girin.")]
        [Required(ErrorMessage = "Bu kısım boş olamaz.")]
        [Display(Name = "Yeni e-mail:")]
        public string NewEmail { get; set; }


        [DataType(DataType.EmailAddress, ErrorMessage = "Geçerli bir e-mail girin.")]
        [Compare(nameof(NewEmail), ErrorMessage = "E-mail uyuşmuyor.")]
        [Required(ErrorMessage = "Bu kısım boş olamaz.")]
        [Display(Name = "E-mail Tekrar:")]
        public string NewEmailConfirmed { get; set; }
    }
}
