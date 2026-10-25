using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="BindPracticalModuleRequest"/>.</summary>
internal sealed class BindPracticalModuleRequestValidator : AbstractValidator<BindPracticalModuleRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public BindPracticalModuleRequestValidator()
    {
        RuleFor(request => request.PracticalModuleId)
            .NotEmpty()
            .WithMessage("Идентификатор модуля обязателен.");

        RuleFor(request => request.ExternalTaskRef)
            .NotEmpty()
            .WithMessage("Ключ задания модуля обязателен.")
            .MaximumLength(200)
            .WithMessage("Ключ задания модуля не должен превышать 200 символов.");

        RuleFor(request => request.TriesCount)
            .GreaterThan(0)
            .WithMessage("Количество попыток должно быть больше 0.");

        RuleFor(request => request.TimeLimitMinutes)
            .GreaterThan(0)
            .When(request => request.TimeLimitMinutes.HasValue)
            .WithMessage("Лимит времени должен быть больше 0 минут.");

        RuleFor(request => request.AttemptsPerSession)
            .InclusiveBetween(1, 1000)
            .When(request => request.AttemptsPerSession.HasValue)
            .WithMessage("Число попыток отправки в заходе должно быть от 1 до 1000.");
    }
}
