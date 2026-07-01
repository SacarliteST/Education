using Education.Consts;
using Education.DAL;
using Education.DAL.Models;
using Education.Helpers;
using Education.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Education.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = RoleConstants.Admin)]
public class UserController(ApplicationContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] AddUserRequest res)
    {
        if (await context.Users.AnyAsync(u => u.Login == res.Login))
        {
            return BadRequest();
        }

        User newUser = new User()
        {
            FirstName = res.FirstName,
            LastName = res.LastName,
            MiddleName = res.MiddleName,
            Login = res.Login,
            Password = HashHelper.GetSha256Hash(res.Password),
            RoleId = res.RoleId,
        };
        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();
        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> CanDeleteUser([FromQuery] long id)
    {
        var user = await context.Users
            .Include(u => u.Courses)
            .FirstOrDefaultAsync(u => u.Id == id);
        bool canDelete = user?.Courses.Any() ?? false;
        return Ok(new { CanDelete = canDelete });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await context.Users
            .Include(u => u.Courses)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null || user.Courses.Count != 0)
        {
            return BadRequest();
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await context.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await context.Users
            .Include(u => u.Role)
            .Select(u => new
            {
                u.Id,
                u.Login,
                u.FirstName,
                u.LastName,
                u.MiddleName,
                Role = u.Role.Name
            }).ToListAsync();
        return Ok(users);
    }
}
