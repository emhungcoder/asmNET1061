using ASM.Client.Models;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public class CustomerProductService : ICustomerProductService
    {
        private readonly HttpClient _httpClient;

        public CustomerProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<Product>>("api/products/get-all");
            return result ?? new List<Product>();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}");
            return result;
        }
    }
}
