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

