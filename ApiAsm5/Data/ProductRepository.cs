using ASM.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ASM.Data
{
    public class ProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy tất cả các sản phẩm, bao gồm cả sản phẩm đang bán và ngừng bán
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.TinhTrang == "on")
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Color = p.Color,
                    Size = p.Size,
                    Image = p.Image,
                    Description = p.Description,
                    TinhTrang = p.TinhTrang,
                    CategoryName = p.Category.CategoryName
                })
                .ToListAsync();
        }

        public async Task<List<Product>> AllPro(string status) // Thêm tham số status
        {
            return await _context.Products
                .Where(p => p.TinhTrang == status) // Dùng tham số status
                .Include(p => p.Category)
                .ToListAsync(); // Lấy sản phẩm theo trạng thái On hoặc Off
        }


        // Tìm kiếm sản phẩm theo các điều kiện
        public async Task<List<Product>> SearchProductsAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.ProductName.Contains(searchTerm));

            // Tìm kiếm theo CategoryID nếu có
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryID == categoryId.Value);

            // Tìm kiếm theo giá nếu có
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.Where(p => p.TinhTrang == "On").ToListAsync(); // Trạng thái "On" để hiển thị sản phẩm đang bán
        }

        public async Task<List<Product>> GetProductsByStatusAsync(string status)
        {
            return await _context.Products
                .Where(p => p.TinhTrang == status)
                .Include(p => p.Category)
                .ToListAsync();
        }

        // Thêm sản phẩm mới
        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // Cập nhật sản phẩm
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Cập nhật trạng thái sản phẩm (ngừng bán hoặc kích hoạt lại)
        public async Task UpdateProductStatusAsync(int productId, string status)
        {
            var product = await GetProductByIdAsync(productId);
            if (product != null)
            {
                product.TinhTrang = status;
                await _context.SaveChangesAsync();
            }
        }

        // Lấy sản phẩm theo ID
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        }
    }
}
