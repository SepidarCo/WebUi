
namespace Sepidar.Admin.Models
{
    public class LoginResponseModel
    {
        public string Status { get; set; }

        public UserData Result { get; set; }

        public string Message { get; set; }
    }
}
