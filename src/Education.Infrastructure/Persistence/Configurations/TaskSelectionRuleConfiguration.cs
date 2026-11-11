using Education.Domain.PracticalModules;
using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="TaskSelectionRule"/> на таблицу базы данных.</summary>
internal sealed class TaskSelectionRuleConfiguration : IEntityTypeConfiguration<TaskSelectionRule>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaskSelectionRule> builder)
    {
        builder.ToTable("TaskSelectionRules");
        builder.HasKey(rule => rule.Id);
        builder.Property(rule => rule.Id).HasColumnName("id");
        builder.Property(rule => rule.PracticalMaterialId).HasColumnName("practical_material_id");
        builder.Property(rule => rule.PracticalModuleId).HasColumnName("practical_module_id");
        builder.HasOne<PracticalModule>()
            .WithMany()
            .HasForeignKey(rule => rule.PracticalModuleId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(rule => rule.TaskCount).HasColumnName("task_count");
        builder.Property(rule => rule.DifficultyLevels).HasColumnName("difficulty_levels").IsRequired();
        builder.Property(rule => rule.TopicRefs).HasColumnName("topic_refs").IsRequired();
        builder.HasOne(rule => rule.PracticalMaterial)
            .WithOne()
            .HasForeignKey<TaskSelectionRule>(rule => rule.PracticalMaterialId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(rule => rule.PracticalMaterialId).IsUnique();
    }
}
