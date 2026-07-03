using Education.Application.AdminProfiles;
using Education.Contracts.AdminProfiles;

namespace Education.Web.Endpoints;

internal static class AdminProfilesEndpointMappings
{
    public static CreateAdminProfileCommand ToCommand(this CreateAdminProfileRequest request)
    {
        return new CreateAdminProfileCommand(
            request.IdentityUserId,
            request.Login,
            request.FirstName,
            request.LastName,
            request.MiddleName);
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
}

