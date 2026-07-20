using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreatePracticalRequestValidator : AbstractValidator<CreatePracticalRequest>
{
    public CreatePracticalRequestValidator()
    {
        RuleFor(request => request.ModuleId)
            .NotEmpty()
            .WithMessage("Идентификатор модуля обязателен.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название практического материала обязательно.")
            .MaximumLength(200)
            .WithMessage("Название практического материала не должно превышать 200 символов.");
    }
}

internal sealed class ConfigurePracticalQuestionsRequestValidator : AbstractValidator<ConfigurePracticalQuestionsRequest>
{
    public ConfigurePracticalQuestionsRequestValidator()
    {
        RuleFor(request => request.QuestionIds)
            .NotNull()
            .WithMessage("Список вопросов обязателен.");

        RuleFor(request => request.TriesCount)
            .GreaterThan(0)
            .WithMessage("Количество попыток должно быть больше 0.");

        RuleFor(request => request.PercentForFive)
            .InclusiveBetween(0, 100)
            .WithMessage("Процент для оценки 5 должен быть от 0 до 100.");

        RuleFor(request => request.PercentForFour)
            .InclusiveBetween(0, 100)
            .WithMessage("Процент для оценки 4 должен быть от 0 до 100.");

        RuleFor(request => request.PercentForThree)
            .InclusiveBetween(0, 100)
            .WithMessage("Процент для оценки 3 должен быть от 0 до 100.");

        RuleFor(request => request)
            .Must(request => request.PercentForFive >= request.PercentForFour)
            .WithMessage("Процент для оценки 5 должен быть больше или равен проценту для оценки 4.");

        RuleFor(request => request)
            .Must(request => request.PercentForFour >= request.PercentForThree)
            .WithMessage("Процент для оценки 4 должен быть больше или равен проценту для оценки 3.");
    }
}

