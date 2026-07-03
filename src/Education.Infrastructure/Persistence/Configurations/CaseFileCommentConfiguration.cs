using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class CaseFileCommentConfiguration : IEntityTypeConfiguration<CaseFileComment>
{
    public void Configure(EntityTypeBuilder<CaseFileComment> builder)
    {
        builder.ToTable("CaseFileComments");
        builder.HasKey(comment => comment.Id);
        builder.Property(comment => comment.Id).HasColumnName("id");
        builder.Property(comment => comment.Text).HasColumnName("cfc_text").IsRequired();
        builder.Property(comment => comment.IsGenerated).HasColumnName("is_generated");
        builder.Property(comment => comment.Created).HasColumnName("created");
        builder.HasOne(comment => comment.CaseFile)
            .WithMany(file => file.Comments)
            .HasForeignKey(comment => comment.CaseFileId);
    }
}


