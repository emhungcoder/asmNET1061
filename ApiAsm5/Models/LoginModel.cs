using System.ComponentModel.DataAnnotations.Schema;

namespace ASM5.Models
{
    [NotMapped]
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
