using Deepotex.core.Models;
using Deepotex.core.ViewModels;
using Deepotex.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Deepotex_V2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _service;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductService service, IWebHostEnvironment env)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public IActionResult Index()
        {
            ViewBag.RandomBackgroundImage = _service.GetRandomBackgroundImage(_env.WebRootPath);
            try
            {
                var result = _service.GetAllProducts();
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
            ViewBag.RandomBackgroundImage = _service.GetRandomBackgroundImage(_env.WebRootPath);
            if (id <= 0) return BadRequest("Invalid product ID.");

            try
            {
                var result = _service.GetProductById(id);
                if (result == null) return NotFound($"Product with ID {id} not found.");

                ViewBag.RelatedProducts = _service.GetRelatedProducts(result.Id, result.CategoryId);
                return View("Details", result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while retrieving product details.";
                return RedirectToAction("Index");
            }
        }

    }
}