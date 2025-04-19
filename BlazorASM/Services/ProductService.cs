using ASM.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<Product>> GetInactiveProductsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>("api/products/GetInactiveProducts");
            return response ?? new List<Product>();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>("api/products");
            return response ?? new List<Product>();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}");
            return response;
        }

        public async Task AddAsync(Product product, MultipartFormDataContent formData)
        {
            var response = await _httpClient.PostAsync("api/products", formData);
            response.EnsureSuccessStatusCode();
        }


        public async Task UpdateAsync(Product product, MultipartFormDataContent formData)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(product.ProductName), "ProductName");
            content.Add(new StringContent(product.Price.ToString()), "Price");
            content.Add(new StringContent(product.Quantity.ToString()), "Quantity");
            content.Add(new StringContent(product.Color), "Color");
            content.Add(new StringContent(product.Size), "Size");
            content.Add(new StringContent(product.Description), "Description");
            content.Add(new StringContent(product.CategoryID.ToString()), "CategoryID");

            if (formData != null)
            {
                content.Add(formData, "ProductImage", formData.Headers.ContentDisposition.FileName);
            }

            var response = await _httpClient.PutAsync("api/products", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopSellingAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/products/stop/{id}", null);
            response.EnsureSuccessStatusCode();
        }


        public async Task ActivateAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/products/activate/{id}", null);
            response.EnsureSuccessStatusCode();
        }



        public async Task<List<Product>> SearchAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var queryString = $"?searchTerm={searchTerm}&categoryId={categoryId}&minPrice={minPrice}&maxPrice={maxPrice}";
            var response = await _httpClient.GetFromJsonAsync<List<Product>>($"api/products/Search{queryString}");
            return response ?? new List<Product>();
        }
    }
}
