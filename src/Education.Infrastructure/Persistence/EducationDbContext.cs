using Education.Domain.Audit;
using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Domain.Practicals;
using Education.Domain.PracticalModules;
using Education.Domain.Tests;
using Education.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Persistence;

/// <summary>
/// Контекст базы данных Education API.
/// </summary>
/// <param name="options">Параметры подключения и настройки Entity Framework Core.</param>
public sealed class EducationDbContext(DbContextOptions<EducationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Журнал административных действий Education (не путать с аудитом
    /// IdentityService — тот про логины/роли/блокировки).
    /// </summary>
    public DbSet<AdminEvent> AdminEvents => Set<AdminEvent>();

    /// <summary>
    /// Связи пользователей identity-сервиса с пользователями учебной системы.
    /// </summary>
    public DbSet<IdentityUserLink> IdentityUserLinks => Set<IdentityUserLink>();

    /// <summary>
    /// Пользователи учебной системы.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Роли пользователей.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Курсы.
    /// </summary>
    public DbSet<Course> Courses => Set<Course>();

    /// <summary>
    /// Модули курсов.
    /// </summary>
    public DbSet<Module> Modules => Set<Module>();

    /// <summary>
    /// Назначения студентов на курсы.
    /// </summary>
    public DbSet<CourseBindUser> CourseBindUsers => Set<CourseBindUser>();

    /// <summary>
    /// Практические материалы.
    /// </summary>
    public DbSet<PracticalMaterial> PracticalMaterials => Set<PracticalMaterial>();

    /// <summary>
    /// Реестр внешних практических модулей.
    /// </summary>
    public DbSet<PracticalModule> PracticalModules => Set<PracticalModule>();

    /// <summary>
    /// Попытки прохождения внешних практических модулей.
    /// </summary>
    public DbSet<PracticalModuleSession> PracticalModuleSessions => Set<PracticalModuleSession>();

    /// <summary>
    /// «Цифровой след» — журнал действий студента во внешних модулях.
    /// </summary>
    public DbSet<PracticalTaskEvent> PracticalTaskEvents => Set<PracticalTaskEvent>();

    /// <summary>
    /// Назначения студентов на практические материалы.
    /// </summary>
    public DbSet<PracticalBindUser> PracticalBindUsers => Set<PracticalBindUser>();

    /// <summary>
    /// Задачи практических материалов.
    /// </summary>
    public DbSet<Case> Cases => Set<Case>();

    /// <summary>
    /// Файлы решений задач.
    /// </summary>
    public DbSet<CaseFile> CaseFiles => Set<CaseFile>();

    /// <summary>
    /// Комментарии к файлам решений.
    /// </summary>
    public DbSet<CaseFileComment> CaseFileComments => Set<CaseFileComment>();

    /// <summary>
    /// Вопросы тестирования.
    /// </summary>
    public DbSet<Question> Questions => Set<Question>();

    /// <summary>
    /// Типы вопросов тестирования.
    /// </summary>
    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();

    /// <summary>
    /// Связи практических материалов с вопросами.
    /// </summary>
    public DbSet<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions => Set<PracticalMaterialBindQuestion>();

    /// <summary>
    /// Результаты прохождения тестов.
    /// </summary>
    public DbSet<TestResult> TestResults => Set<TestResult>();

    /// <summary>
    /// Ответы студентов в тестах.
    /// </summary>
    public DbSet<Answer> Answers => Set<Answer>();

    /// <summary>
    /// Теоретические материалы.
    /// </summary>
    public DbSet<TheoreticalMaterial> TheoreticalMaterials => Set<TheoreticalMaterial>();

    /// <summary>
    /// Файлы теоретических материалов.
    /// </summary>
    public DbSet<TheoreticalMaterialFile> TheoreticalMaterialFiles => Set<TheoreticalMaterialFile>();

    /// <summary>
    /// Ссылки теоретических материалов.
    /// </summary>
    public DbSet<TheoreticalMaterialLink> TheoreticalMaterialLinks => Set<TheoreticalMaterialLink>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EducationDbContext).Assembly);
    }
}


