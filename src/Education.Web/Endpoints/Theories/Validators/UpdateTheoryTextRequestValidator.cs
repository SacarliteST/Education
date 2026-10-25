using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdateTheoryTextRequest"/>.</summary>
internal sealed class UpdateTheoryTextRequestValidator : AbstractValidator<UpdateTheoryTextRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public UpdateTheoryTextRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст теоретического материала обязателен.");
    }
}
