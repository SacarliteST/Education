using System.Security.Claims;
using Education.Application.AdminProfiles;
using Education.Application.Courses;
using Education.Application.Files;
using Education.Kafka;
using Education.Application.Grades;
using Education.Application.Identity;
using Education.Application.Modules;
using Education.Application.Practicals;
using Education.Application.PracticalModules;
using Education.Application.Questions;
using Education.Application.TaskFiles;
using Education.Application.TestResults;
using Education.Application.Theories;
using Education.Application.Users;
using Education.Contracts.AdminProfiles;
using Education.Contracts.Auth;
using Education.Contracts.Courses;
using Education.Contracts.Grades;
using Education.Contracts.Modules;
using Education.Contracts.Practicals;
using Education.Contracts.PracticalModules;
using Education.Contracts.Questions;
using Education.Contracts.TaskFiles;
using Education.Contracts.TestResults;
using Education.Contracts.Theories;
using Education.Infrastructure;
using Education.Infrastructure.Files;
using Education.Web.Endpoints;
using Education.Web.Identity;
using Education.Web.Integration;
using Education.Infrastructure.Persistence;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var identitySection = builder.Configuration.GetSection("Identity");
var jwtSection = builder.Configuration.GetSection("Jwt");
var configuredFrontendOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
var frontendOrigins = configuredFrontendOrigins is { Length: > 0 }
    ? configuredFrontendOrigins
    : ["http://localhost:5173", "https://localhost:5173"];
const string frontendCorsPolicy = "Frontend";

builder.Services.AddHttpContextAccessor();
// Каждый запрос — одна Information-строка (метод/путь/статус/длительность).
// Специально БЕЗ заголовков и тела запроса/ответа — иначе в лог попал бы
// Authorization: Bearer <JWT>. См. SQLTren/PLATFORM.md, задачи по
// логированию кода, п.2.
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.ResponseStatusCode
        | HttpLoggingFields.Duration;
});
builder.Services.AddKafkaMessaging(builder.Configuration);
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IEducationUserResolver, EducationUserResolver>();
builder.Services.AddScoped<IAdminProfilesService, AdminProfilesService>();
builder.Services.AddScoped<ICoursesService, CoursesService>();
builder.Services.AddScoped<IModulesService, ModulesService>();
builder.Services.AddScoped<IPracticalsService, PracticalsService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IPracticalModulesService, PracticalModulesService>();
builder.Services.AddScoped<IModuleCatalogService, ModuleCatalogService>();
builder.Services.AddHttpClient<IModuleCatalogClient, HttpModuleCatalogClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(3));
builder.Services.Configure<ModuleIntegrationOptions>(
    builder.Configuration.GetSection(ModuleIntegrationOptions.SectionKey));
builder.Services.AddScoped<IModuleIntegrationConfig, ModuleIntegrationConfig>();
builder.Services.AddScoped<IModuleAuthoringService, ModuleAuthoringService>();
builder.Services.AddScoped<IModuleSessionsService, ModuleSessionsService>();
builder.Services.AddScoped<IPracticeEventHandler, PracticeEventHandler>();
builder.Services.AddHttpClient<IModulePushClient, HttpModulePushClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(5));
builder.Services.AddHttpClient<ITokenExchangeClient, HttpTokenExchangeClient>(client =>
    client.Timeout = TimeSpan.FromSeconds(5));
