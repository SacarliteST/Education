using Education.Application.AdminProfiles;
using Education.Contracts.AdminProfiles;
using Education.Contracts.Courses;
using Education.Contracts.Practicals;
using Education.Domain.Users;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «администрирование профилей» между контрактами API и слоем приложения.</summary>
internal static class AdminProfilesEndpointMappings
{
    /// <summary>Преобразует <see cref="UpdateCourseStudentsRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="courseId">Идентификатор курса.</param>
    public static SetCourseStudentsCommand ToCommand(this UpdateCourseStudentsRequest request, Guid courseId)
    {
        return new SetCourseStudentsCommand(courseId, request.UserIds ?? []);
    }

    /// <summary>Преобразует <see cref="UpdatePracticalStudentsRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="practicalId">Идентификатор практики.</param>
    public static SetPracticalStudentsCommand ToCommand(this UpdatePracticalStudentsRequest request, Guid practicalId)
    {
        return new SetPracticalStudentsCommand(practicalId, request.UserIds ?? []);
    }

    /// <summary>Преобразует <see cref="CreateAdminProfileRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreateAdminProfileCommand ToCommand(this CreateAdminProfileRequest request)
    {
        return new CreateAdminProfileCommand(
            request.UserId,
            request.Login,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            ResolveRoleId(request.Role),
            request.Group);
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

    /// <summary>Преобразует <see cref="UpdateAdminProfileRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static UpdateAdminProfileCommand ToCommand(this UpdateAdminProfileRequest request)
    {
        return new UpdateAdminProfileCommand(request.Group);
    }

    /// <summary>Преобразует <see cref="AdminProfile"/> в ответ API.</summary>
    /// <param name="profile">Исходные данные.</param>
    public static AdminProfileResponse ToResponse(this AdminProfile profile)
    {
        return new AdminProfileResponse(
            profile.LegacyUserId,
            profile.Login,
            profile.FirstName,
            profile.LastName,
            profile.MiddleName,
            profile.IsActive,
            profile.GroupName);
    }

    /// <summary>Преобразует <see cref="AssignableStudent"/> в ответ API.</summary>
    /// <param name="student">Исходные данные.</param>
    public static AssignableStudentResponse ToResponse(this AssignableStudent student)
    {
        return new AssignableStudentResponse(student.LegacyUserId, student.FullName, student.IsAssigned);
    }

    /// <summary>Преобразует <see cref="AssignableStudentsPage"/> в ответ API.</summary>
    /// <param name="page">Исходные данные.</param>
    /// <param name="pageNumber">Номер страницы (с 1).</param>
    /// <param name="pageSize">Размер страницы.</param>
    public static StudentAssignmentPageResponse ToResponse(this AssignableStudentsPage page, int pageNumber, int pageSize)
    {
        return new StudentAssignmentPageResponse(
            page.Items
                .Select(item => new StudentAssignmentEntryResponse(
                    item.LegacyUserId, item.FullName.Trim(), item.Login, item.IsAssigned, item.GroupName))
                .ToArray(),
            page.TotalCount,
            page.AssignedCount,
            pageNumber,
            pageSize);
    }

    /// <summary>Преобразует <see cref="ChangeStudentsRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="courseId">Идентификатор курса.</param>
    public static ChangeCourseStudentsCommand ToCommand(this ChangeStudentsRequest request, Guid courseId)
    {
        return new ChangeCourseStudentsCommand(courseId, request.Add, request.Remove);
    }

    /// <summary>Преобразует <see cref="ChangeStudentsRequest"/> в команду изменения студентов практики.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="practicalId">Идентификатор практики.</param>
    public static ChangePracticalStudentsCommand ToPracticalCommand(this ChangeStudentsRequest request, Guid practicalId)
    {
        return new ChangePracticalStudentsCommand(practicalId, request.Add, request.Remove);
    }
}

