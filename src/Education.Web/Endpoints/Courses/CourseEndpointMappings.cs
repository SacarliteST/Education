using Education.Application.Courses;
using Education.Contracts.Courses;
using Education.Domain.Courses;

namespace Education.Web.Endpoints;

internal static class CourseEndpointMappings
{
    public static CreateCourseCommand ToCommand(this CreateCourseRequest request)
    {
        return new CreateCourseCommand(request.Date, request.Description, request.Name);
    }

    public static CourseResponse ToResponse(this Course course)
    {
        return new CourseResponse(course.Id, course.Date, course.Description, course.Name);
    }
}
