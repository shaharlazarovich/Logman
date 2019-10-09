using System.ComponentModel.DataAnnotations;

namespace Logman.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email must be entered.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

    }
}