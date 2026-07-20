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

internal sealed class UpdateTheoryTextRequestValidator : AbstractValidator<UpdateTheoryTextRequest>
{
    public UpdateTheoryTextRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty()
            .WithMessage("Текст теоретического материала обязателен.");
    }
}

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

internal sealed class CreateTheoryLinkRequestValidator : AbstractValidator<CreateTheoryLinkRequest>
{
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

