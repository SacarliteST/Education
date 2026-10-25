using Education.Contracts.AdminProfiles;
using Education.Domain.Users;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdateAdminProfileRequest"/>.</summary>
internal sealed class UpdateAdminProfileRequestValidator : AbstractValidator<UpdateAdminProfileRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public UpdateAdminProfileRequestValidator()
    {
        RuleFor(request => request.Group)
            .MaximumLength(User.GroupNameMaxLength)
            .WithMessage($"Название группы не должно превышать {User.GroupNameMaxLength} символов.");
    }
}
