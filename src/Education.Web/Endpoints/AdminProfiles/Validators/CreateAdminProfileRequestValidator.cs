using Education.Contracts.AdminProfiles;
using Education.Domain.Users;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateAdminProfileRequest"/>.</summary>
internal sealed class CreateAdminProfileRequestValidator : AbstractValidator<CreateAdminProfileRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateAdminProfileRequestValidator()
    {
        RuleFor(request => request.UserId)
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

        RuleFor(request => request.Group)
            .MaximumLength(User.GroupNameMaxLength)
            .WithMessage($"Название группы не должно превышать {User.GroupNameMaxLength} символов.");
    }
}
