using Deepotex.core.Models;
using Microsoft.AspNetCore.Http;

namespace Deepotex.Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetLatestProducts();
        Product? GetProductById(int id);
        IEnumerable<Product> GetRelatedProducts(int productId, int categoryId);
        IEnumerable<Category> GetCategories();
        Task<Product> CreateProductAsync(Product product, List<IFormFile> images);
        Task<Product> UpdateProductAsync(int id, Product product, List<IFormFile> newImages);
        Task DeleteProductsAsync(int[] ids);
        string GetRandomBackgroundImage(string webRootPath);
    }
}
