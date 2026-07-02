using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreatePracticalRequestValidator : AbstractValidator<CreatePracticalRequest>
{
    public CreatePracticalRequestValidator()
    {
        RuleFor(request => request.ModuleId)
            .GreaterThan(0);

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}

internal sealed class ConfigurePracticalQuestionsRequestValidator : AbstractValidator<ConfigurePracticalQuestionsRequest>
{
    public ConfigurePracticalQuestionsRequestValidator()
    {
        RuleFor(request => request.QuestionIds)
            .NotNull();

        RuleFor(request => request.TriesCount)
            .GreaterThan(0);

        RuleFor(request => request.PercentForFive)
            .InclusiveBetween(0, 100);

        RuleFor(request => request.PercentForFour)
            .InclusiveBetween(0, 100);

        RuleFor(request => request.PercentForThree)
            .InclusiveBetween(0, 100);
    }
}
