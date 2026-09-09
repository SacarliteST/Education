using Education.Contracts.PracticalModules;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreatePracticalModuleRequestValidator : AbstractValidator<CreatePracticalModuleRequest>
{
    public CreatePracticalModuleRequestValidator()
    {
        RuleFor(request => request.Slug)
            .NotEmpty().WithMessage("Slug обязателен.")
            .MaximumLength(100)
            .Matches("^[a-z0-9-]+$").WithMessage("Slug — латиница в нижнем регистре, цифры и дефис.");
        RuleFor(request => request.Name).NotEmpty().WithMessage("Название обязательно.").MaximumLength(200);
        RuleFor(request => request.PracticeType).NotEmpty().WithMessage("Тип практики обязателен.").MaximumLength(100);
        RuleFor(request => request.BasePath)
            .NotEmpty().WithMessage("Базовый путь обязателен.")
            .Matches("^/").WithMessage("Базовый путь должен начинаться с '/'.");
        RuleFor(request => request.IdentityAudience).NotEmpty().WithMessage("Audience обязателен.").MaximumLength(200);
    }
}

internal sealed class UpdatePracticalModuleRequestValidator : AbstractValidator<UpdatePracticalModuleRequest>
{
    public UpdatePracticalModuleRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage("Название обязательно.").MaximumLength(200);
        RuleFor(request => request.PracticeType).NotEmpty().WithMessage("Тип практики обязателен.").MaximumLength(100);
        RuleFor(request => request.BasePath)
            .NotEmpty().WithMessage("Базовый путь обязателен.")
            .Matches("^/").WithMessage("Базовый путь должен начинаться с '/'.");
        RuleFor(request => request.IdentityAudience).NotEmpty().WithMessage("Audience обязателен.").MaximumLength(200);
    }
}
