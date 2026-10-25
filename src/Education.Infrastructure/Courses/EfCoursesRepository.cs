using Education.Application.Courses;
using Education.Domain.Courses;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Courses;

/// <summary>EF Core-реализация хранилища курсов.</summary>
internal sealed class EfCoursesRepository : RepositoryBase<Course, Guid>, ICoursesRepository
{
    /// <summary>Создаёт экземпляр класса <see cref="EfCoursesRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfCoursesRepository(EducationDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Courses.FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(
        Guid teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Courses
            .AsNoTracking()
            .Where(course => course.UserId == teacherUserId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Course> CreateCourseAsync(
        Guid teacherUserId,
        CreateCourseCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = new Course(command.Name, command.Description, command.Date, teacherUserId);
        await DatabaseContext.Courses.AddAsync(course, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return course;
    }

    /// <inheritdoc />
    public async Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Courses
            .Where(course => course.Id == courseId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Courses
            .AsNoTracking()
            .Where(course => course.CourseBindUsers.Any(bind => bind.UserId == studentUserId))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> IsCourseOwnerAsync(Guid courseId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Courses.AnyAsync(
            course => course.Id == courseId && course.UserId == teacherUserId,
            cancellationToken);
    }
}


