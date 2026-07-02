using Education.Application.Users;
using Education.Contracts.Courses;
using Education.Contracts.Modules;
using Education.Contracts.Theories;

namespace Education.Application.Courses;

public sealed class CoursesService(
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository,
    ITheoryDocumentStorage theoryDocumentStorage)
    : ICoursesService
{
    public async Task<IReadOnlyList<CourseResponse>> GetTeacherCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetTeacherCoursesAsync(legacyUserId, cancellationToken);
    }

    public async Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.CreateCourseAsync(legacyUserId, request, cancellationToken);
    }

    public async Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(courseId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(courseId);
        }

        await coursesRepository.DeleteCourseAsync(courseId, cancellationToken);
    }

    public async Task<IReadOnlyList<CourseResponse>> GetStudentCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetStudentCoursesAsync(legacyUserId, cancellationToken);
    }

    public async Task<IReadOnlyList<ModuleResponse>> GetModulesAsync(long courseId, CancellationToken cancellationToken = default)
    {
        return await coursesRepository.GetModulesAsync(courseId, cancellationToken);
    }

    public async Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(request.CourseId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(request.CourseId);
        }

        return await coursesRepository.CreateModuleAsync(request, cancellationToken);
    }

    public async Task DeleteModuleAsync(long moduleId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsModuleOwnerAsync(moduleId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(moduleId);
        }

        await coursesRepository.DeleteModuleAsync(moduleId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoryListItemResponse>> GetTheoriesAsync(long moduleId, CancellationToken cancellationToken = default)
    {
        return coursesRepository.GetTheoriesAsync(moduleId, cancellationToken);
    }

    public Task<TheoryTextResponse?> GetTheoryTextAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        return coursesRepository.GetTheoryTextAsync(theoryId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoryDocumentResponse>> GetTheoryDocsAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        return coursesRepository.GetTheoryDocsAsync(theoryId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoryLinkResponse>> GetTheoryLinksAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        return coursesRepository.GetTheoryLinksAsync(theoryId, cancellationToken);
    }

    public async Task<TheoryListItemResponse> CreateTheoryAsync(CreateTheoryRequest request, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsModuleOwnerAsync(request.ModuleId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(request.ModuleId);
        }

        return await coursesRepository.CreateTheoryAsync(request, cancellationToken);
    }

    public async Task<TheoryDocumentResponse> CreateTheoryDocumentAsync(
        long theoryMaterialId,
        string description,
        TheoryDocumentFile file,
        CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryMaterialId, cancellationToken);

        var path = await theoryDocumentStorage.SaveAsync(file, cancellationToken);
        return await coursesRepository.CreateTheoryDocumentAsync(
            theoryMaterialId,
            description,
            path,
            cancellationToken);
    }

    public async Task UpdateTheoryTitleAsync(long theoryId, UpdateTheoryTitleRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await coursesRepository.UpdateTheoryTitleAsync(theoryId, request.Title, cancellationToken);
    }

    public async Task UpdateTheoryTextAsync(long theoryId, UpdateTheoryTextRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await coursesRepository.UpdateTheoryTextAsync(theoryId, request.Text, cancellationToken);
    }

    public async Task DeleteTheoryAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await coursesRepository.DeleteTheoryAsync(theoryId, cancellationToken);
    }

    public async Task DeleteTheoryDocumentAsync(long documentId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsTheoryDocumentOwnerAsync(documentId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(documentId);
        }

        var path = await coursesRepository.GetTheoryDocumentPathAsync(documentId, cancellationToken);
        if (path is not null)
        {
            await theoryDocumentStorage.DeleteAsync(path, cancellationToken);
        }

        await coursesRepository.DeleteTheoryDocumentAsync(documentId, cancellationToken);
    }

    public async Task<TheoryLinkResponse> CreateTheoryLinkAsync(CreateTheoryLinkRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(request.TheoryMaterialId, cancellationToken);
        return await coursesRepository.CreateTheoryLinkAsync(request, cancellationToken);
    }

    public async Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsTheoryLinkOwnerAsync(linkId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(linkId);
        }

        await coursesRepository.DeleteTheoryLinkAsync(linkId, cancellationToken);
    }

    private async Task EnsureTheoryOwnerAsync(long theoryId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsTheoryOwnerAsync(theoryId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(theoryId);
        }
    }
}
