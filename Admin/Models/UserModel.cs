using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;

namespace Sepidar.Admin.Models
{
    public class UserModel
    {
        public long Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string PictureToken { get; set; }

        public string PhotoUrl { get; set; }

        public IFormFile PictureFile { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
