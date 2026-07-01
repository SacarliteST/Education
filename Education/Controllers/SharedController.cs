using Education.Consts;
using Education.DAL;
using Education.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Education.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = RoleConstants.All)]
public class SharedController(ApplicationContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetModules(long courseId)
    {
        var result = await context.Modules
            .AsNoTracking()
            .Where(m => m.CourseId == courseId)
            .Select(m => new { m.Id, m.Name })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTheoryText(long theoryId)
    {
        var theory = await context.TheoreticalMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == theoryId);
        if (theory is null)
        {
            return BadRequest();
        }

        return Ok(new { theory.Text, theory.Name });
    }

    [HttpGet]
    public async Task<IActionResult> GetTheoryDocs(long theoryId)
    {
        var result = await context.TheoreticalMaterialFiles
        .AsNoTracking()
            .Where(m => m.TheoreticalMaterialId == theoryId)
            .Select(m => new { m.Id, m.Path, m.Description, Name = m.Path.GetPublicFileName() })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTheoryLinks(long theoryId)
    {
        var result = await context.TheoreticalMaterialLinks
        .AsNoTracking()
            .Where(m => m.TheoreticalMaterialId == theoryId)
            .Select(m => new { m.Id, m.Link, m.Description })
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskText(long taskId)
    {
        var task = await context.Cases
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == taskId);
        if (task is null)
        {
            return BadRequest();
        }

        return Ok(new { task.Text });
    }
}
