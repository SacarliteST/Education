using Education.Contracts.TaskFiles;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class AddTaskFileCommentRequestValidator : AbstractValidator<AddTaskFileCommentRequest>
{
    public AddTaskFileCommentRequestValidator()
    {
        RuleFor(request => request.Comment)
            .NotEmpty()
            .MaximumLength(1000);
    }
}

internal sealed class AcceptTaskFileRequestValidator : AbstractValidator<AcceptTaskFileRequest>
{
    public AcceptTaskFileRequestValidator()
    {
        RuleFor(request => request.Grade)
            .InclusiveBetween(2, 5);
    }
}

