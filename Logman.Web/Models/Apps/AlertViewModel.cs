using System.ComponentModel.DataAnnotations;
using Logman.Common.DomainObjects;

namespace Logman.Web.Models.Apps
{
    public class AlertViewModel
    {
        [Display(Name="Fatal Errors")]
        public bool IncludesFatals { get; set; }

        [Display(Name = "Errors")]
        public bool IncludesErrors { get; set; }

        [Display(Name = "Warnings")]
        public bool IncludesWarnings { get; set; }

        [Required]
        public short TypeOfPeriod { get; set; }

        [Required]
        [Range(1,100,ErrorMessage= "Length of the period must be between 1 and 100")]
        public int PeriodValue { get; set; }

        [Required]
        [Range(1, 50000, ErrorMessage = "Number of events recived in the given period must be between 1 and 50,000")]
        public int Value { get; set; }

        [Required]
        public short TypeOfNotification { get; set; }

        [Required(ErrorMessage = "Email or web hook address must be specified.")]
        public string Target { get; set; }

        [Required]
        public long AppId { get; set; }
    }
}