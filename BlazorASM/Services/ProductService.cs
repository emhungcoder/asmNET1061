using System.Net.Http.Json;
using ASM.Client.Models;
namespace ASM.Client.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _http.GetFromJsonAsync<List<Product>>("api/Products");
            return products ?? new List<Product>();
        }

        public async Task<Product?> GetById(int id)
        {
            return await _http.GetFromJsonAsync<Product>($"api/Products/{id}");
        }

        public async Task<HttpResponseMessage> AddProduct(MultipartFormDataContent formData)
        {
            return await _http.PostAsync("api/Products", formData);
        }

        public async Task<HttpResponseMessage> UpdateProduct(MultipartFormDataContent formData)
        {
            return await _http.PutAsync("api/Products", formData);
        }

        public async Task<HttpResponseMessage> StopSelling(int productId)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(productId.ToString()), "ProductID");
            return await _http.PostAsync("api/Products/stop", content);
        }

        public async Task<HttpResponseMessage> Activate(int productId)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(productId.ToString()), "ProductID");
            return await _http.PostAsync("api/Products/activate", content);
        }
    }

}