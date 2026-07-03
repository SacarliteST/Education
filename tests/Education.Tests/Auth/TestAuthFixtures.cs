using Education.Web.Identity;

namespace Education.Tests.Auth;

internal static class TestAuthFixtures
{
    public static readonly string[] StudentRoles = [EducationRoles.Student];
    public static readonly string[] TeacherRoles = [EducationRoles.Teacher];
    public static readonly string[] AdminRoles = [EducationRoles.Admin];
}
