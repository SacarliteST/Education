namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Правило случайной выборки можно задать только для внешней практики.</summary>
public sealed class PracticalNotExternalException()
    : Exception("Правило случайной выборки задаётся только для практики, привязанной к внешнему модулю.");
