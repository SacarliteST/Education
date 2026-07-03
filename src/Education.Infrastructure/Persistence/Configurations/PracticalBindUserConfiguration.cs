using Education.Domain.Practicals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class PracticalBindUserConfiguration : IEntityTypeConfiguration<PracticalBindUser>
{
    public void Configure(EntityTypeBuilder<PracticalBindUser> builder)
    {
        builder.ToTable("PracticalBindUsers");
        builder.HasKey(bind => bind.Id);
        builder.Property(bind => bind.Id).HasColumnName("id");
        builder.Property(bind => bind.PracticalMaterialId).HasColumnName("practical_material_id");
        builder.Property(bind => bind.UserId).HasColumnName("user_id");
        builder.HasOne(bind => bind.PracticalMaterial)
            .WithMany(practical => practical.PracticalBindUsers)
            .HasForeignKey(bind => bind.PracticalMaterialId);
        builder.HasOne(bind => bind.User).WithMany(user => user.PracticalBindUsers).HasForeignKey(bind => bind.UserId);
    }
}


