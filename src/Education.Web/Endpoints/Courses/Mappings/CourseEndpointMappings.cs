using Education.Application.Courses;
using Education.Contracts.Courses;
using Education.Domain.Courses;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «курсы» между контрактами API и слоем приложения.</summary>
internal static class CourseEndpointMappings
{
    /// <summary>Преобразует <see cref="CreateCourseRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreateCourseCommand ToCommand(this CreateCourseRequest request)
    {
        return new CreateCourseCommand(request.Date, request.Description, request.Name);
    }

    /// <summary>Преобразует <see cref="Course"/> в ответ API.</summary>
    /// <param name="course">Исходные данные.</param>
    public static CourseResponse ToResponse(this Course course)
    {
        return new CourseResponse(course.Id, course.Date, course.Description, course.Name);
    }
}

