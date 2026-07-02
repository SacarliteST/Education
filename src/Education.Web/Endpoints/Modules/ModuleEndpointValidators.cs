using Education.Contracts.Modules;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateModuleRequestValidator : AbstractValidator<CreateModuleRequest>
{
    public CreateModuleRequestValidator()
    {
        RuleFor(request => request.CourseId)
            .GreaterThan(0);

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
