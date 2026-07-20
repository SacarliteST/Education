using Education.Contracts.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class SubmitTestRequestValidator : AbstractValidator<SubmitTestRequest>
{
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

