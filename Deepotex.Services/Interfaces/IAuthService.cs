using Deepotex.core.Models;
using Microsoft.AspNetCore.Identity;

namespace Deepotex.Services.Interfaces
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<IdentityResult> RegisterUserAsync(string email, string password, string fullName);
    }
}
