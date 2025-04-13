using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Deepotex.core.ViewModels;
using Deepotex.EF.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Deepotex_V2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _repo;
        private readonly PhotoServices _cloudinaryService;
        private readonly IWebHostEnvironment _env;
        public ProductController(IProductRepo repo, PhotoServices cloudinaryService, IWebHostEnvironment env)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
            _env = env;
        }
        private string GetRandomBackgroundImage()
        {
            string imagesFolder = Path.Combine(_env.WebRootPath, "images");
            string[] imageFiles = Directory.GetFiles(imagesFolder, "*.jpg");
            if (imageFiles.Length == 0)
            {
                return "/images/default-image.jpg"; 
            }
            Random random = new Random();
            int randomIndex = random.Next(0, imageFiles.Length);
            string filePath = imageFiles[randomIndex];
            string fileName = Path.GetFileName(filePath);
            return $"/images/{fileName}";
        }

        public IActionResult Index()
        {
            string randomImage = GetRandomBackgroundImage();
            ViewBag.RandomBackgroundImage = randomImage;
            try
            {
                var result = _repo.GetAll();
                if (result == null || !result.Any())
                {
                    // Return an empty view instead of redirecting to avoid confusion
                    return View(Enumerable.Empty<Product>());
                }
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while retrieving products.";
                return View(Enumerable.Empty<Product>());
            }
        }

        public IActionResult Details(int id)
        {
            string randomImage = GetRandomBackgroundImage();
            ViewBag.RandomBackgroundImage = randomImage;
            if (id <= 0)
            {
                return BadRequest("Invalid product ID.");
            }

            try
            {
                var result = _repo.GetById(id);
                if (result == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                
                // Get related products (products in the same category or random products if no category)
                var relatedProducts = new List<Product>();
                if (result.CategoryId.HasValue)
                {
                    // Get products from the same category, excluding the current product
                    relatedProducts = _repo.GetAll()
                        .Where(p => p.CategoryId == result.CategoryId && p.Id != result.Id)
                        .Take(3)
                        .ToList();
                }
                
                // If we don't have enough related products, add some random ones
                if (relatedProducts.Count < 3)
                {
                    var randomProducts = _repo.GetAll()
                        .Where(p => p.Id != result.Id && !relatedProducts.Select(rp => rp.Id).Contains(p.Id))
                        .OrderBy(p => Guid.NewGuid()) // Random ordering
                        .Take(3 - relatedProducts.Count)
                        .ToList();
                    
                    relatedProducts.AddRange(randomProducts);
                }
                
                ViewBag.RelatedProducts = relatedProducts;
                
                return View("Details", result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while retrieving product details.";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public IActionResult Create()
        {
            try
            {
                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(new ProductViewModel { Features = new List<string>() });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the create form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            if (product == null)
            {
                return BadRequest("Product data is missing.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(product);
            }

            try
            {
                if (product.Image == null || product.Image.Length == 0)
                {
                    ModelState.AddModelError("Image", "Please select an image to upload.");
                    ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                    return View(product);
                }

                // Upload to Cloudinary
                string imageUrl = await _cloudinaryService.UploadProductImageAsync(product.Image);
                if (string.IsNullOrEmpty(imageUrl))
                {
                    ModelState.AddModelError("Image", "Failed to upload image to Cloudinary.");
                    ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                    return View(product);
                }

                var newProduct = new Product
                {
                    Name = product.Name,
                    SmallDescription = product.SmallDescription,
                    Description = product.Description,
                    Features = product.Features ?? new List<string>(),
                    Price = product.Price,
                    CategoryId = 1, // Default to 1 if null
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _repo.Add(newProduct);
                _repo.Save();

                TempData["Success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while creating the product: {ex.Message}";
                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(product);
            }
        }

        [Authorize]
        
        public IActionResult Delete()
        {
            try
            {
                var products = _repo.GetAll();
                return View(products ?? Enumerable.Empty<Product>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while retrieving products for deletion.";
                return RedirectToAction("Index");
            }
        }

        
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                TempData["Error"] = "No products selected for deletion.";
                return RedirectToAction("Delete");
            }

            try
            {
                int deletedCount = 0;
                foreach (var id in ids)
                {
                    if (id <= 0) continue; // Skip invalid IDs

                    var product = _repo.GetById(id);
                    if (product != null)
                    {
                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            await _cloudinaryService.DeleteProductImageAsync(product.ImageUrl);
                        }
                        _repo.Delete(id);
                        deletedCount++;
                    }
                }

            if (deletedCount > 0)
                {
                    TempData["Success"] = $"{deletedCount} product(s) deleted successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting products: {ex.Message}";
                return RedirectToAction("Delete");
            }
        }

        [Authorize]
        public IActionResult Update(int id)
        {
            try
            {
                var product = _repo.GetById(id);
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var viewModel = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    SmallDescription = product.SmallDescription,
                    Description = product.Description,
                    Features = product.Features,
                    Price = product.Price
                };

                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the update form.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductViewModel product)
        {
            if (id != product.Id)
            {
                TempData["Error"] = "Product ID mismatch.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(product);
            }

            try
            {
                var existingProduct = _repo.GetById(id);
                if (existingProduct == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                // Update product properties
                existingProduct.Name = product.Name;
                existingProduct.SmallDescription = product.SmallDescription;
                existingProduct.Description = product.Description;
                existingProduct.Features = product.Features;
                existingProduct.Price = product.Price;
                existingProduct.UpdatedAt = DateTime.Now;

                // Handle image upload if a new image is provided
                if (product.Image != null && product.Image.Length > 0)
                {
                    // Delete old image from Cloudinary
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        await _cloudinaryService.DeleteProductImageAsync(existingProduct.ImageUrl);
                    }

                    // Upload new image to Cloudinary
                    string imageUrl = await _cloudinaryService.UploadProductImageAsync(product.Image);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        ModelState.AddModelError("Image", "Failed to upload image to Cloudinary.");
                        ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                        return View(product);
                    }

                    existingProduct.ImageUrl = imageUrl;
                }

                _repo.Update(id, existingProduct);
                _repo.Save();

                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while updating the product: {ex.Message}";
                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(product);
            }
        }
    }
}