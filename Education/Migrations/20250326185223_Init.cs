using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Education.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    qt_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    r_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    middle_name = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_role_id",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.id);
                    table.ForeignKey(
                        name: "FK_Courses_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseBindUsers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseBindUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseBindUsers_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseBindUsers_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    m_name = table.Column<string>(type: "text", nullable: false),
                    course_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.id);
                    table.ForeignKey(
                        name: "FK_Modules_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticalMaterials",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pm_name = table.Column<string>(type: "text", nullable: false),
                    module_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalMaterials", x => x.id);
                    table.ForeignKey(
                        name: "FK_PracticalMaterials_Modules_module_id",
                        column: x => x.module_id,
                        principalTable: "Modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    question_body = table.Column<string>(type: "jsonb", nullable: false),
                    answer = table.Column<string>(type: "jsonb", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    question_type_id = table.Column<long>(type: "bigint", nullable: false),
                    module_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Questions_Modules_module_id",
                        column: x => x.module_id,
                        principalTable: "Modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionTypes_question_type_id",
                        column: x => x.question_type_id,
                        principalTable: "QuestionTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TheoreticalMaterials",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tm_name = table.Column<string>(type: "text", nullable: false),
                    lecture_text = table.Column<string>(type: "text", nullable: false),
                    module_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoreticalMaterials", x => x.id);
                    table.ForeignKey(
                        name: "FK_TheoreticalMaterials_Modules_module_id",
                        column: x => x.module_id,
                        principalTable: "Modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cases",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pm_name = table.Column<string>(type: "text", nullable: false),
                    case_text = table.Column<string>(type: "text", nullable: false),
                    practical_material_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cases", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cases_PracticalMaterials_practical_material_id",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticalMaterialBindQuestions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    practical_material_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalMaterialBindQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_PracticalMaterialBindQuestions_PracticalMaterials_practical~",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticalMaterialBindQuestions_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TheoreticalMaterialFiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    theoretical_material_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoreticalMaterialFiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_TheoreticalMaterialFiles_TheoreticalMaterials_theoretical_m~",
                        column: x => x.theoretical_material_id,
                        principalTable: "TheoreticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TheoreticalMaterialLinks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    link = table.Column<string>(type: "text", nullable: false),
                    theoretical_material_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoreticalMaterialLinks", x => x.id);
                    table.ForeignKey(
                        name: "FK_TheoreticalMaterialLinks_TheoreticalMaterials_theoretical_m~",
                        column: x => x.theoretical_material_id,
                        principalTable: "TheoreticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseFiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    path = table.Column<string>(type: "text", nullable: false),
                    case_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseFiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_CaseFiles_Cases_case_id",
                        column: x => x.case_id,
                        principalTable: "Cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseFiles_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    answer = table.Column<string>(type: "jsonb", nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    question_list_bind_question_id = table.Column<long>(type: "bigint", nullable: false),
                    PracticalMaterialBindQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.id);
                    table.ForeignKey(
                        name: "FK_Answers_PracticalMaterialBindQuestions_PracticalMaterialBin~",
                        column: x => x.PracticalMaterialBindQuestionId,
                        principalTable: "PracticalMaterialBindQuestions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "id", "qt_name" },
                values: new object[,]
                {
                    { 1L, "Вопрос с одним ответом" },
                    { 2L, "Вопрос с несколькими ответами" },
                    { 3L, "Вопрос с соотнесением" },
                    { 4L, "Вопрос с вводом ответа" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "id", "r_name" },
                values: new object[,]
                {
                    { 1L, "Администратор" },
                    { 2L, "Преподаватель" },
                    { 3L, "Студент" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id", "first_name", "last_name", "login", "middle_name", "password", "role_id" },
                values: new object[] { 1L, "Admin", "Admin", "Admin", "Admin", "wcIksDzZvHtqhtd/XazkAZF2bEhc1V3EjK+ayHMzXW8=", 1L });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_PracticalMaterialBindQuestionId",
                table: "Answers",
                column: "PracticalMaterialBindQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_user_id",
                table: "Answers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CaseFiles_case_id",
                table: "CaseFiles",
                column: "case_id");

            migrationBuilder.CreateIndex(
                name: "IX_CaseFiles_user_id",
                table: "CaseFiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_practical_material_id",
                table: "Cases",
                column: "practical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseBindUsers_course_id",
                table: "CourseBindUsers",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseBindUsers_user_id",
                table: "CourseBindUsers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_user_id",
                table: "Courses",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_course_id",
                table: "Modules",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalMaterialBindQuestions_practical_material_id",
                table: "PracticalMaterialBindQuestions",
                column: "practical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalMaterialBindQuestions_question_id",
                table: "PracticalMaterialBindQuestions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalMaterials_module_id",
                table: "PracticalMaterials",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_module_id",
                table: "Questions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_question_type_id",
                table: "Questions",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_TheoreticalMaterialFiles_theoretical_material_id",
                table: "TheoreticalMaterialFiles",
                column: "theoretical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_TheoreticalMaterialLinks_theoretical_material_id",
                table: "TheoreticalMaterialLinks",
                column: "theoretical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_TheoreticalMaterials_module_id",
                table: "TheoreticalMaterials",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_login",
                table: "Users",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_role_id",
                table: "Users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "CaseFiles");

            migrationBuilder.DropTable(
                name: "CourseBindUsers");

            migrationBuilder.DropTable(
                name: "TheoreticalMaterialFiles");

            migrationBuilder.DropTable(
                name: "TheoreticalMaterialLinks");

            migrationBuilder.DropTable(
                name: "PracticalMaterialBindQuestions");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "TheoreticalMaterials");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "PracticalMaterials");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
