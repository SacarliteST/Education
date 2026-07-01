using Education.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules");
        builder.HasKey(module => module.Id);
        builder.Property(module => module.Id).HasColumnName("id");
        builder.Property(module => module.Name).HasColumnName("m_name").IsRequired();
        builder.Property(module => module.CourseId).HasColumnName("course_id");
        builder.HasOne(module => module.Course).WithMany(course => course.Modules).HasForeignKey(module => module.CourseId);
    }
}
