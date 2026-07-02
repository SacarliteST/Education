using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.Theories;
using Education.Application.Users;
using Education.Infrastructure.Courses;
using Education.Infrastructure.Modules;
using Education.Infrastructure.Persistence;
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
        services.AddScoped<ITheoriesRepository, EfTheoriesRepository>();
        services.AddScoped<ITheoryDocumentStorage, PublicTheoryDocumentStorage>();
        services.AddScoped<IUserProfileRepository, EfUserProfileRepository>();

        return services;
    }
}
