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
    /// <summary>Создаёт правила валидации.</summary>
    public CreateModuleAuthoringLinkRequestValidator()
    {
        RuleFor(request => request.ReturnPath)
            .MustBeRelativePlatformPath();
        RuleFor(request => request.TaskRef)
            .MaximumLength(100)
            .Matches("^[A-Za-z0-9_-]+$")
            .When(request => request.TaskRef is not null)
            .WithMessage("Ссылка на задание — латиница, цифры, '-' и '_'.");
    }
}
