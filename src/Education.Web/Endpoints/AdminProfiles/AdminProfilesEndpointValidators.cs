using Education.Contracts.AdminProfiles;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateAdminProfileRequestValidator : AbstractValidator<CreateAdminProfileRequest>
{
    public CreateAdminProfileRequestValidator()
    {
        RuleFor(request => request.IdentityUserId)
            .NotEmpty();

        RuleFor(request => request.Login)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.MiddleName)
            .MaximumLength(256);
    }
}

internal sealed class UpdateAdminProfileRequestValidator : AbstractValidator<UpdateAdminProfileRequest>
{
    public UpdateAdminProfileRequestValidator()
    {
        RuleFor(request => request.Login)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.FirstName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.LastName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(request => request.MiddleName)
            .MaximumLength(256);
    }
}

