using ASM.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ASM.Client.Services;
namespace ASM.Client.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy tất cả danh mục
        public async Task<List<Category>> GetAllAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Category>>("api/categories");
            return response ?? new List<Category>();
        }

        // Lấy danh mục theo ID
        public async Task<Category?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<Category>($"api/categories/{id}");
            return response;
        }

        // Thêm danh mục mới
        public async Task AddAsync(Category category)
        {
            var response = await _httpClient.PostAsJsonAsync("api/categories", category);
            response.EnsureSuccessStatusCode();
        }

        // Cập nhật danh mục
        public async Task UpdateAsync(int id, Category category)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", category);
            response.EnsureSuccessStatusCode();
        }


        // Xóa danh mục
        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/categories/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
