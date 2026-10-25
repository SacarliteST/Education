using Education.Contracts.PracticalModules;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>
/// Проверяет параметры ссылки авторинга. <c>ReturnPath</c> — только относительный путь платформы
/// (один «/» в начале, без «//», обратных косых черт и управляющих символов): иначе кнопка возврата в модуле
/// стала бы открытым редиректом на произвольный сайт.
/// </summary>
internal sealed class CreateModuleAuthoringLinkRequestValidator : AbstractValidator<CreateModuleAuthoringLinkRequest>
{
    public CreateModuleAuthoringLinkRequestValidator()
    {
        RuleFor(request => request.ReturnPath)
            .MaximumLength(512)
            .Must(path => path is null || (path.StartsWith('/')
                && !path.StartsWith("//", StringComparison.Ordinal)
                && !path.Contains('\\')
                && !path.Any(Char.IsControl)))
            .WithMessage("Путь возврата должен быть относительным путём платформы, начинающимся с одного '/'.");
        RuleFor(request => request.TaskRef)
            .MaximumLength(100)
            .Matches("^[A-Za-z0-9_-]+$")
            .When(request => request.TaskRef is not null)
            .WithMessage("Ссылка на задание — латиница, цифры, '-' и '_'.");
    }
}
