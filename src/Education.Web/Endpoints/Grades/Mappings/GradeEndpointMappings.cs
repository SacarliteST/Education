using Education.Application.Grades;
using Education.Contracts.Grades;

namespace Education.Web.Endpoints;

internal static class GradeEndpointMappings
{
    public static PracticalGradeResponse ToResponse(this PracticalGrade grade)
    {
        return new PracticalGradeResponse(grade.Grade, grade.Messages);
    }

    public static string ToWire(this GradeStatus status) => status switch
    {
        GradeStatus.Passed => "passed",
        GradeStatus.Failed => "failed",
        _ => "incomplete",
    };

    public static PracticalStudentGradeResponse ToResponse(this StudentPracticalGrade row)
    {
        return new PracticalStudentGradeResponse(
            row.StudentId,
            row.StudentName,
            row.Login,
            row.Group,
            row.Grade.Grade,
            row.Status.ToWire(),
            row.Grade.Messages);
    }

    public static CourseGradebookResponse ToResponse(this Gradebook gradebook)
    {
        return new CourseGradebookResponse(
            gradebook.Practicals
                .Select(practical => new GradebookPracticalResponse(
                    practical.Id, practical.Name, practical.IsExternal, practical.ModuleId, practical.ModuleName))
                .ToList(),
            gradebook.Students
                .Select(student => new GradebookStudentResponse(
                    student.StudentId,
                    student.StudentName,
                    student.Login,
                    student.Group,
                    student.Cells
                        .Select(cell => new GradebookCellResponse(
                            cell.PracticalId, cell.Assigned, cell.Grade?.Grade, cell.Status.ToWire()))
                        .ToList(),
                    student.AssignedCount,
                    student.PassedCount,
                    student.AverageGrade))
                .ToList());
    }
}

