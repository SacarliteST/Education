using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateTheoryRequest"/>.</summary>
internal sealed class CreateTheoryRequestValidator : AbstractValidator<CreateTheoryRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateTheoryRequestValidator()
    {
        RuleFor(request => request.ModuleId)
            .NotEmpty()
            .WithMessage("Идентификатор модуля обязателен.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название теоретического материала обязательно.")
            .MaximumLength(200)
            .WithMessage("Название теоретического материала не должно превышать 200 символов.");
    }
}
