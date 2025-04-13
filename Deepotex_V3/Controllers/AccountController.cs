using Deepotex.core.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Deepotex_V2.Controllers;

public class AccountController : Controller
{
    private readonly string _validEmail;
    private readonly string _hashedPassword;

    public AccountController(IConfiguration configuration)
    {
        _validEmail = configuration["Admin:Email"] ?? throw new ArgumentNullException("Admin:Email is missing in configuration.");
        _hashedPassword = configuration["Admin:Password"] ?? throw new ArgumentNullException("Admin:Password is missing in configuration.");
    }

    [HttpGet]
    public IActionResult Login() 
    {
        return View("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Email == _validEmail && BCrypt.Net.BCrypt.Verify(model.Password, _hashedPassword))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Email, model.Email)
                };
                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password.");
            }
        }
        return View(model);
    }
}