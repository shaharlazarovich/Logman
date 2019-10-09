using System.ComponentModel.DataAnnotations;

namespace Logman.Web.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username must be entered.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password must be entered.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}