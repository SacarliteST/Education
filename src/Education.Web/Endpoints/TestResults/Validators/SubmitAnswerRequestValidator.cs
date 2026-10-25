using Education.Contracts.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
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
