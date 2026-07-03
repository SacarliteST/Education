using Education.Contracts.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class SubmitTestRequestValidator : AbstractValidator<SubmitTestRequest>
{
    public SubmitTestRequestValidator()
    {
        RuleFor(request => request.Answers)
            .NotNull()
            .NotEmpty();

        RuleForEach(request => request.Answers)
            .SetValidator(new SubmitAnswerRequestValidator());
    }
}

internal sealed class SubmitAnswerRequestValidator : AbstractValidator<SubmitAnswerRequest>
{
    public SubmitAnswerRequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty();

        RuleFor(request => request.Answer)
            .NotNull();
    }
}

