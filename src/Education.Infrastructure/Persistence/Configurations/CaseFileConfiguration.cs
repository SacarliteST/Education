using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class CaseFileConfiguration : IEntityTypeConfiguration<CaseFile>
{
    public void Configure(EntityTypeBuilder<CaseFile> builder)
    {
        builder.ToTable("CaseFiles");
        builder.HasKey(file => file.Id);
        builder.Property(file => file.Id).HasColumnName("id");
        builder.Property(file => file.Path).HasColumnName("path").IsRequired();
        builder.Property(file => file.CaseId).HasColumnName("case_id");
        builder.Property(file => file.UserId).HasColumnName("user_id");
        builder.Property(file => file.IsAccepted).HasColumnName("is_accepted");
        builder.Property(file => file.Grade).HasColumnName("grade");
        builder.HasOne(file => file.Case).WithMany(task => task.CaseFiles).HasForeignKey(file => file.CaseId);
        builder.HasOne(file => file.User).WithMany(user => user.CaseFiles).HasForeignKey(file => file.UserId);
    }
}
