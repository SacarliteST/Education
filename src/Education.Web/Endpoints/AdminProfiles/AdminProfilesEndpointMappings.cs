using Education.Application.AdminProfiles;
using Education.Contracts.AdminProfiles;
using Education.Contracts.Courses;
using Education.Contracts.Practicals;
using Education.Domain.Users;

namespace Education.Web.Endpoints;

internal static class AdminProfilesEndpointMappings
{
    public static SetCourseStudentsCommand ToCommand(this UpdateCourseStudentsRequest request, Guid courseId)
    {
        return new SetCourseStudentsCommand(courseId, request.UserIds ?? []);
    }

    public static SetPracticalStudentsCommand ToCommand(this UpdatePracticalStudentsRequest request, Guid practicalId)
    {
        return new SetPracticalStudentsCommand(practicalId, request.UserIds ?? []);
    }

    public static CreateAdminProfileCommand ToCommand(this CreateAdminProfileRequest request)
    {
        return new CreateAdminProfileCommand(
            request.IdentityUserId,
            request.Login,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            ResolveRoleId(request.Role));
    }

    private static Guid ResolveRoleId(ProfileRole role)
    {
        return role switch
        {
            ProfileRole.Admin => RoleIds.Admin,
            ProfileRole.Teacher => RoleIds.Teacher,
            _ => RoleIds.Student,
        };
    }

    public static UpdateAdminProfileCommand ToCommand(this UpdateAdminProfileRequest request)
    {
        return new UpdateAdminProfileCommand(
            request.Login,
            request.FirstName,
            request.LastName,
            request.MiddleName);
    }

    public static AdminProfileResponse ToResponse(this AdminProfile profile)
    {
        return new AdminProfileResponse(
            profile.LegacyUserId,
            profile.IdentityUserId,
            profile.Login,
            profile.FirstName,
            profile.LastName,
            profile.MiddleName,
            profile.IsActive);
    }

    public static AssignableStudentResponse ToResponse(this AssignableStudent student)
    {
        return new AssignableStudentResponse(student.LegacyUserId, student.FullName, student.IsAssigned);
    }

    public static StudentAssignmentPageResponse ToResponse(this AssignableStudentsPage page, int pageNumber, int pageSize)
    {
        return new StudentAssignmentPageResponse(
            page.Items
                .Select(item => new StudentAssignmentEntryResponse(
                    item.LegacyUserId, item.FullName.Trim(), item.Login, item.IsAssigned))
                .ToArray(),
            page.TotalCount,
            page.AssignedCount,
            pageNumber,
            pageSize);
    }

    public static ChangeCourseStudentsCommand ToCommand(this ChangeStudentsRequest request, Guid courseId)
    {
        return new ChangeCourseStudentsCommand(courseId, request.Add, request.Remove);
    }

    public static ChangePracticalStudentsCommand ToPracticalCommand(this ChangeStudentsRequest request, Guid practicalId)
    {
        return new ChangePracticalStudentsCommand(practicalId, request.Add, request.Remove);
    }
}