builder.Services.AddHostedService<PracticeEventConsumer>();
builder.Services.AddScoped<IQuestionsService, QuestionsService>();
builder.Services.AddScoped<ITestResultsService, TestResultsService>();
builder.Services.AddScoped<IGradesService, GradesService>();
builder.Services.AddScoped<ITheoriesService, TheoriesService>();
builder.Services.AddScoped<ITaskFilesService, TaskFilesService>();
builder.Services.AddScoped<IFilesService, FilesService>();
builder.Services.AddScoped<IValidator<CreateAdminProfileRequest>, CreateAdminProfileRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateAdminProfileRequest>, UpdateAdminProfileRequestValidator>();
builder.Services.AddScoped<IValidator<CreateCourseRequest>, CreateCourseRequestValidator>();
builder.Services.AddScoped<IValidator<CreateModuleRequest>, CreateModuleRequestValidator>();
builder.Services.AddScoped<IValidator<CreatePracticalRequest>, CreatePracticalRequestValidator>();
builder.Services.AddScoped<IValidator<BindPracticalModuleRequest>, BindPracticalModuleRequestValidator>();
builder.Services.AddScoped<IValidator<CreatePracticalModuleRequest>, CreatePracticalModuleRequestValidator>();
builder.Services.AddScoped<IValidator<CreateModuleAuthoringLinkRequest>, CreateModuleAuthoringLinkRequestValidator>();
builder.Services.AddScoped<IValidator<UpdatePracticalModuleRequest>, UpdatePracticalModuleRequestValidator>();
builder.Services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskTextRequest>, UpdateTaskTextRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateCourseStudentsRequest>, UpdateCourseStudentsRequestValidator>();
builder.Services.AddScoped<IValidator<UpdatePracticalStudentsRequest>, UpdatePracticalStudentsRequestValidator>();
builder.Services.AddScoped<IValidator<ChangeStudentsRequest>, ChangeStudentsRequestValidator>();
builder.Services.AddScoped<IValidator<ConfigurePracticalQuestionsRequest>, ConfigurePracticalQuestionsRequestValidator>();
builder.Services.AddScoped<IValidator<CreateQuestionRequest>, CreateQuestionRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateQuestionRequest>, UpdateQuestionRequestValidator>();
builder.Services.AddScoped<IValidator<SubmitTestRequest>, SubmitTestRequestValidator>();
builder.Services.AddScoped<IValidator<CreateTheoryRequest>, CreateTheoryRequestValidator>();
builder.Services.AddScoped<IValidator<CreateTheoryDocumentRequest>, CreateTheoryDocumentRequestValidator>();
builder.Services.AddScoped<IValidator<CreateTheoryLinkRequest>, CreateTheoryLinkRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateTheoryTextRequest>, UpdateTheoryTextRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateTheoryTitleRequest>, UpdateTheoryTitleRequestValidator>();
builder.Services.AddScoped<IValidator<AddTaskFileCommentRequest>, AddTaskFileCommentRequestValidator>();
builder.Services.AddScoped<IValidator<AcceptTaskFileRequest>, AcceptTaskFileRequestValidator>();
builder.Services.AddEducationInfrastructure(builder.Configuration);
builder.Services.Configure<FileStorageOptions>(options =>
{
    options.RootPath = Path.Combine(builder.Environment.ContentRootPath, "Files");
    builder.Configuration.GetSection("FileStorage").Bind(options);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = identitySection["Authority"];
        options.Audience = identitySection["Audience"];
        options.RequireHttpsMetadata = jwtSection.GetValue("RequireHttpsMetadata", true);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = jwtSection["NameClaimType"] ?? ClaimTypes.Name,
            RoleClaimType = jwtSection["RoleClaimType"] ?? ClaimTypes.Role,
            ValidateAudience = !String.IsNullOrWhiteSpace(identitySection["Audience"]),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.AuthenticatedEducationUser, policy =>
        policy.RequireAuthenticatedUser());
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
        policy.RequireRole(EducationRoles.Admin));
    options.AddPolicy(AuthorizationPolicies.TeacherOnly, policy =>
        policy.RequireRole(EducationRoles.Teacher));
    options.AddPolicy(AuthorizationPolicies.StudentOnly, policy =>
        policy.RequireRole(EducationRoles.Student));
});

