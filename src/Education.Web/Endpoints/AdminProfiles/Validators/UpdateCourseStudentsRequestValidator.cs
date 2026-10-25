using Education.Contracts.Courses;
using FluentValidation;

namespace Education.Web.Endpoints;

internal sealed class UpdateCourseStudentsRequestValidator : AbstractValidator<UpdateCourseStudentsRequest>
{
    public UpdateCourseStudentsRequestValidator()
    {
        RuleFor(request => request.UserIds)
            .NotNull()
            .WithMessage("Список студентов обязателен.");

        RuleForEach(request => request.UserIds)
            .NotEmpty()
            .WithMessage("Идентификатор студента не может быть пустым.");
    }
}
