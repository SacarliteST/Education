using Education.Contracts.Courses;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Правила валидации запроса <see cref="UpdateCourseStudentsRequest"/>.</summary>
internal sealed class UpdateCourseStudentsRequestValidator : AbstractValidator<UpdateCourseStudentsRequest>
{
    /// <summary>Создаёт правила валидации.</summary>
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
