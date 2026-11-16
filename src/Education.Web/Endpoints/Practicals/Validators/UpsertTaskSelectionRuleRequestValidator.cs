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

        RuleFor(request => request.DifficultyQuotas)
            .Must(quotas => quotas is null || quotas.Select(quota => quota.DifficultyLevel).Distinct().Count() == quotas.Count)
            .WithMessage("Для одного уровня сложности можно задать только одну квоту.")
            .Must(quotas => quotas is null || quotas.All(quota => quota.DifficultyLevel is >= 1 and <= 5))
            .WithMessage("Уровень сложности должен быть от 1 до 5.")
            .Must(quotas => quotas is null || quotas.All(quota => quota.TaskCount is >= 1 and <= TaskSelectionRule.MaxTaskCount))
            .WithMessage($"Количество заданий одной сложности должно быть от 1 до {TaskSelectionRule.MaxTaskCount}.");

        RuleFor(request => request)
            .Must(request => request.DifficultyQuotas is not { Count: > 0 }
                || request.DifficultyLevels is not { Count: > 0 })
            .WithName(nameof(UpsertTaskSelectionRuleRequest.DifficultyQuotas))
            .WithMessage("Нельзя одновременно задавать допустимые сложности и точные квоты.")
            .Must(request => request.DifficultyQuotas is not { Count: > 0 }
                || request.DifficultyQuotas.Sum(quota => quota.TaskCount) == request.TaskCount)
            .WithName(nameof(UpsertTaskSelectionRuleRequest.DifficultyQuotas))
            .WithMessage("Сумма квот по сложности должна совпадать с общим количеством заданий.");

        RuleFor(request => request.TopicRefs)
            .Must(refs => refs is null || refs.All(topicRef => !String.IsNullOrWhiteSpace(topicRef)))
            .WithMessage("Идентификаторы тем не должны быть пустыми.");
    }
}
