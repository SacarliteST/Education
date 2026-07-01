using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Persistence;

public sealed class EducationDbContext(DbContextOptions<EducationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<CourseBindUser> CourseBindUsers => Set<CourseBindUser>();
    public DbSet<PracticalMaterial> PracticalMaterials => Set<PracticalMaterial>();
    public DbSet<PracticalBindUser> PracticalBindUsers => Set<PracticalBindUser>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<CaseFile> CaseFiles => Set<CaseFile>();
    public DbSet<CaseFileComment> CaseFileComments => Set<CaseFileComment>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();
    public DbSet<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions => Set<PracticalMaterialBindQuestion>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<TheoreticalMaterial> TheoreticalMaterials => Set<TheoreticalMaterial>();
    public DbSet<TheoreticalMaterialFile> TheoreticalMaterialFiles => Set<TheoreticalMaterialFile>();
    public DbSet<TheoreticalMaterialLink> TheoreticalMaterialLinks => Set<TheoreticalMaterialLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EducationDbContext).Assembly);
    }
}
