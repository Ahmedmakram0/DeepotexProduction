using System.Diagnostics;
using Deepotex.core.Models;
using Deepotex.core.Repositories;
using Deepotex.core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Deepotex_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        protected readonly IProductRepo _repo;
        private readonly IWebHostEnvironment _env;
        public HomeController(ILogger<HomeController> logger, IProductRepo repo, IWebHostEnvironment env)
        {
            _logger = logger;
            _repo = repo;
            _env = env;
        }


        public IActionResult Index()
        {
            string randomImage = GetRandomBackgroundImage();
            ViewBag.RandomBackgroundImage = randomImage;
            var result = _repo.GetLatest();
            if (result.Count==0)
            {
                return View();
            }
            return View(result);
        }
        private string GetRandomBackgroundImage()
        {
            string imagesFolder = Path.Combine(_env.WebRootPath, "images");
            string[] imageFiles = Directory.GetFiles(imagesFolder, "*.jpg");
            if (imageFiles.Length == 0)
            {
                return "/images/default-image.jpg"; // Fallback if no images are found
            }
            Random random = new Random();
            int randomIndex = random.Next(0, imageFiles.Length);
            string filePath = imageFiles[randomIndex];
            string fileName = Path.GetFileName(filePath);
            return $"/images/{fileName}";
        }

        public IActionResult Contact()
        {
            string randomImage = GetRandomBackgroundImage();
            ViewBag.RandomBackgroundImage = randomImage;
            return View();
        }
        
        public IActionResult About()
        {
            string randomImage = GetRandomBackgroundImage();
            ViewBag.RandomBackgroundImage = randomImage;
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
