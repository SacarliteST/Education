using Education.Application.Courses;
using Education.Contracts.Courses;
using Education.Contracts.Modules;
using Education.Contracts.Theories;
using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Courses;

internal sealed class EfCoursesRepository(EducationDbContext context) : ICoursesRepository
{
    public async Task<IReadOnlyList<CourseResponse>> GetTeacherCoursesAsync(
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return await context.Courses
            .AsNoTracking()
            .Where(course => course.UserId == teacherUserId)
            .Select(course => new CourseResponse(course.Id, course.Date, course.Description, course.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<CourseResponse> CreateCourseAsync(
        long teacherUserId,
        CreateCourseRequest request,
        CancellationToken cancellationToken = default)
    {
        var course = new Course(request.Name, request.Description, request.Date, teacherUserId);
        await context.Courses.AddAsync(course, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new CourseResponse(course.Id, course.Date, course.Description, course.Name);
    }

    public async Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default)
    {
        await context.Courses
            .Where(course => course.Id == courseId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourseResponse>> GetStudentCoursesAsync(
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
        return await context.Courses
            .AsNoTracking()
            .Where(course => course.CourseBindUsers.Any(bind => bind.UserId == studentUserId))
            .Select(course => new CourseResponse(course.Id, course.Date, course.Description, course.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsCourseOwnerAsync(long courseId, long teacherUserId, CancellationToken cancellationToken = default)
    {
        return context.Courses.AnyAsync(
            course => course.Id == courseId && course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsModuleOwnerAsync(long moduleId, long teacherUserId, CancellationToken cancellationToken = default)
    {
        return context.Modules.AnyAsync(
            module => module.Id == moduleId && module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsTheoryOwnerAsync(long theoryId, long teacherUserId, CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterials.AnyAsync(
            theory => theory.Id == theoryId && theory.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsTheoryLinkOwnerAsync(long linkId, long teacherUserId, CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialLinks.AnyAsync(
            link => link.Id == linkId && link.TheoreticalMaterial.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsTheoryDocumentOwnerAsync(long documentId, long teacherUserId, CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialFiles.AnyAsync(
            file => file.Id == documentId && file.TheoreticalMaterial.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ModuleResponse>> GetModulesAsync(
        long courseId,
        CancellationToken cancellationToken = default)
    {
        return await context.Modules
            .AsNoTracking()
            .Where(module => module.CourseId == courseId)
            .Select(module => new ModuleResponse(module.Id, module.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<ModuleResponse> CreateModuleAsync(
        CreateModuleRequest request,
        CancellationToken cancellationToken = default)
    {
        var module = new Module(request.CourseId, request.Name);
        await context.Modules.AddAsync(module, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new ModuleResponse(module.Id, module.Name);
    }

    public async Task DeleteModuleAsync(long moduleId, CancellationToken cancellationToken = default)
    {
        await context.Modules
            .Where(module => module.Id == moduleId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TheoryListItemResponse>> GetTheoriesAsync(
        long moduleId,
        CancellationToken cancellationToken = default)
    {
        return await context.TheoreticalMaterials
            .AsNoTracking()
            .Where(theory => theory.ModuleId == moduleId)
            .Select(theory => new TheoryListItemResponse(theory.Id, theory.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<TheoryTextResponse?> GetTheoryTextAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterials
            .AsNoTracking()
            .Where(theory => theory.Id == theoryId)
            .Select(theory => new TheoryTextResponse(theory.Text, theory.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TheoryDocumentResponse>> GetTheoryDocsAsync(
        long theoryId,
        CancellationToken cancellationToken = default)
    {
        var files = await context.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.TheoreticalMaterialId == theoryId)
            .Select(file => new { file.Id, file.Path, file.Description })
            .ToListAsync(cancellationToken);

        return files
            .Select(file => new TheoryDocumentResponse(
                file.Id,
                file.Path,
                file.Description,
                GetPublicFileName(file.Path)))
            .ToList();
    }

    public async Task<IReadOnlyList<TheoryLinkResponse>> GetTheoryLinksAsync(
        long theoryId,
        CancellationToken cancellationToken = default)
    {
        return await context.TheoreticalMaterialLinks
            .AsNoTracking()
            .Where(link => link.TheoreticalMaterialId == theoryId)
            .Select(link => new TheoryLinkResponse(link.Id, link.Link, link.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<TheoryListItemResponse> CreateTheoryAsync(
        CreateTheoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var theory = new TheoreticalMaterial(request.ModuleId, request.Name, "Текст лекции");
        await context.TheoreticalMaterials.AddAsync(theory, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new TheoryListItemResponse(theory.Id, theory.Name);
    }

    public async Task<TheoryDocumentResponse> CreateTheoryDocumentAsync(
        long theoryMaterialId,
        string description,
        string path,
        CancellationToken cancellationToken = default)
    {
        var file = new TheoreticalMaterialFile(theoryMaterialId, description, path);
        await context.TheoreticalMaterialFiles.AddAsync(file, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new TheoryDocumentResponse(file.Id, file.Path, file.Description, GetPublicFileName(file.Path));
    }

    public async Task UpdateTheoryTitleAsync(
        long theoryId,
        string title,
        CancellationToken cancellationToken = default)
    {
        var theory = await context.TheoreticalMaterials.FirstOrDefaultAsync(
            item => item.Id == theoryId,
            cancellationToken);

        if (theory is null)
        {
            return;
        }

        theory.Rename(title);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTheoryTextAsync(
        long theoryId,
        string text,
        CancellationToken cancellationToken = default)
    {
        var theory = await context.TheoreticalMaterials.FirstOrDefaultAsync(
            item => item.Id == theoryId,
            cancellationToken);

        if (theory is null)
        {
            return;
        }

        theory.UpdateText(text);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTheoryAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        await context.TheoreticalMaterials
            .Where(theory => theory.Id == theoryId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<string?> GetTheoryDocumentPathAsync(long documentId, CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.Id == documentId)
            .Select(file => file.Path)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeleteTheoryDocumentAsync(long documentId, CancellationToken cancellationToken = default)
    {
        await context.TheoreticalMaterialFiles
            .Where(file => file.Id == documentId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<TheoryLinkResponse> CreateTheoryLinkAsync(
        CreateTheoryLinkRequest request,
        CancellationToken cancellationToken = default)
    {
        var link = new TheoreticalMaterialLink(
            request.TheoryMaterialId,
            request.Description,
            request.Link);

        await context.TheoreticalMaterialLinks.AddAsync(link, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new TheoryLinkResponse(link.Id, link.Link, link.Description);
    }

    public async Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        await context.TheoreticalMaterialLinks
            .Where(link => link.Id == linkId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private static string GetPublicFileName(string path)
    {
        var name = Path.GetFileName(path);
        var lastUnderscore = name.LastIndexOf('_');

        return lastUnderscore <= 0 ? name : name[..lastUnderscore] + Path.GetExtension(name);
    }
}