// TD-001: web-дефолт System.Text.Json включает AllowReadingFromString → .NET-OpenAPI
// документирует каждое числовое поле как union [integer|number, string] + pattern,
// Orval генерирует `number | string`. Strict — числа только числами, схема чистая.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Первым в конвейере — ловит исключения из всего, что ниже. Раньше у
// Education глобального обработчика не было вообще (см. Endpoints/
// ExceptionHandlerExtensions.cs).
app.UseApiExceptionHandler();
app.UseHttpLogging();

// TD-011: вне Development миграции накатываются при старте только по флагу
// Database:ApplyMigrationsOnStartup (по умолчанию false) — тогда схему на
// прод/стейджинге догоняет сам процесс на деплое, без отдельного шага
// `dotnet ef database update`. В Development — всегда, для удобства разработки.
var applyMigrationsOnStartup = app.Environment.IsDevelopment()
    || app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", false);

if (applyMigrationsOnStartup)
{
    await using var migrationScope = app.Services.CreateAsyncScope();
    var migrationDbContext = migrationScope.ServiceProvider.GetRequiredService<EducationDbContext>();

    var pendingMigrations = (await migrationDbContext.Database.GetPendingMigrationsAsync()).ToList();
    if (pendingMigrations.Count > 0)
    {
        app.Logger.LogInformation(
            "Применяю миграции EducationDb при старте ({Count}): {Migrations}",
            pendingMigrations.Count,
            String.Join(", ", pendingMigrations));
    }

    // MigrateAsync берёт advisory-lock на __EFMigrationsHistory — безопасно,
    // когда стартует несколько реплик одновременно.
    await migrationDbContext.Database.MigrateAsync();
}

app.UseCors(frontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("Health")
    .WithTags("System")
    .WithSummary("Проверка работоспособности API")
    .WithDescription("Возвращает успешный ответ, если приложение запущено.")
    .Produces(StatusCodes.Status200OK);

var authGroup = app.MapGroup("/api/v1/auth")
    .WithTags("Auth");

authGroup.MapGet("/me", (ICurrentUser currentUser) => Results.Ok(new AuthMeResponse(
        currentUser.UserId,
        currentUser.Email,
        currentUser.Name,
        currentUser.Roles,
        currentUser.IsAuthenticated)))
    .WithName("GetCurrentUser")
    .WithSummary("Получение текущего пользователя")
    .WithDescription("Возвращает сведения о пользователе, извлечённые из Bearer JWT-токена.")
    .Produces<AuthMeResponse>()
    .Produces(StatusCodes.Status401Unauthorized)
    .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

authGroup.MapPost("/logout", () => Results.NoContent())
    .WithName("LogoutEducationSession")
    .WithSummary("Завершение локальной сессии Education API")
    .WithDescription("Education API не хранит cookie-сессию; endpoint оставлен для совместимого выхода клиента после очистки токенов.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized)
    .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

app.MapGet("/api/v1/student/ping", () => Results.Ok())
    .RequireAuthorization(AuthorizationPolicies.StudentOnly)
    .WithTags("Student")
    .WithSummary("Проверка доступа студента")
    .WithDescription("Проверяет, что Bearer-токен содержит роль Student.")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

app.MapGet("/api/v1/teacher/ping", () => Results.Ok())
    .RequireAuthorization(AuthorizationPolicies.TeacherOnly)
    .WithTags("Teacher")
    .WithSummary("Проверка доступа преподавателя")
    .WithDescription("Проверяет, что Bearer-токен содержит роль Teacher.")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

app.MapGet("/api/v1/admin/ping", () => Results.Ok())
    .RequireAuthorization(AuthorizationPolicies.AdminOnly)
    .WithTags("Admin")
    .WithSummary("Проверка доступа администратора")
    .WithDescription("Проверяет, что Bearer-токен содержит роль Admin.")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

app.MapEducationEndpoints();

app.Run();

/// <summary>
/// Точка входа Education API, используемая приложением и интеграционными тестами.
/// </summary>
public partial class Program;

