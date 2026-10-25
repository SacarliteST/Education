using Education.Contracts.AdminProfiles;
using Education.Domain.Users;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdateAdminProfileRequestValidator : AbstractValidator<UpdateAdminProfileRequest>
{
    public UpdateAdminProfileRequestValidator()
    {
        RuleFor(request => request.Group)
            .MaximumLength(User.GroupNameMaxLength)
            .WithMessage($"Название группы не должно превышать {User.GroupNameMaxLength} символов.");
    }
}
