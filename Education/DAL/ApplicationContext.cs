using Education.DAL.Configs;
using Education.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Education.DAL;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Answer> Answers { get; set; } = null!;
    public DbSet<Case> Cases { get; set; } = null!;
    public DbSet<CaseFile> CaseFiles { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Module> Modules { get; set; } = null!;
    public DbSet<PracticalMaterial> PracticalMaterials { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; set; } = null!;
    public DbSet<QuestionType> QuestionTypes { get; set; } = null!;
    public DbSet<TheoreticalMaterial> TheoreticalMaterials { get; set; } = null!;
    public DbSet<TheoreticalMaterialFile> TheoreticalMaterialFiles { get; set; } = null!;
    public DbSet<TheoreticalMaterialLink> TheoreticalMaterialLinks { get; set; } = null!;
    public DbSet<CourseBindUser> CourseBindUsers { get; set; } = null!;
    public DbSet<CaseFileComment> CaseFileComments { get; set; } = null!;
    public DbSet<TestResult> TestResults { get; set; } = null!;
    public DbSet<PracticalBindUser> PracticalBindUsers { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleConfig());
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new QuestionTypeConfig());
    }
}
