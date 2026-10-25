using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateTheoryLinkRequest"/>.</summary>
internal sealed class CreateTheoryLinkRequestValidator : AbstractValidator<CreateTheoryLinkRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateTheoryLinkRequestValidator()
    {
        RuleFor(request => request.TheoryMaterialId)
            .NotEmpty()
            .WithMessage("Идентификатор теоретического материала обязателен.");

        RuleFor(request => request.Link)
            .NotEmpty()
            .WithMessage("Ссылка обязательна.")
            .MaximumLength(2048)
            .WithMessage("Ссылка не должна превышать 2048 символов.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Описание ссылки обязательно.")
            .MaximumLength(1000)
            .WithMessage("Описание ссылки не должно превышать 1000 символов.");
    }
}
