using Education.Contracts.Courses;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(request => request.Date)
            .NotEmpty();
    }
}
