using ASM.Data;
using ASM.Models;
using apiASM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả danh mục
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDTO
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }

        // Lấy danh mục theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return Ok(new CategoryDTO
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName
            });
        }

        // Thêm danh mục
        [Authorize(Roles = "Employee,Manager")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO model)
        {
            var category = new Category
            {
                CategoryName = model.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Danh mục đã được tạo!" });
        }

        // Cập nhật danh mục
        [Authorize(Roles = "Employee,Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO model)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            category.CategoryName = model.CategoryName;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật danh mục thành công!" });
        }

        // Xoá danh mục
        [Authorize(Roles = "Employee,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Danh mục đã được xoá!" });
        }
    }
}
