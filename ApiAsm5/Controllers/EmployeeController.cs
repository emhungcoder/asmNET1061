using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ASM.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Lấy danh sách nhân viên
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var users = _userManager.Users.ToList();
            var employeeDtos = new List<EmployeeDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                employeeDtos.Add(new EmployeeDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    TinhTrangHoatDong = user.TinhTrangHoatDong,
                    Roles = roles.ToList()
                });
            }

            return Ok(employeeDtos);
        }

        // Thêm nhân viên mới (mặc định role là "Employee")
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromForm] EmployeeDto model, [FromForm] string password)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                return BadRequest(new { message = "Email đã tồn tại." });
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                TinhTrangHoatDong = "On"
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            return Ok(new { message = "Thêm nhân viên thành công." });
        }

        // Cập nhật vai trò thành Quản lý
        [HttpPost("updaterole")]
        public async Task<IActionResult> UpdateRole([FromBody] UserIdRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            if (!await _roleManager.RoleExistsAsync("Manager"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            await _userManager.AddToRoleAsync(user, "Manager");
            return Ok(new { message = "Cập nhật vai trò thành Quản lý thành công." });
        }

        // Ngừng hoạt động
        [HttpPost("deactivate")]
        public async Task<IActionResult> Deactivate([FromBody] UserIdRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            user.TinhTrangHoatDong = "Off";
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Ngừng hoạt động thành công." });
        }

        // Kích hoạt
        [HttpPost("activate")]
        public async Task<IActionResult> Activate([FromBody] UserIdRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            user.TinhTrangHoatDong = "On";
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Kích hoạt thành công." });
        }
    }

    public class UserIdRequest
    {
        public string UserId { get; set; }
    }
}
