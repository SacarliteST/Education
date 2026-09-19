using Education.Contracts.AdminProfiles;
using Education.Contracts.Courses;
using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdateCourseStudentsRequestValidator : AbstractValidator<UpdateCourseStudentsRequest>
{
    public UpdateCourseStudentsRequestValidator()
    {
        RuleFor(request => request.UserIds)
            .NotNull()
            .WithMessage("Список студентов обязателен.");

        RuleForEach(request => request.UserIds)
            .NotEmpty()
            .WithMessage("Идентификатор студента не может быть пустым.");
    }
}

internal sealed class UpdatePracticalStudentsRequestValidator : AbstractValidator<UpdatePracticalStudentsRequest>
{
    public UpdatePracticalStudentsRequestValidator()
    {
        RuleFor(request => request.UserIds)
            .NotNull()
            .WithMessage("Список студентов обязателен.");

        RuleForEach(request => request.UserIds)
            .NotEmpty()
            .WithMessage("Идентификатор студента не может быть пустым.");
    }
}

/// <summary>
/// Проверяет точечное изменение набора студентов: списки не пусты одновременно, ограничены по размеру
/// и не пересекаются (иначе результат зависел бы от порядка применения).
/// </summary>
internal sealed class ChangeStudentsRequestValidator : AbstractValidator<ChangeStudentsRequest>
{
    /// <summary>Максимальное число идентификаторов в каждом из списков одного запроса.</summary>
    public const int MaxIdsPerList = 2000;

    public ChangeStudentsRequestValidator()
    {
        RuleFor(request => request.Add)
            .NotNull().WithMessage("Список добавляемых студентов обязателен.")
            .Must(ids => ids is null || ids.Count <= MaxIdsPerList)
            .WithMessage($"За один запрос можно добавить не более {MaxIdsPerList} студентов.");
        RuleFor(request => request.Remove)
            .NotNull().WithMessage("Список снимаемых студентов обязателен.")
            .Must(ids => ids is null || ids.Count <= MaxIdsPerList)
            .WithMessage($"За один запрос можно снять не более {MaxIdsPerList} студентов.");

        RuleForEach(request => request.Add)
            .NotEmpty().WithMessage("Идентификатор студента не может быть пустым.");
        RuleForEach(request => request.Remove)
            .NotEmpty().WithMessage("Идентификатор студента не может быть пустым.");

        RuleFor(request => request)
            .Must(request => request.Add is null || request.Remove is null || !request.Add.Intersect(request.Remove).Any())
            .WithName("Add")
            .WithMessage("Один и тот же студент не может быть одновременно в списках добавления и снятия.");
    }
}

internal sealed class CreateAdminProfileRequestValidator : AbstractValidator<CreateAdminProfileRequest>
{
    public CreateAdminProfileRequestValidator()
    {
        RuleFor(request => request.IdentityUserId)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя Identity обязателен.");

        RuleFor(request => request.Login)
            .NotEmpty()
            .WithMessage("Логин обязателен.")
            .MaximumLength(256)
            .WithMessage("Логин не должен превышать 256 символов.");

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .WithMessage("Имя обязательно.")
            .MaximumLength(256)
            .WithMessage("Имя не должно превышать 256 символов.");

        RuleFor(request => request.LastName)
            .NotEmpty()
            .WithMessage("Фамилия обязательна.")
            .MaximumLength(256)
            .WithMessage("Фамилия не должна превышать 256 символов.");

        RuleFor(request => request.MiddleName)
            .MaximumLength(256)
            .WithMessage("Отчество не должно превышать 256 символов.");

        RuleFor(request => request.Role)
            .IsInEnum()
            .WithMessage("Недопустимая роль профиля.");
    }
}

internal sealed class UpdateAdminProfileRequestValidator : AbstractValidator<UpdateAdminProfileRequest>
{
    public UpdateAdminProfileRequestValidator()
    {
        RuleFor(request => request.Login)
            .NotEmpty()
            .WithMessage("Логин обязателен.")
            .MaximumLength(256)
            .WithMessage("Логин не должен превышать 256 символов.");

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .WithMessage("Имя обязательно.")
            .MaximumLength(256)
            .WithMessage("Имя не должно превышать 256 символов.");

        RuleFor(request => request.LastName)
            .NotEmpty()
            .WithMessage("Фамилия обязательна.")
            .MaximumLength(256)
            .WithMessage("Фамилия не должна превышать 256 символов.");

        RuleFor(request => request.MiddleName)
            .MaximumLength(256)
            .WithMessage("Отчество не должно превышать 256 символов.");
    }
}

