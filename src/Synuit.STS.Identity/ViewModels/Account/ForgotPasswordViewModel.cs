using System.ComponentModel.DataAnnotations;

namespace Synuit.Idp.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}






