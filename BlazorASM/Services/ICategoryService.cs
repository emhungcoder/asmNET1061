using ASM.Client.Models;

namespace ASM.Client.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(int id, Category category);

        Task DeleteAsync(int id);
    }
}
