using System.Security.Claims;
using Education.Application.AdminProfiles;
using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.Grades;
using Education.Application.Identity;
using Education.Application.Modules;
using Education.Application.Practicals;
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
using Education.Contracts.Questions;
using Education.Contracts.TaskFiles;
using Education.Contracts.TestResults;
using Education.Contracts.Theories;
using Education.Infrastructure;
using Education.Infrastructure.Files;
using Education.Web.Endpoints;
using Education.Web.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IEducationUserResolver, EducationUserResolver>();
builder.Services.AddScoped<IAdminProfilesService, AdminProfilesService>();
builder.Services.AddScoped<ICoursesService, CoursesService>();
builder.Services.AddScoped<IModulesService, ModulesService>();
builder.Services.AddScoped<IPracticalsService, PracticalsService>();
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

