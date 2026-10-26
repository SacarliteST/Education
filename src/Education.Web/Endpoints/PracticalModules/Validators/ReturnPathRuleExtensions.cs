using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Общее правило для пути возврата на платформу.</summary>
internal static class ReturnPathRuleExtensions
{
    /// <summary>
    /// Путь возврата — только относительный путь платформы (один «/» в начале, без «//», обратных косых черт и управляющих
    /// символов): иначе кнопка возврата в модуле стала бы открытым редиректом на произвольный сайт.
    /// </summary>
    /// <typeparam name="T">Тип запроса.</typeparam>
    /// <param name="rule">Правило для строкового свойства.</param>
    /// <returns>То же правило для продолжения цепочки.</returns>
    public static IRuleBuilderOptions<T, string?> MustBeRelativePlatformPath<T>(this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(512)
            .Must(path => path is null || (path.StartsWith('/')
                && !path.StartsWith("//", StringComparison.Ordinal)
                && !path.Contains('\\')
                && !path.Any(Char.IsControl)))
            .WithMessage("Путь возврата должен быть относительным путём платформы, начинающимся с одного '/'.");
    }
}
