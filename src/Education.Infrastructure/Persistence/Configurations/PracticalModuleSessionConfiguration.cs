using Education.Domain.Practicals;
using Education.Domain.PracticalModules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class PracticalModuleSessionConfiguration : IEntityTypeConfiguration<PracticalModuleSession>
{
    public void Configure(EntityTypeBuilder<PracticalModuleSession> builder)
    {
        builder.ToTable("PracticalModuleSessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).HasColumnName("id");
        builder.Property(session => session.PracticalTaskId).HasColumnName("practical_task_id");
        builder.Property(session => session.UserId).HasColumnName("user_id");
        builder.Property(session => session.TryNumber).HasColumnName("try_number");
        builder.Property(session => session.Status)
            .HasColumnName("status")
            .HasConversion(state => state.ToWire(), value => ModuleSessionEnums.ParseState(value))
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(session => session.EndReason)
            .HasColumnName("end_reason")
            .HasConversion(
                reason => reason == null ? null : reason.Value.ToWire(),
                value => ModuleSessionEnums.ParseEndReason(value))
            .HasMaxLength(20);
        builder.Property(session => session.SessionKey).HasColumnName("session_key").IsRequired();
        builder.Property(session => session.ReturnUrl).HasColumnName("return_url").IsRequired();
        builder.Property(session => session.StartedAt).HasColumnName("started_at");
        builder.Property(session => session.ExpiresAt).HasColumnName("expires_at");
        builder.Property(session => session.EndedAt).HasColumnName("ended_at");
        builder.Property(session => session.Grade).HasColumnName("grade");
        builder.Property(session => session.CompletionData).HasColumnName("completion_data").HasColumnType("jsonb");
        builder.HasIndex(session => new { session.UserId, session.PracticalTaskId });
        builder.HasOne<Case>()
            .WithMany()
            .HasForeignKey(session => session.PracticalTaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
