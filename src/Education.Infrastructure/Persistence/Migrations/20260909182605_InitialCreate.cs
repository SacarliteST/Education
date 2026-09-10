using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PracticalModules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pm_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    practice_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    base_path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    identity_audience = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    configuration = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalModules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    middle_name = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "IdentityUserLinks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    legacy_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLinks", x => x.id);
                    table.ForeignKey(
                        name: "FK_IdentityUserLinks_Users_legacy_user_id",
                        column: x => x.legacy_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseBindUsers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    m_name = table.Column<string>(type: "text", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pm_name = table.Column<string>(type: "text", nullable: false),
                    module_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    time_limit_minutes = table.Column<int>(type: "integer", nullable: true),
                    tries_count = table.Column<int>(type: "integer", nullable: false),
                    percent_for_five = table.Column<double>(type: "double precision", nullable: false),
                    percent_for_four = table.Column<double>(type: "double precision", nullable: false),
                    percent_for_three = table.Column<double>(type: "double precision", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    question_body = table.Column<string>(type: "jsonb", nullable: false),
                    answer = table.Column<string>(type: "jsonb", nullable: false),
                    weight = table.Column<double>(type: "double precision", nullable: false),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tm_name = table.Column<string>(type: "text", nullable: false),
                    lecture_text = table.Column<string>(type: "text", nullable: false),
                    module_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pm_name = table.Column<string>(type: "text", nullable: false),
                    case_text = table.Column<string>(type: "text", nullable: false),
                    practical_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_module_id = table.Column<Guid>(type: "uuid", nullable: true),
                    external_task_ref = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Cases_PracticalModules_practical_module_id",
                        column: x => x.practical_module_id,
                        principalTable: "PracticalModules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    turned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    try_number = table.Column<int>(type: "integer", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: true),
                    max_score = table.Column<double>(type: "double precision", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_material_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.id);
                    table.ForeignKey(
                        name: "FK_TestResults_PracticalMaterials_practical_material_id",
                        column: x => x.practical_material_id,
                        principalTable: "PracticalMaterials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResults_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticalMaterialBindQuestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_material_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    theoretical_material_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    link = table.Column<string>(type: "text", nullable: false),
                    theoretical_material_id = table.Column<Guid>(type: "uuid", nullable: false)
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_accepted = table.Column<bool>(type: "boolean", nullable: false),
                    grade = table.Column<int>(type: "integer", nullable: false)
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
                name: "PracticalModuleSessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    practical_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    try_number = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    end_reason = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    session_key = table.Column<string>(type: "text", nullable: false),
                    return_url = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    grade = table.Column<int>(type: "integer", nullable: true),
                    completion_data = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalModuleSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_PracticalModuleSessions_Cases_practical_task_id",
                        column: x => x.practical_task_id,
                        principalTable: "Cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    answer = table.Column<string>(type: "jsonb", nullable: false),
                    practical_material_bind_question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_result_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.id);
                    table.ForeignKey(
                        name: "FK_Answers_PracticalMaterialBindQuestions_practical_material_b~",
                        column: x => x.practical_material_bind_question_id,
                        principalTable: "PracticalMaterialBindQuestions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_TestResults_test_result_id",
                        column: x => x.test_result_id,
                        principalTable: "TestResults",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseFileComments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cfc_text = table.Column<string>(type: "text", nullable: false),
                    is_generated = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CaseFileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseFileComments", x => x.id);
                    table.ForeignKey(
                        name: "FK_CaseFileComments_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticalTaskEvents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalTaskEvents", x => x.id);
                    table.ForeignKey(
                        name: "FK_PracticalTaskEvents_PracticalModuleSessions_session_id",
                        column: x => x.session_id,
                        principalTable: "PracticalModuleSessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "id", "qt_name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Вопрос с одним ответом" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Вопрос с несколькими ответами" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Вопрос с соотнесением" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Вопрос с вводом ответа" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "id", "r_name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Администратор" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Преподаватель" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Студент" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_practical_material_bind_question_id",
                table: "Answers",
                column: "practical_material_bind_question_id");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_test_result_id",
                table: "Answers",
                column: "test_result_id");

            migrationBuilder.CreateIndex(
                name: "IX_CaseFileComments_CaseFileId",
                table: "CaseFileComments",
                column: "CaseFileId");

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
                name: "IX_Cases_practical_module_id",
                table: "Cases",
                column: "practical_module_id");

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
                name: "IX_IdentityUserLinks_identity_user_id",
                table: "IdentityUserLinks",
                column: "identity_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserLinks_legacy_user_id",
                table: "IdentityUserLinks",
                column: "legacy_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_course_id",
                table: "Modules",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalBindUsers_practical_material_id",
                table: "PracticalBindUsers",
                column: "practical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalBindUsers_user_id",
                table: "PracticalBindUsers",
                column: "user_id");

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
                name: "IX_PracticalModules_slug",
                table: "PracticalModules",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PracticalModuleSessions_practical_task_id",
                table: "PracticalModuleSessions",
                column: "practical_task_id");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalModuleSessions_user_id_practical_task_id",
                table: "PracticalModuleSessions",
                columns: new[] { "user_id", "practical_task_id" });

            migrationBuilder.CreateIndex(
                name: "IX_PracticalTaskEvents_session_id",
                table: "PracticalTaskEvents",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_module_id",
                table: "Questions",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_question_type_id",
                table: "Questions",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_practical_material_id",
                table: "TestResults",
                column: "practical_material_id");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_user_id",
                table: "TestResults",
                column: "user_id");

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
                name: "CaseFileComments");

            migrationBuilder.DropTable(
                name: "CourseBindUsers");

            migrationBuilder.DropTable(
                name: "IdentityUserLinks");

            migrationBuilder.DropTable(
                name: "PracticalBindUsers");

            migrationBuilder.DropTable(
                name: "PracticalTaskEvents");

            migrationBuilder.DropTable(
                name: "TheoreticalMaterialFiles");

            migrationBuilder.DropTable(
                name: "TheoreticalMaterialLinks");

            migrationBuilder.DropTable(
                name: "PracticalMaterialBindQuestions");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "CaseFiles");

            migrationBuilder.DropTable(
                name: "PracticalModuleSessions");

            migrationBuilder.DropTable(
                name: "TheoreticalMaterials");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Cases");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropTable(
                name: "PracticalMaterials");

            migrationBuilder.DropTable(
                name: "PracticalModules");

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
