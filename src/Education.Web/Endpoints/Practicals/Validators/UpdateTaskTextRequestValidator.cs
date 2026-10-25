using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdateTaskTextRequestValidator : AbstractValidator<UpdateTaskTextRequest>
{
    public UpdateTaskTextRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст задания обязателен.");
    }
}
