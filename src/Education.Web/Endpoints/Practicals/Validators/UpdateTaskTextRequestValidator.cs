using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdateTaskTextRequest"/>.</summary>
internal sealed class UpdateTaskTextRequestValidator : AbstractValidator<UpdateTaskTextRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public UpdateTaskTextRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст задания обязателен.");
    }
}
