using Education.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="ClassWindowStudent"/> на таблицу базы данных.</summary>
internal sealed class ClassWindowStudentConfiguration : IEntityTypeConfiguration<ClassWindowStudent>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ClassWindowStudent> builder)
    {
        builder.ToTable("ClassWindowStudents");
        builder.HasKey(student => student.Id);
        builder.Property(student => student.Id).HasColumnName("id");
        builder.Property(student => student.ClassWindowId).HasColumnName("class_window_id");
        builder.Property(student => student.UserId).HasColumnName("user_id");
        builder.HasIndex(student => new { student.ClassWindowId, student.UserId }).IsUnique();
    }
}
