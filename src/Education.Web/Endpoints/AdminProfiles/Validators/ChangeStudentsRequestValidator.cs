using Education.Contracts.AdminProfiles;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>
/// Проверяет точечное изменение набора студентов: списки не пусты одновременно, ограничены по размеру
/// и не пересекаются (иначе результат зависел бы от порядка применения).
/// </summary>
internal sealed class ChangeStudentsRequestValidator : AbstractValidator<ChangeStudentsRequest>
{
    /// <summary>Максимальное число идентификаторов в каждом из списков одного запроса.</summary>
    public const int MaxIdsPerList = 2000;

    public ChangeStudentsRequestValidator()
    {
        RuleFor(request => request.Add)
            .NotNull().WithMessage("Список добавляемых студентов обязателен.")
            .Must(ids => ids is null || ids.Count <= MaxIdsPerList)
            .WithMessage($"За один запрос можно добавить не более {MaxIdsPerList} студентов.");
        RuleFor(request => request.Remove)
            .NotNull().WithMessage("Список снимаемых студентов обязателен.")
            .Must(ids => ids is null || ids.Count <= MaxIdsPerList)
            .WithMessage($"За один запрос можно снять не более {MaxIdsPerList} студентов.");

        RuleForEach(request => request.Add)
            .NotEmpty().WithMessage("Идентификатор студента не может быть пустым.");
        RuleForEach(request => request.Remove)
            .NotEmpty().WithMessage("Идентификатор студента не может быть пустым.");

        RuleFor(request => request)
            .Must(request => request.Add is null || request.Remove is null || !request.Add.Intersect(request.Remove).Any())
            .WithName("Add")
            .WithMessage("Один и тот же студент не может быть одновременно в списках добавления и снятия.");
    }
}
