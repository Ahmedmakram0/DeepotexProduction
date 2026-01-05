using Deepotex.core.Models;
using Deepotex.core.ViewModels;
using Deepotex.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Deepotex_V2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Ensure entire controller is protected
    public class ProductController : Controller
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var products = _service.GetAllProducts();
            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _service.GetCategories();
            return View(new ProductViewModel { Features = new List<string>() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _service.GetCategories();
                return View(product);
            }

            try
            {
                if (product.Images == null) product.Images = new List<IFormFile>();
                if (Request.Form.Files.Any(f => f.Name == "Image")) product.Images.Add(Request.Form.Files["Image"]);

                var newProduct = new Product
                {
                    Name = product.Name,
                    SmallDescription = product.SmallDescription,
                    Description = product.Description,
                    Features = product.Features ?? new List<string>(),
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                };

                await _service.CreateProductAsync(newProduct, product.Images);
                TempData["Success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = _service.GetCategories();
                return View(product);
            }
        }

        public IActionResult Update(int id)
        {
            var product = _service.GetProductById(id);
            if (product == null) return NotFound();

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                SmallDescription = product.SmallDescription,
                Description = product.Description,
                Features = product.Features,
                Price = product.Price,
                CategoryId = product.CategoryId
            };

            ViewBag.Categories = _service.GetCategories();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProductViewModel product)
        {
            if (id != product.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _service.GetCategories();
                return View(product);
            }

            try
            {
                if (product.Images == null) product.Images = new List<IFormFile>();
                if (Request.Form.Files.Any(f => f.Name == "Image")) product.Images.Add(Request.Form.Files["Image"]);

                var productData = new Product
                {
                    Name = product.Name,
                    SmallDescription = product.SmallDescription,
                    Description = product.Description,
                    Features = product.Features,
                    Price = product.Price,
                    CategoryId = product.CategoryId
                };

                await _service.UpdateProductAsync(id, productData, product.Images);
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Categories = _service.GetCategories();
                return View(product);
            }
        }

        public IActionResult Delete()
        {
            var products = _service.GetAllProducts();
            return View(products);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                TempData["Error"] = "No products selected.";
                return RedirectToAction("Delete");
            }

            try
            {
                await _service.DeleteProductsAsync(ids);
                TempData["Success"] = "Products deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Delete");
            }
        }
    }
}
