using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="PracticalTaskAssignment"/> на таблицу базы данных.</summary>
internal sealed class PracticalTaskAssignmentConfiguration : IEntityTypeConfiguration<PracticalTaskAssignment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PracticalTaskAssignment> builder)
    {
        builder.ToTable("PracticalTaskAssignments");
        builder.HasKey(assignment => assignment.Id);
        builder.Property(assignment => assignment.Id).HasColumnName("id");
        builder.Property(assignment => assignment.PracticalMaterialId).HasColumnName("practical_material_id");
        builder.Property(assignment => assignment.StudentId).HasColumnName("student_id");
        builder.Property(assignment => assignment.ClassWindowId).HasColumnName("class_window_id");
        builder.Property(assignment => assignment.ExternalTaskRef).HasColumnName("external_task_ref").HasMaxLength(200).IsRequired();
        builder.Property(assignment => assignment.SortOrder).HasColumnName("sort_order");
        builder.Property(assignment => assignment.CreatedAt).HasColumnName("created_at");
        builder.HasOne(assignment => assignment.PracticalMaterial)
            .WithMany()
            .HasForeignKey(assignment => assignment.PracticalMaterialId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(assignment => assignment.ClassWindow)
            .WithMany()
            .HasForeignKey(assignment => assignment.ClassWindowId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(assignment => new
        {
            assignment.PracticalMaterialId,
            assignment.StudentId,
            assignment.ClassWindowId,
            assignment.ExternalTaskRef
        })
            .IsUnique();
        builder.HasIndex(assignment => new { assignment.PracticalMaterialId, assignment.ClassWindowId });
    }
}
