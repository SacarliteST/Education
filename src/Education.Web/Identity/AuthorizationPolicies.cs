namespace Education.Web.Identity;

public static class AuthorizationPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string TeacherOnly = nameof(TeacherOnly);
    public const string StudentOnly = nameof(StudentOnly);
    public const string AuthenticatedEducationUser = nameof(AuthenticatedEducationUser);
}
