using System.ComponentModel.DataAnnotations;

namespace Sepidar.Admin.Models
{
    public class CalendarModel
    {
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string ShortDate { get; set; }

        [Required]
        public string Date { get; set; }

        public string Address { get; set; }

        [Display(Name = "Language")]
        public long LanguageId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public long OrderNumber { get; set; }

        public string LinkUrl { get; set; }
    }
}