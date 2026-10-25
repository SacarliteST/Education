using Education.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.Infrastructure.Persistence.Configurations;

/// <summary>Отображение сущности <see cref="Role"/> на таблицу базы данных.</summary>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(role => role.Id);
        builder.Property(role => role.Id).HasColumnName("id");
        builder.Property(role => role.Name).HasColumnName("r_name").IsRequired();
        builder.HasData(
            new Role(RoleIds.Admin, "Администратор"),
            new Role(RoleIds.Teacher, "Преподаватель"),
            new Role(RoleIds.Student, "Студент"));
    }
}


