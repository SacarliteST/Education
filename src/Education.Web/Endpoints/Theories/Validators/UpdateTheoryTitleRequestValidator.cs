using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdateTheoryTitleRequest"/>.</summary>
internal sealed class UpdateTheoryTitleRequestValidator : AbstractValidator<UpdateTheoryTitleRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public UpdateTheoryTitleRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Заголовок теоретического материала обязателен.")
            .MaximumLength(200)
            .WithMessage("Заголовок теоретического материала не должен превышать 200 символов.");
    }
}
