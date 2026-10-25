using Education.Contracts.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="SubmitAnswerRequest"/>.</summary>
internal sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public SubmitAnswerRequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .WithMessage("Идентификатор вопроса обязателен.");

        RuleFor(request => request.Answer)
            .NotNull()
            .WithMessage("Ответ обязателен.");
    }
}
