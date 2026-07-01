using Education.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Education.DAL.Configs;

internal class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Login)
            .IsUnique();

        builder.HasData(new List<User>()
        {
            new() { Id = 1, Login = "Admin", Password = "wcIksDzZvHtqhtd/XazkAZF2bEhc1V3EjK+ayHMzXW8=", FirstName = "Admin", LastName = "Admin", MiddleName = "Admin", RoleId = 1},
        });
    }
}
