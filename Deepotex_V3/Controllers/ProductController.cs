// Deepotex_V2/Controllers/ProductController.cs
using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Deepotex.core.ViewModels;
using Deepotex.EF.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        private string GetRandomBackgroundImage()
        {
            string imagesFolder = Path.Combine(_env.WebRootPath, "images");
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

        public IActionResult Index()
        {
            string randomImage = GetRandomBackgroundImage();
            ViewBag.RandomBackgroundImage = randomImage;
            try
            {
                var result = _repo.GetAll();
                return View(result ?? Enumerable.Empty<Product>());
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

                var relatedProducts = new List<Product>();
                if (result.CategoryId==1)
                {
                    relatedProducts = _repo.GetAll()
                        .Where(p => p.CategoryId == result.CategoryId && p.Id != result.Id)
                        .Take(3)
                        .ToList();
                }

                if (relatedProducts.Count < 3)
                {
                    var randomProducts = _repo.GetAll()
                        .Where(p => p.Id != result.Id && !relatedProducts.Select(rp => rp.Id).Contains(p.Id))
                        .OrderBy(p => Guid.NewGuid())
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
                return View(new ProductViewModel
                {
                    Features = new List<string>(),
                    Images = new List<IFormFile>()
                });
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

            // Handle single image as a list for compatibility
            if (product.Images == null)
            {
                product.Images = new List<IFormFile>();
                if (Request.Form.Files.Any(f => f.Name == "Image"))
                {
                    product.Images.Add(Request.Form.Files["Image"]);
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                return View(product);
            }

            try
            {
                if (!product.Images.Any(f => f != null && f.Length > 0))
                {
                    ModelState.AddModelError("Images", "Please select at least one image to upload.");
                    ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                    return View(product);
                }

                var imageUrls = await _cloudinaryService.UploadProductImagesAsync(product.Images);
                if (!imageUrls.Any())
                {
                    ModelState.AddModelError("Images", "Failed to upload images to Cloudinary.");
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
                    CategoryId =  1,
                    ImageUrls = imageUrls,
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
                    if (id <= 0)
                        continue;

                    var product = _repo.GetById(id);
                    if (product != null)
                    {
                        if (product.ImageUrls.Any())
                        {
                            await _cloudinaryService.DeleteProductImagesAsync(product.ImageUrls);
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
                    Price = product.Price,
                    Images = new List<IFormFile>()
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

            // Handle single image as a list for compatibility
            if (product.Images == null)
            {
                product.Images = new List<IFormFile>();
                if (Request.Form.Files.Any(f => f.Name == "Image"))
                {
                    product.Images.Add(Request.Form.Files["Image"]);
                }
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

                existingProduct.Name = product.Name;
                existingProduct.SmallDescription = product.SmallDescription;
                existingProduct.Description = product.Description;
                existingProduct.Features = product.Features;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = 1;
                existingProduct.UpdatedAt = DateTime.Now;

                if (product.Images.Any(f => f != null && f.Length > 0))
                {
                    // Delete old images from Cloudinary
                    if (existingProduct.ImageUrls.Any())
                    {
                        await _cloudinaryService.DeleteProductImagesAsync(existingProduct.ImageUrls);
                    }

                    // Upload new images
                    var newImageUrls = await _cloudinaryService.UploadProductImagesAsync(product.Images);
                    if (!newImageUrls.Any())
                    {
                        ModelState.AddModelError("Images", "Failed to upload images to Cloudinary.");
                        ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                        return View(product);
                    }

                    existingProduct.ImageUrls = newImageUrls;
                }

                if (!existingProduct.ImageUrls.Any())
                {
                    ModelState.AddModelError("Images", "Product must have at least one image.");
                    ViewBag.Categories = _repo.GetCategories() ?? Enumerable.Empty<Category>();
                    return View(product);
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