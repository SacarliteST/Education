using Education.Contracts.Theories;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateTheoryRequestValidator : AbstractValidator<CreateTheoryRequest>
{
    public CreateTheoryRequestValidator()
    {
        RuleFor(request => request.ModuleId)
            .GreaterThan(0);

        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}

internal sealed class UpdateTheoryTitleRequestValidator : AbstractValidator<UpdateTheoryTitleRequest>
{
    public UpdateTheoryTitleRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}

internal sealed class UpdateTheoryTextRequestValidator : AbstractValidator<UpdateTheoryTextRequest>
{
    public UpdateTheoryTextRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty();
    }
}

internal sealed class CreateTheoryDocumentRequestValidator : AbstractValidator<CreateTheoryDocumentRequest>
{
    public CreateTheoryDocumentRequestValidator()
    {
        RuleFor(request => request.TheoryMaterialId)
            .GreaterThan(0);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(1000);
    }
}

internal sealed class CreateTheoryLinkRequestValidator : AbstractValidator<CreateTheoryLinkRequest>
{
    public CreateTheoryLinkRequestValidator()
    {
        RuleFor(request => request.TheoryMaterialId)
            .GreaterThan(0);

        RuleFor(request => request.Link)
            .NotEmpty()
            .MaximumLength(2048);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
