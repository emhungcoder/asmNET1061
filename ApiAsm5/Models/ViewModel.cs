using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASM.Models
{
    [NotMapped]
    public class ChangePasswordModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
    [NotMapped]
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    [NotMapped]
    public class ProductCreateModel
    {
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile ProductImage { get; set; } = null!;
        public int CategoryID { get; set; }
    }
    [NotMapped]
    public class ProductUpdateModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile? ProductImage { get; set; }
    }
    [NotMapped]
    public class UpdateProfileModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
    [NotMapped]
    public class RegisterModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }  // Thêm số điện thoại
    }
    [NotMapped]
    public class EmployeeDto
    {

        public string? Id { get; set; }
        public required string FullName { get; set; }
        public string Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? TinhTrangHoatDong { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

    }
}
