using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sepidar.Admin.Models
{
    public class SliderModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public IFormFile PictureFile { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public long OrderNumber { get; set; }

        [Display(Name = "Language")]
        public long LanguageId { get; set; }

        public string Description { get; set; }

        public string PictureToken { get; set; }

        public string PhotoUrl { get; set; }

        public string LinkUrl { get; set; }
    }
}
