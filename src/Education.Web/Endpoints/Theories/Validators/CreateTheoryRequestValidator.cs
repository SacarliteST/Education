using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateTheoryRequestValidator : AbstractValidator<CreateTheoryRequest>
{
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
