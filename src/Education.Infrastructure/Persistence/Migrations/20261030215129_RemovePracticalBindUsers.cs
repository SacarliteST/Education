using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePracticalBindUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticalBindUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PracticalBindUsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalBindUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_PracticalBindUsers_PracticalMaterials_practical_material_id",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticalBindUsers_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticalBindUsers_practical_material_id",
                table: "PracticalBindUsers",
                column: "practical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalBindUsers_user_id",
                table: "PracticalBindUsers",
                column: "user_id");
        }
    }
}
