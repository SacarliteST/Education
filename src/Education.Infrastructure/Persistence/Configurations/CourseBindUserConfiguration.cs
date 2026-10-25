using Education.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="CourseBindUser"/> на таблицу базы данных.</summary>
internal sealed class CourseBindUserConfiguration : IEntityTypeConfiguration<CourseBindUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CourseBindUser> builder)
    {
        builder.ToTable("CourseBindUsers");
        builder.HasKey(bind => bind.Id);
        builder.Property(bind => bind.Id).HasColumnName("id");
        builder.Property(bind => bind.CourseId).HasColumnName("course_id");
        builder.Property(bind => bind.UserId).HasColumnName("user_id");
        builder.HasOne(bind => bind.Course).WithMany(course => course.CourseBindUsers).HasForeignKey(bind => bind.CourseId);
        builder.HasOne(bind => bind.User).WithMany(user => user.CourseBindUsers).HasForeignKey(bind => bind.UserId);
    }
}


