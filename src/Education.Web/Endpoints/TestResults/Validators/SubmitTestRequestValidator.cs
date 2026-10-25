using Education.Contracts.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="SubmitTestRequest"/>.</summary>
internal sealed class SubmitTestRequestValidator : AbstractValidator<SubmitTestRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public SubmitTestRequestValidator()
    {
        RuleFor(request => request.Answers)
            .NotNull()
            .WithMessage("Список ответов обязателен.")
            .NotEmpty()
            .WithMessage("Список ответов не должен быть пустым.");

        RuleForEach(request => request.Answers)
            .SetValidator(new SubmitAnswerRequestValidator());
    }
}
