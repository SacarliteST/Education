using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.Grades;
using Education.Application.Modules;
using Education.Application.Practicals;
using Education.Application.Questions;
using Education.Application.TaskFiles;
using Education.Application.TestResults;
using Education.Application.Theories;
using Education.Application.Users;
using Education.Infrastructure.Courses;
using Education.Infrastructure.Files;
using Education.Infrastructure.Grades;
using Education.Infrastructure.Modules;
using Education.Infrastructure.Persistence;
using Education.Infrastructure.Practicals;
using Education.Infrastructure.Questions;
using Education.Infrastructure.TaskFiles;
using Education.Infrastructure.TestResults;
using Education.Infrastructure.Theories;
using Education.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEducationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EducationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<ICoursesRepository, EfCoursesRepository>();
        services.AddScoped<IModulesRepository, EfModulesRepository>();
        services.AddScoped<IPracticalsRepository, EfPracticalsRepository>();
        services.AddScoped<IQuestionsRepository, EfQuestionsRepository>();
        services.AddScoped<ITestResultsRepository, EfTestResultsRepository>();
        services.AddScoped<IGradesRepository, EfGradesRepository>();
        services.AddScoped<ITheoriesRepository, EfTheoriesRepository>();
        services.AddScoped<ITaskFilesRepository, EfTaskFilesRepository>();
        services.AddScoped<IFileAccessRepository, EfFileAccessRepository>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IUserProfileRepository, EfUserProfileRepository>();

        return services;
    }
}
