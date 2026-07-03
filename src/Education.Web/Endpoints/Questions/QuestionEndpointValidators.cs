using Education.Contracts.Questions;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class CreateQuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {
        RuleFor(request => request.ModuleId).NotEmpty();
        RuleFor(request => request.Type).NotEmpty();
        RuleFor(request => request.Text).NotEmpty();
        RuleFor(request => request.Body).NotEmpty();
        RuleFor(request => request.Answer).NotEmpty();
        RuleFor(request => request.Weight).GreaterThan(0);
    }
}

internal sealed class UpdateQuestionRequestValidator : AbstractValidator<UpdateQuestionRequest>
{
    public UpdateQuestionRequestValidator()
    {
        RuleFor(request => request.Type).NotEmpty();
        RuleFor(request => request.Text).NotEmpty();
        RuleFor(request => request.Body).NotEmpty();
        RuleFor(request => request.Answer).NotEmpty();
        RuleFor(request => request.Weight).GreaterThan(0);
    }
}

