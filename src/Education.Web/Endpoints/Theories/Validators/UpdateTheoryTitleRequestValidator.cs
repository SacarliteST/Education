using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdateTheoryTitleRequestValidator : AbstractValidator<UpdateTheoryTitleRequest>
{
    public UpdateTheoryTitleRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .WithMessage("Заголовок теоретического материала обязателен.")
            .MaximumLength(200)
            .WithMessage("Заголовок теоретического материала не должен превышать 200 символов.");
    }
}
