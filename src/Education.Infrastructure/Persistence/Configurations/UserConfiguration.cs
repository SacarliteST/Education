using Education.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).HasColumnName("id");
        builder.Property(user => user.Login).HasColumnName("login").IsRequired();
        builder.Property(user => user.Password).HasColumnName("password").IsRequired();
        builder.Property(user => user.FirstName).HasColumnName("first_name").IsRequired();
        builder.Property(user => user.LastName).HasColumnName("last_name").IsRequired();
        builder.Property(user => user.MiddleName).HasColumnName("middle_name").IsRequired();
        builder.Property(user => user.RoleId).HasColumnName("role_id");
        builder.Property(user => user.GroupName).HasColumnName("group_name").HasMaxLength(User.GroupNameMaxLength);
        builder.HasIndex(user => user.Login).IsUnique();
        builder.HasIndex(user => user.GroupName);
        builder.HasOne(user => user.Role).WithMany().HasForeignKey(user => user.RoleId);
    }
}


