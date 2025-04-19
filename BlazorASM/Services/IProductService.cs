using ASM.Client.Models;
namespace ASM.Client.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetInactiveProductsAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product, MultipartFormDataContent formData);
        Task UpdateAsync(Product product, MultipartFormDataContent formData);
        Task StopSellingAsync(int id);
        Task ActivateAsync(int id);
        Task<List<Product>> SearchAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice);
    }
}
