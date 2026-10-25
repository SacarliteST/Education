using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateTheoryDocumentRequestValidator : AbstractValidator<CreateTheoryDocumentRequest>
{
    public CreateTheoryDocumentRequestValidator()
    {
        RuleFor(request => request.TheoryMaterialId)
            .NotEmpty()
            .WithMessage("Идентификатор теоретического материала обязателен.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .WithMessage("Описание документа обязательно.")
            .MaximumLength(1000)
            .WithMessage("Описание документа не должно превышать 1000 символов.");
    }
}
