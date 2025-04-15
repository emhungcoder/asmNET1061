using System.ComponentModel.DataAnnotations.Schema;

namespace ASM5.Models
{
    [NotMapped]
    public class UpdateProfileModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
