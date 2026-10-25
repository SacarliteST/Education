using Education.Contracts.PracticalModules;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdatePracticalModuleRequest"/>.</summary>
internal sealed class UpdatePracticalModuleRequestValidator : AbstractValidator<UpdatePracticalModuleRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
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
