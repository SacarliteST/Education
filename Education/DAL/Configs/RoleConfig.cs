using Education.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.DAL.Configs;

internal class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(new List<Role>()
        {
            new() { Id = 1, Name = "Администратор" },
            new() { Id = 2, Name = "Преподаватель" },
            new() { Id = 3, Name = "Студент" }
        });
    }
}
