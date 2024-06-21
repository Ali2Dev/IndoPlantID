using System.ComponentModel.DataAnnotations;

namespace Web.Enums
{
    public enum TwoFactor
    {
        [Display(Name = "Hiçbiri")]
        None = 0,

        [Display(Name = "Microsoft/Google (Önerilen)")]
        MicrosoftGoogle = 1,

        [Display(Name = "SMS")]
        SMS = 2
    }
}
