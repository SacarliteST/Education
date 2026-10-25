using Education.Contracts.Practicals;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreatePracticalRequest"/>.</summary>
internal sealed class CreatePracticalRequestValidator : AbstractValidator<CreatePracticalRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreatePracticalRequestValidator()
    {
        RuleFor(request => request.ModuleId)
            .NotEmpty()
            .WithMessage("Идентификатор модуля обязателен.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Название практического материала обязательно.")
            .MaximumLength(200)
            .WithMessage("Название практического материала не должно превышать 200 символов.");
    }
}
