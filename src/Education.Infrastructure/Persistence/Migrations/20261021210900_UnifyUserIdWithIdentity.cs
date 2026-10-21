using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UnifyUserIdWithIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            // Единый идентификатор: Users.Id становится равным идентификатору пользователя в Identity.
            // Все ссылки на пользователя переписываются в одной транзакции миграции; внешние ключи на время
            // переписывания снимаются и возвращаются (проверка целостности выполняется при возврате).
            migrationBuilder.Sql(@"
DO $$
DECLARE
    r record;
BEGIN
    -- 1. Активность берётся из связи; профиль без связи с Identity остаётся отключённым.
    UPDATE ""Users"" u
    SET is_active = COALESCE((SELECT l.is_active FROM ""IdentityUserLinks"" l WHERE l.legacy_user_id = u.id LIMIT 1), false);

    FOR r IN
        SELECT u.login FROM ""Users"" u
        WHERE NOT EXISTS (SELECT 1 FROM ""IdentityUserLinks"" l WHERE l.legacy_user_id = u.id)
    LOOP
        RAISE NOTICE 'Профиль без связи с Identity оставлен отключённым: %', r.login;
    END LOOP;

    -- 2. Соответствие старый id -> id в Identity.
    CREATE TEMP TABLE _user_id_map ON COMMIT DROP AS
        SELECT l.legacy_user_id AS old_id, l.identity_user_id AS new_id
        FROM ""IdentityUserLinks"" l
        WHERE l.legacy_user_id <> l.identity_user_id;

    IF EXISTS (SELECT 1 FROM _user_id_map m JOIN ""Users"" u ON u.id = m.new_id) THEN
        RAISE EXCEPTION 'Идентификатор из Identity уже занят другим профилем Education — миграция остановлена.';
    END IF;

    -- 3. Снимаем все внешние ключи на Users (связи Identity-таблицы не возвращаются — таблица удаляется ниже).
    CREATE TEMP TABLE _user_fks ON COMMIT DROP AS
        SELECT c.conrelid::regclass::text AS tbl, c.conname AS name, pg_get_constraintdef(c.oid) AS def,
               (c.conrelid <> '""IdentityUserLinks""'::regclass) AS keep
        FROM pg_constraint c
        WHERE c.contype = 'f' AND c.confrelid = '""Users""'::regclass;

    FOR r IN SELECT * FROM _user_fks LOOP
        EXECUTE format('ALTER TABLE %s DROP CONSTRAINT %I', r.tbl, r.name);
    END LOOP;

    -- 4. Переписываем идентификатор и все ссылки на него.
    UPDATE ""Users"" t SET id = m.new_id FROM _user_id_map m WHERE t.id = m.old_id;
    UPDATE ""Courses"" t SET user_id = m.new_id FROM _user_id_map m WHERE t.user_id = m.old_id;
    UPDATE ""CaseFiles"" t SET user_id = m.new_id FROM _user_id_map m WHERE t.user_id = m.old_id;
    UPDATE ""CourseBindUsers"" t SET user_id = m.new_id FROM _user_id_map m WHERE t.user_id = m.old_id;
    UPDATE ""PracticalBindUsers"" t SET user_id = m.new_id FROM _user_id_map m WHERE t.user_id = m.old_id;
    UPDATE ""PracticalModuleSessions"" t SET user_id = m.new_id FROM _user_id_map m WHERE t.user_id = m.old_id;
    UPDATE ""TestResults"" t SET user_id = m.new_id FROM _user_id_map m WHERE t.user_id = m.old_id;

    -- 5. Возвращаем внешние ключи: PostgreSQL проверит, что ссылок на старые идентификаторы не осталось.
    FOR r IN SELECT * FROM _user_fks WHERE keep LOOP
        EXECUTE format('ALTER TABLE %s ADD CONSTRAINT %I %s', r.tbl, r.name, r.def);
    END LOOP;
END
$$;");

            migrationBuilder.DropTable(
                name: "IdentityUserLinks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityUserLinks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    legacy_user_id = table.Column<Guid>(type: "uuid", nullable: false)
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

            // Откат схемы: идентификатор Education остаётся равным идентификатору Identity (прежние старые id
            // восстановить нельзя — только из бэкапа), связь создаётся «сама на себя».
            migrationBuilder.Sql(@"
INSERT INTO ""IdentityUserLinks"" (id, legacy_user_id, identity_user_id, created_at, is_active)
SELECT gen_random_uuid(), u.id, u.id, now(), u.is_active FROM ""Users"" u;");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "Users");
        }
    }
}
