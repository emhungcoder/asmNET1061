using ASM.Client.Models;

namespace ASM.Client.Services
{
    public interface ICustomerProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
    }
}
