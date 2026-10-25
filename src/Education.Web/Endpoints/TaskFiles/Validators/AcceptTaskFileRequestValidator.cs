using Education.Contracts.TaskFiles;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class AcceptTaskFileRequestValidator : AbstractValidator<AcceptTaskFileRequest>
{
    public AcceptTaskFileRequestValidator()
    {
        RuleFor(request => request.Grade)
            .InclusiveBetween(2, 5)
            .WithMessage("Оценка должна быть от 2 до 5.");
    }
}
