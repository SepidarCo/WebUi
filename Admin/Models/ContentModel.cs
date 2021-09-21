using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sepidar.Admin.Models
{
    public class ContentModel
    {
        public long Id { get; set; }

        [Display(Name = "Language")]
        public long LanguageId { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public long OrderNumber { get; set; }

        public string PhotoUrl { get; set; }

        public IFormFile PictureFile { get; set; }

        public long FilterLanguageId { get; set; }
    }
}
