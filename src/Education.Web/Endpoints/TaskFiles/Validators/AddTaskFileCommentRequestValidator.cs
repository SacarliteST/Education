using Education.Contracts.TaskFiles;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="AddTaskFileCommentRequest"/>.</summary>
internal sealed class AddTaskFileCommentRequestValidator : AbstractValidator<AddTaskFileCommentRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
    public AddTaskFileCommentRequestValidator()
    {
        RuleFor(request => request.Comment)
            .NotEmpty()
            .WithMessage("Комментарий обязателен.")
            .MaximumLength(1000)
            .WithMessage("Комментарий не должен превышать 1000 символов.");
    }
}
