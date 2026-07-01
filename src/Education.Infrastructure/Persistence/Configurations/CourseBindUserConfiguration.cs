using Education.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class CourseBindUserConfiguration : IEntityTypeConfiguration<CourseBindUser>
{
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
