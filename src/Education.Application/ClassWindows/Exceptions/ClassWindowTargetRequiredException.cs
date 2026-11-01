namespace Education.Application.ClassWindows;

/// <summary>Пару нельзя начать без цели: нужна группа и/или хотя бы один студент.</summary>
public sealed class ClassWindowTargetRequiredException() : Exception("Укажите группу или хотя бы одного студента.");
