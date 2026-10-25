using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateTaskRequest"/>.</summary>
internal sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateTaskRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название задания обязательно.")
            .MaximumLength(200)
            .WithMessage("Название задания не должно превышать 200 символов.");
    }
}
