using System.ComponentModel.DataAnnotations;

namespace Logman.Web.Models.Apps
{
    public class AppCreateViewModel
    {
        private const int RetentionDays = 30;

        [Required]
        [Display(Name = "Application title")]
        [StringLength(100)]
        public string AppName { get; set; }

        [StringLength(36)]
        public string AppKey { get; set; }

        public long Id { get; set; }
        [Required]
        [Display(Name = "Application is enabled")]
        public bool Enabled { get; set; }

        [Required]
        [Display(Name = "Retention days (max 30 days)")]
        [Range(1, RetentionDays)]
        public int DefaultRetainPeriodDays { get; set; }

        [Required]
        [Display(Name = "Max. number of accepted Fatal errors during the retention period")]
        public int MaxFatalErrors { get; set; }

        [Required]
        [Display(Name = "Max. number of accepted unhandled errors during the retention period")]
        public int MaxErrors { get; set; }

        [Required]
        [Display(Name = "Max. number of accepted warnings during the retention period")]
        public int MaxWarnings { get; set; }

        public bool IsEditMode { get; set; }
    }
}