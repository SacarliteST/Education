using Education.Contracts.TaskFiles;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class AddTaskFileCommentRequestValidator : AbstractValidator<AddTaskFileCommentRequest>
{
    public AddTaskFileCommentRequestValidator()
    {
        RuleFor(request => request.Comment)
            .NotEmpty()
            .WithMessage("Комментарий обязателен.")
            .MaximumLength(1000)
            .WithMessage("Комментарий не должен превышать 1000 символов.");
    }
}
