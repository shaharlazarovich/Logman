using System.ComponentModel.DataAnnotations;

namespace Logman.Web.Models.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        public long? UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password and its repetition mismatch.")]
        public string RepeatNewPassword { get; set; }
    }
}