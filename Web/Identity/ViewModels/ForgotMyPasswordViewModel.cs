using System.ComponentModel.DataAnnotations;

namespace Web.Identity.ViewModels
{
    public class ForgotMyPasswordViewModel
    {

        public ForgotMyPasswordViewModel()
        {

        }

        public ForgotMyPasswordViewModel(string email)
        {
            Email = email;
        }



        public string Email { get; set; }

    }
}
