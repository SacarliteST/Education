using Education.Contracts.PracticalModules;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="CreateTrainingLinkRequest"/>.</summary>
internal sealed class CreateTrainingLinkRequestValidator : AbstractValidator<CreateTrainingLinkRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public CreateTrainingLinkRequestValidator()
    {
        RuleFor(request => request.ReturnPath).MustBeRelativePlatformPath();
    }
}
