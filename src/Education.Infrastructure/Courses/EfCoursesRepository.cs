using Education.Application.Courses;
using Education.Domain.Courses;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Courses;

internal sealed class EfCoursesRepository(EducationDbContext context)
    : RepositoryBase<Course, long>(context), ICoursesRepository
{
    public override Task<Course?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Courses.FirstOrDefaultAsync(course => course.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Courses
            .AsNoTracking()
            .Where(course => course.UserId == teacherUserId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course> CreateCourseAsync(
        long teacherUserId,
        CreateCourseCommand command,
        CancellationToken cancellationToken = default)
    {
        var course = new Course(command.Name, command.Description, command.Date, teacherUserId);
        await DatabaseContext.Courses.AddAsync(course, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return course;
    }

    public async Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Courses
            .Where(course => course.Id == courseId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Courses
            .AsNoTracking()
            .Where(course => course.CourseBindUsers.Any(bind => bind.UserId == studentUserId))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsCourseOwnerAsync(long courseId, long teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Courses.AnyAsync(
            course => course.Id == courseId && course.UserId == teacherUserId,
            cancellationToken);
    }
}
