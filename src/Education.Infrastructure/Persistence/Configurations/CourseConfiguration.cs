using Education.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="Course"/> на таблицу базы данных.</summary>
internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(course => course.Id);
        builder.Property(course => course.Id).HasColumnName("id");
        builder.Property(course => course.Name).HasColumnName("c_name").IsRequired();
        builder.Property(course => course.Description).HasColumnName("description").IsRequired();
        builder.Property(course => course.Date).HasColumnName("date");
        builder.Property(course => course.UserId).HasColumnName("user_id");
        builder.Property(course => course.PracticeOnlyDuringClass).HasColumnName("practice_only_during_class").HasDefaultValue(false);
        builder.HasOne(course => course.User).WithMany(user => user.Courses).HasForeignKey(course => course.UserId);
    }
}


