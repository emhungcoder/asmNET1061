using ASM.Models;
using Microsoft.AspNetCore.Mvc;
using ASM.Data;
using Microsoft.EntityFrameworkCore;
using apiASM.Models;

namespace ASM.API.Controllers
{
    [Route("api/customer/products")]
    [ApiController]
    public class CustomerProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerProductController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy tất cả sản phẩm đang được bán (TinhTrang = "On")
        [HttpGet]
        public async Task<IActionResult> GetAllActiveProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.TinhTrang == "On")
                .ToListAsync();

            var result = products.Select(p => new ProductDTO
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                Quantity = p.Quantity,
                Color = p.Color,
                Size = p.Size,
                Image = p.Image,
                Description = p.Description,
                CategoryID = p.CategoryID,
                CategoryName = p.Category?.CategoryName,
                TinhTrang = p.TinhTrang
            }).ToList();

            return Ok(result);
        }

        // Xem chi tiết sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id && p.TinhTrang == "On");

            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại hoặc đã ngừng bán!" });

            var productDto = new ProductDTO
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                Color = product.Color,
                Size = product.Size,
                Image = product.Image,
                Description = product.Description,
                CategoryID = product.CategoryID,
                CategoryName = product.Category?.CategoryName,
                TinhTrang = product.TinhTrang
            };

            return Ok(productDto);
        }

        // Tìm kiếm sản phẩm dành cho khách hàng
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? keyword, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.TinhTrang == "On");

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.ProductName.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == categoryId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var products = await query.ToListAsync();

            var result = products.Select(p => new ProductDTO
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                Quantity = p.Quantity,
                Color = p.Color,
                Size = p.Size,
                Image = p.Image,
                Description = p.Description,
                CategoryID = p.CategoryID,
                CategoryName = p.Category?.CategoryName,
                TinhTrang = p.TinhTrang
            }).ToList();

            return Ok(result);
        }
    }
}
    