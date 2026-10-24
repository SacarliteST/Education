using Education.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class ClassWindowConfiguration : IEntityTypeConfiguration<ClassWindow>
{
    public void Configure(EntityTypeBuilder<ClassWindow> builder)
    {
        builder.ToTable("ClassWindows");
        builder.HasKey(window => window.Id);
        builder.Property(window => window.Id).HasColumnName("id");
        builder.Property(window => window.CourseId).HasColumnName("course_id");
        builder.Property(window => window.GroupName).HasColumnName("group_name").HasMaxLength(50).IsRequired();
        builder.Property(window => window.StartsAt).HasColumnName("starts_at");
        builder.Property(window => window.EndsAt).HasColumnName("ends_at");
        builder.Property(window => window.StartedByUserId).HasColumnName("started_by_user_id");
        builder.Property(window => window.ClosedAt).HasColumnName("closed_at");
        builder.Property(window => window.ClosedByUserId).HasColumnName("closed_by_user_id");
        builder.Property(window => window.CreatedAt).HasColumnName("created_at");

        // Не более одной незакрытой вручную пары на курс и группу (просроченную закрывает «Начать пару»).
        builder.HasIndex(window => new { window.CourseId, window.GroupName })
            .IsUnique()
            .HasFilter("closed_at IS NULL");
        builder.HasOne<Course>().WithMany().HasForeignKey(window => window.CourseId).OnDelete(DeleteBehavior.Cascade);
    }
}
