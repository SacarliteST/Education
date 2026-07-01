using System.Security.Claims;
using Education.DAL;
using Education.Extensions;
using Education.Helpers;
using Education.Models.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Education.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(ApplicationContext context) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromQuery] string? returnUrl, [FromBody] AuthRequest req)
    {
        var hash = HashHelper.GetSha256Hash(req.Password);
        var user = context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Login == req.Login && u.Password == hash);
        if (user is null)
        {
            return Unauthorized();
        }

        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, user.Login),
            new (ClaimTypes.Role, user.Role.Name)
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Ok(new { Role = user.Role.Name, Username = user.GetFullName() });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult IsSignedIn()
    {
        return Ok(new { IsSignedIn = HttpContext?.User?.Identity?.IsAuthenticated });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }
}
