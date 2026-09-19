using Education.Contracts.PracticalModules;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreatePracticalModuleRequestValidator : AbstractValidator<CreatePracticalModuleRequest>
{
    public CreatePracticalModuleRequestValidator()
    {
        RuleFor(request => request.Slug)
            .NotEmpty().WithMessage("Slug обязателен.")
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug — латиница в нижнем регистре, цифры и дефис.");
        RuleFor(request => request.Name).NotEmpty().WithMessage("Название обязательно.").MaximumLength(200);
        RuleFor(request => request.PracticeType).NotEmpty().WithMessage("Тип практики обязателен.").MaximumLength(100);
        RuleFor(request => request.BasePath)
            .NotEmpty().WithMessage("Базовый путь обязателен.")
            .Matches("^/").WithMessage("Базовый путь должен начинаться с '/'.");
        RuleFor(request => request.IdentityAudience).NotEmpty().WithMessage("Audience обязателен.").MaximumLength(200);
    }
}

internal sealed class UpdatePracticalModuleRequestValidator : AbstractValidator<UpdatePracticalModuleRequest>
{
    public UpdatePracticalModuleRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage("Название обязательно.").MaximumLength(200);
        RuleFor(request => request.PracticeType).NotEmpty().WithMessage("Тип практики обязателен.").MaximumLength(100);
        RuleFor(request => request.BasePath)
            .NotEmpty().WithMessage("Базовый путь обязателен.")
            .Matches("^/").WithMessage("Базовый путь должен начинаться с '/'.");
        RuleFor(request => request.IdentityAudience).NotEmpty().WithMessage("Audience обязателен.").MaximumLength(200);
    }
}
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
