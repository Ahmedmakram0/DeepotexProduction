using Deepotex.core.Models;
using Deepotex.EF.Repos;
using Deepotex.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Deepotex.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _repo;
        private readonly PhotoServices _cloudinaryService;

        public ProductService(IProductRepo repo, PhotoServices cloudinaryService)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _repo.GetAll();
        }

        public IEnumerable<Product> GetLatestProducts()
        {
            return _repo.GetLatest();
        }

        public Product? GetProductById(int id)
        {
            return _repo.GetById(id);
        }

        public IEnumerable<Category> GetCategories()
        {
            return _repo.GetCategories() ?? Enumerable.Empty<Category>();
        }

        public IEnumerable<Product> GetRelatedProducts(int productId, int categoryId)
        {
            var relatedProducts = new List<Product>();
            
            // Logic moved from Controller:
            // Assuming we want to search by CategoryId, but currently logic was specific to CategoryId == 1
            // We should make it generic:
            if (categoryId > 0)
            {
                relatedProducts = _repo.GetAll()
                    .Where(p => p.CategoryId == categoryId && p.Id != productId)
                    .Take(3)
                    .ToList();
            }

            if (relatedProducts.Count < 3)
            {
                var randomProducts = _repo.GetAll()
                    .Where(p => p.Id != productId && !relatedProducts.Select(rp => rp.Id).Contains(p.Id))
                    .OrderBy(p => Guid.NewGuid())
                    .Take(3 - relatedProducts.Count)
                    .ToList();
                relatedProducts.AddRange(randomProducts);
            }

            return relatedProducts;
        }

        public async Task<Product> CreateProductAsync(Product product, List<IFormFile> images)
        {
            if (images == null || !images.Any())
            {
                 throw new InvalidOperationException("At least one image is required.");
            }

            var imageUrls = await _cloudinaryService.UploadProductImagesAsync(images);
            if (!imageUrls.Any())
            {
                throw new InvalidOperationException("Failed to upload images to Cloudinary.");
            }

            product.ImageUrls = imageUrls;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            // Bug Fix: Previously CategoryId was hardcoded to 1. 
            // Now we respect the CategoryId passed in the product object.
            
            _repo.Add(product);
            _repo.Save();

            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, Product productData, List<IFormFile> newImages)
        {
             var existingProduct = _repo.GetById(id);
             if (existingProduct == null)
             {
                 throw new KeyNotFoundException($"Product with ID {id} not found.");
             }

             existingProduct.Name = productData.Name;
             existingProduct.SmallDescription = productData.SmallDescription;
             existingProduct.Description = productData.Description;
             existingProduct.Features = productData.Features;
             existingProduct.Price = productData.Price;
             existingProduct.CategoryId = productData.CategoryId; // Updated Bug Fix
             existingProduct.UpdatedAt = DateTime.Now;

             if (newImages != null && newImages.Any(f => f.Length > 0))
             {
                 // Delete old images
                 if (existingProduct.ImageUrls.Any())
                 {
                     await _cloudinaryService.DeleteProductImagesAsync(existingProduct.ImageUrls);
                 }
                 
                 // Upload new
                 var newImageUrls = await _cloudinaryService.UploadProductImagesAsync(newImages);
                 if (!newImageUrls.Any())
                 {
                     throw new InvalidOperationException("Failed to upload new images.");
                 }
                 existingProduct.ImageUrls = newImageUrls;
             }

             if (!existingProduct.ImageUrls.Any())
             {
                 throw new InvalidOperationException("Product must have at least one image.");
             }

             _repo.Update(id, existingProduct);
             _repo.Save();

             return existingProduct;
        }

        public async Task DeleteProductsAsync(int[] ids)
        {
             foreach (var id in ids)
             {
                 if (id <= 0) continue;

                 var product = _repo.GetById(id);
                 if (product != null)
                 {
                     if (product.ImageUrls.Any())
                     {
                         await _cloudinaryService.DeleteProductImagesAsync(product.ImageUrls);
                     }
                     _repo.Delete(id);
                 }
             }
        }

        public string GetRandomBackgroundImage(string webRootPath)
        {
            string imagesFolder = Path.Combine(webRootPath, "images");
            if (!Directory.Exists(imagesFolder)) return "/images/saudi1.jpg";

            string[] imageFiles = Directory.GetFiles(imagesFolder, "*.jpg");
            if (imageFiles.Length == 0)
            {
                return "/images/saudi1.jpg";
            }
            Random random = new Random();
            int randomIndex = random.Next(0, imageFiles.Length);
            string filePath = imageFiles[randomIndex];
            string fileName = Path.GetFileName(filePath);
            return $"/images/{fileName}";
        }
    }
}
