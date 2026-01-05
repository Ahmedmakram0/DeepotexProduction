using System.Diagnostics;
using Deepotex.core.Models;
using Deepotex.core.ViewModels;
using Deepotex.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Deepotex_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _service;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IProductService service, IWebHostEnvironment env)
        {
            _logger = logger;
            _service = service;
            _env = env;
        }

        public IActionResult Index()
        {
            ViewBag.RandomBackgroundImage = _service.GetRandomBackgroundImage(_env.WebRootPath);
            var result = _service.GetLatestProducts();
            return View(result);
        }

        public IActionResult Contact()
        {
            ViewBag.RandomBackgroundImage = _service.GetRandomBackgroundImage(_env.WebRootPath);
            return View();
        }
        
        public IActionResult About()
        {
            ViewBag.RandomBackgroundImage = _service.GetRandomBackgroundImage(_env.WebRootPath);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
