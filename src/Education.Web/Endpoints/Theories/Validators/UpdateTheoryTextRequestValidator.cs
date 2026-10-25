using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdateTheoryTextRequestValidator : AbstractValidator<UpdateTheoryTextRequest>
{
    public UpdateTheoryTextRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст теоретического материала обязателен.");
    }
}
