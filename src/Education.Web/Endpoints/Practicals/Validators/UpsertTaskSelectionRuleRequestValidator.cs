using Education.Contracts.Practicals;
using Education.Domain.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Валидатор запроса создания/замены правила случайной выборки заданий.</summary>
internal sealed class UpsertTaskSelectionRuleRequestValidator : AbstractValidator<UpsertTaskSelectionRuleRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public UpsertTaskSelectionRuleRequestValidator()
    {
        RuleFor(request => request.PracticalModuleId)
            .NotEmpty()
            .WithMessage("Идентификатор внешнего модуля обязателен.");

        RuleFor(request => request.TaskCount)
            .InclusiveBetween(TaskSelectionRule.MinTaskCount, TaskSelectionRule.MaxTaskCount)
            .WithMessage($"Количество заданий должно быть от {TaskSelectionRule.MinTaskCount} до {TaskSelectionRule.MaxTaskCount}.");

        RuleFor(request => request.DifficultyLevels)
            .Must(levels => levels is null || levels.Count == levels.Distinct().Count())
            .WithMessage("Уровни сложности не должны повторяться.");

        RuleFor(request => request.TopicRefs)
            .Must(refs => refs is null || refs.All(topicRef => !String.IsNullOrWhiteSpace(topicRef)))
            .WithMessage("Идентификаторы тем не должны быть пустыми.");
    }
}
