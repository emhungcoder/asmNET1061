using Microsoft.AspNetCore.Identity;

namespace foodService.Models
{
    // Kế thừa từ IdentityRole<int> để dùng khóa kiểu int
    public class ApplicationRole : IdentityRole<string>
    {
        // Thêm các thuộc tính mở rộng nếu cần
    }
}
