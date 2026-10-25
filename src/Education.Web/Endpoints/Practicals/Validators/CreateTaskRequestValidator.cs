using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название задания обязательно.")
            .MaximumLength(200)
            .WithMessage("Название задания не должно превышать 200 символов.");
    }
}
