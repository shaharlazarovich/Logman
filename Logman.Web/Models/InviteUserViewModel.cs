using System.ComponentModel.DataAnnotations;
using Logman.Common.DomainObjects;

namespace Logman.Web.Models
{
    public class InviteUserViewModel
    {
        [Required]
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string UserEmail { get; set; }

        [Required]
        public long AppId { get; set; }

        [Required]
        public Roles UserRole { get; set; }
    }
}