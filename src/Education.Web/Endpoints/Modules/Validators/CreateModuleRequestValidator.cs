using Education.Contracts.Modules;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateModuleRequest"/>.</summary>
internal sealed class CreateModuleRequestValidator : AbstractValidator<CreateModuleRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateModuleRequestValidator()
    {
        RuleFor(request => request.CourseId)
            .NotEmpty()
            .WithMessage("Идентификатор курса обязателен.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название модуля обязательно.")
            .MaximumLength(200)
            .WithMessage("Название модуля не должно превышать 200 символов.");
    }
}

