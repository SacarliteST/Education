using Education.Application.Grades;
using Education.Contracts.Grades;

namespace Education.Web.Endpoints;

internal static class GradeEndpointMappings
{
    public static PracticalGradeResponse ToResponse(this PracticalGrade grade)
    {
        return new PracticalGradeResponse(grade.Grade, grade.Messages);
    }
}

