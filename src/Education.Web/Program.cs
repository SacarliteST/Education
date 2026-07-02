using System.Security.Claims;
using Education.Application.Courses;
using Education.Application.Identity;
using Education.Application.Users;
using Education.Infrastructure;
using Education.Infrastructure.Courses;
using Education.Web.Endpoints;
using Education.Web.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var identitySection = builder.Configuration.GetSection("Identity");
var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IEducationUserResolver, EducationUserResolver>();
builder.Services.AddScoped<ICoursesService, CoursesService>();
builder.Services.AddEducationInfrastructure(builder.Configuration);
builder.Services.Configure<PublicTheoryDocumentStorageOptions>(options =>
{
    options.RootPath = builder.Environment.ContentRootPath;
    options.DirectoryName = "Files";
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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("Health")
    .WithTags("System");

var authGroup = app.MapGroup("/api/v1/auth")
    .WithTags("Auth");

authGroup.MapGet("/me", (ICurrentUser currentUser) => Results.Ok(new
{
    currentUser.UserId,
    currentUser.Email,
    currentUser.Name,
    currentUser.Roles,
    currentUser.IsAuthenticated
}))
    .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

authGroup.MapPost("/logout", () => Results.NoContent())
    .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

app.MapGet("/api/v1/student/ping", () => Results.Ok())
    .RequireAuthorization(AuthorizationPolicies.StudentOnly)
    .WithTags("Student");

app.MapGet("/api/v1/teacher/ping", () => Results.Ok())
    .RequireAuthorization(AuthorizationPolicies.TeacherOnly)
    .WithTags("Teacher");

app.MapGet("/api/v1/admin/ping", () => Results.Ok())
    .RequireAuthorization(AuthorizationPolicies.AdminOnly)
    .WithTags("Admin");

app.MapCoursesEndpoints();

app.Run();

public partial class Program;
