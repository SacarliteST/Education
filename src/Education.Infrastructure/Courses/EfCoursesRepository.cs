using Education.Application.Courses;
using Education.Domain.Courses;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Courses;

internal sealed class EfCoursesRepository(EducationDbContext context)
    : RepositoryBase<Course, Guid>(context), ICoursesRepository
{
    public override Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Courses.FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(
        Guid teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Courses
            .AsNoTracking()
            .Where(course => course.UserId == teacherUserId)
            .ToListAsync(cancellationToken);
    }

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

    public async Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Courses
            .Where(course => course.Id == courseId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Courses
            .AsNoTracking()
            .Where(course => course.CourseBindUsers.Any(bind => bind.UserId == studentUserId))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsCourseOwnerAsync(Guid courseId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Courses.AnyAsync(
            course => course.Id == courseId && course.UserId == teacherUserId,
            cancellationToken);
    }
}


