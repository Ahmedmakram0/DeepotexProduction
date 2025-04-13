using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System;

namespace Deepotex_V2.Controllers
{
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true
                }
            );

            // Set the language in session for immediate use
            HttpContext.Session.SetString("CurrentCulture", culture);

            // Set the language direction in session
            HttpContext.Session.SetString("IsRTL", culture == "ar" ? "true" : "false");

            // Force a page reload to apply the new language
            return LocalRedirect(returnUrl);
        }
    }
}
