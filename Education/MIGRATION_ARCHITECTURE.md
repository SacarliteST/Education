# Education Migration Architecture

## Purpose

This document records the current legacy system inventory and the target migration direction for moving `Education` from a single ASP.NET Core MVC project to Clean Architecture with Minimal API and external JWT authentication through `IdentityService`.

The first migration rule is to keep the existing PostgreSQL schema as a legacy contract. Existing tables, columns, key types, foreign keys, and EF migrations must not be renamed, rewritten, or removed during the early stages.

## Product Context From The Thesis

The system is an educational complex for the course "Legal Foundations of Informatics". It supports three user roles: administrator, teacher, and student.

The main learning flow is:

1. A student signs in.
2. The student selects an assigned course and module.
3. The student studies theoretical materials and attachments.
4. The student completes practical tasks and uploads files for teacher review.
5. The student passes a weighted test.
6. The system builds a test protocol and calculates a grade.
7. The final practical grade combines the best test grade and the teacher's grade for submitted tasks.

The test model uses weighted questions and weighted answers. The current implementation maps that to `Question.Weight`, JSON answer bodies, `ScoreHelper.GetScore`, `TestResult.Score`, `TestResult.MaxScore`, and configurable thresholds on `PracticalMaterial`.

## Current Technical Shape

- One ASP.NET Core project: `Education`.
- Runtime target: `.NET 9`.
- HTTP layer: MVC controllers.
- Persistence: EF Core with Npgsql and PostgreSQL.
- API documentation: OpenAPI and Scalar in development.
- Frontend: static bundled files in `wwwroot`.
- Authentication: cookie auth.
- Authorization: role strings from local `Roles` table.
- Identity storage: local `Users` table with SHA-256 password hash in `password`.
- Migrations: existing EF migrations are part of the legacy schema and must be preserved.
- Startup issue: `ApplicationContext` calls `Database.Migrate()` in the DbContext constructor.

## Current Bounded Contexts And Features

| Context / feature | Current code | Main responsibilities |
| --- | --- | --- |
| Authentication and local identity | `AuthController`, `HashHelper`, `Users`, `Roles`, `RoleConstants` | Login, logout, cookie creation, local password verification, role claims. |
| Admin user management | `UserController` | Create/delete users, list users, list roles. |
| Shared learning content reads | `SharedController` | Read modules, theory text, theory files, theory links, task text. |
| Student learning flow | `StudentController` | Assigned courses/practicals, theory list, task list, task upload, test start/upload, protocols, grade calculation. |
| Teacher authoring and review | `TeacherController` | Courses, modules, theory, practicals, questions, tests, student assignments, task file review, protocols. |
| File storage | `FilesController`, `FileHelper`, `Education/Files` | Save, delete, and download uploaded files from local disk. |
| Testing and grading | `ScoreHelper`, `Question*` models, `Answer*` models, `TestResults`, `Answers` | Score single-choice, multiple-choice, match, and short-answer questions; build protocols; calculate grades. |
| Legacy data access | `ApplicationContext`, `DAL/Models`, `DAL/Configs`, `Migrations` | EF entity model over the existing database. |

## Current Endpoints And Target Features

All controller routes except file download use `api/[controller]/[action]`.

| Old endpoint | Current access | Target feature |
| --- | --- | --- |
| `POST /api/Auth/Login` | Anonymous | `Auth` compatibility or frontend cutover to `IdentityService`; eventually removed from new auth flow. |
| `GET /api/Auth/IsSignedIn` | Anonymous | `Auth` session/status compatibility; replace with JWT-aware `/me` style endpoint if needed. |
| `GET /api/Auth/Logout` | Any authenticated user | `Auth` compatibility; JWT logout should be handled by client/IdentityService refresh flow. |
| `GET /Files/{fileName}` | Admin, Teacher, Student | `Files` download endpoint with authorization and path validation. |
| `GET /api/Shared/GetModules` | Admin, Teacher, Student | `Courses` / `Modules` read feature. |
| `GET /api/Shared/GetTheoryText` | Admin, Teacher, Student | `Materials` theory read feature. |
| `GET /api/Shared/GetTheoryDocs` | Admin, Teacher, Student | `Materials` theory file read feature. |
| `GET /api/Shared/GetTheoryLinks` | Admin, Teacher, Student | `Materials` theory link read feature. |
| `GET /api/Shared/GetTaskText` | Admin, Teacher, Student | `Practicals` task read feature. |
| `GET /api/Student/GetCourses` | Student | `StudentCourses` assigned course list. |
| `GET /api/Student/GetPracticals` | Student | `StudentPracticals` assigned practical list. |
| `GET /api/Student/GetTheories` | Student | `StudentMaterials` theory list. |
| `GET /api/Student/GetTasks` | Student | `StudentTasks` task list with submission status. |
| `GET /api/Student/GetTaskFile` | Student | `StudentTaskFiles` current submission with comments. |
| `PUT /api/Student/UploadTaskFile` | Student | `StudentTaskFiles` upload or replace submission. |
| `GET /api/Student/GetTestStatus` | Student | `StudentTesting` test attempt status. |
| `PUT /api/Student/StartTest` | Student | `StudentTesting` start or resume attempt. |
| `GET /api/Student/GetPracticalQuestions` | Student | `StudentTesting` question delivery. |
| `POST /api/Student/UploadTest` | Student | `StudentTesting` submit answers and calculate score. |
| `GET /api/Student/GetProtocols` | Student | `StudentProtocols` list completed attempts. |
| `GET /api/Student/GetProtocol` | Student | `StudentProtocols` detailed attempt protocol. |
| `GET /api/Student/GetPracticalGrade` | Student | `StudentGrades` combined test/task grade. |
| `GET /api/Teacher/GetCourses` | Teacher | `TeacherCourses` owned course list. |
| `POST /api/Teacher/CreateCourse` | Teacher | `TeacherCourses` create course. |
| `DELETE /api/Teacher/DeleteCourse` | Teacher | `TeacherCourses` delete course. |
| `GET /api/Teacher/GetPracticals` | Teacher | `TeacherPracticals` practical list by module. |
| `POST /api/Teacher/CreatePractical` | Teacher | `TeacherPracticals` create practical. |
| `DELETE /api/Teacher/DeletePractical` | Teacher | `TeacherPracticals` delete practical. |
| `PUT /api/Teacher/MakePracticalPublic` | Teacher | `TeacherPracticals` publish practical. |
| `POST /api/Teacher/CreateModule` | Teacher | `TeacherModules` create module. |
| `DELETE /api/Teacher/DeleteModule` | Teacher | `TeacherModules` delete module. |
| `GET /api/Teacher/GetTheories` | Teacher | `TeacherMaterials` theory list. |
| `POST /api/Teacher/CreateTheory` | Teacher | `TeacherMaterials` create theory. |
| `PUT /api/Teacher/UpdateTheoryTitle` | Teacher | `TeacherMaterials` update theory title. |
| `PUT /api/Teacher/UpdateTheoryText` | Teacher | `TeacherMaterials` update theory text. |
| `POST /api/Teacher/CreateTheoryDocument` | Teacher | `TeacherMaterials` attach document. |
| `POST /api/Teacher/CreateTheoryLink` | Teacher | `TeacherMaterials` attach link. |
| `DELETE /api/Teacher/DeleteTheoryMaterial` | Teacher | `TeacherMaterials` delete theory. |
| `DELETE /api/Teacher/DeleteTheoryLink` | Teacher | `TeacherMaterials` delete theory link. |
| `DELETE /api/Teacher/DeleteTheoryDoc` | Teacher | `TeacherMaterials` delete theory document and file. |
| `GET /api/Teacher/GetTasks` | Teacher | `TeacherTasks` task list. |
| `POST /api/Teacher/CreateTask` | Teacher | `TeacherTasks` create task. |
| `PUT /api/Teacher/UpdateTaskText` | Teacher | `TeacherTasks` update task text. |
| `DELETE /api/Teacher/DeleteTask` | Teacher | `TeacherTasks` delete task. |
| `GET /api/Teacher/GetTaskFiles` | Teacher | `TeacherTaskReview` submissions by task. |
| `GET /api/Teacher/GetPracticalTaskFiles` | Teacher | `TeacherTaskReview` submissions by practical. |
| `POST /api/Teacher/AddTaskFileComment` | Teacher | `TeacherTaskReview` add review comment. |
| `PUT /api/Teacher/AcceptTaskFile` | Teacher | `TeacherTaskReview` accept and grade task file. |
| `GET /api/Teacher/GetQuestions` | Teacher | `TeacherQuestions` question list. |
| `POST /api/Teacher/CreateQuestion` | Teacher | `TeacherQuestions` create weighted question. |
| `PUT /api/Teacher/UpdateQuestion` | Teacher | `TeacherQuestions` update weighted question. |
| `DELETE /api/Teacher/DeleteQuestion` | Teacher | `TeacherQuestions` delete question. |
| `POST /api/Teacher/CreateTest` | Teacher | `TeacherTests` bind questions to practical. |
| `GET /api/Teacher/GetMakePracticalQuestions` | Teacher | `TeacherTests` edit question selection and thresholds. |
| `PUT /api/Teacher/UpdatePracticalMaterialQuestions` | Teacher | `TeacherTests` update selected questions, tries, thresholds. |
| `GET /api/Teacher/GetStudents` | Teacher | `TeacherAssignments` students for course assignment. |
| `GET /api/Teacher/GetPracticalStudents` | Teacher | `TeacherAssignments` students for practical assignment. |
| `PUT /api/Teacher/UpdateCourseStudents` | Teacher | `TeacherAssignments` update course students. |
| `PUT /api/Teacher/UpdatePracticalStudents` | Teacher | `TeacherAssignments` update practical students. |
| `GET /api/Teacher/GetUserProtocols` | Teacher | `TeacherProtocols` student protocol summary. |
| `GET /api/Teacher/GetTestProtocol` | Teacher | `TeacherProtocols` detailed test protocol. |
| `POST /api/User/CreateUser` | Admin | `AdminUsers` create local profile and, later, link to external identity. |
| `GET /api/User/CanDeleteUser` | Admin | `AdminUsers` delete eligibility. |
| `DELETE /api/User/DeleteUser` | Admin | `AdminUsers` delete local profile when allowed. |
| `GET /api/User/GetRoles` | Admin | `AdminUsers` legacy role list; eventually replaced by `IdentityService` roles. |
| `GET /api/User/GetAllUsers` | Admin | `AdminUsers` list local profiles. |

## Current Tables And Relationships

| Table / entity | Key fields | User references and notes |
| --- | --- | --- |
| `Users` / `User` | `id`, `login`, `password`, `first_name`, `last_name`, `middle_name`, `role_id` | Local identity table. Keep as local profile table during migration. |
| `Roles` / `Role` | `id`, `r_name` | Local legacy roles. Current role constants are mojibake strings, while target roles are `Admin`, `Teacher`, `Student`. |
| `Courses` / `Course` | `id`, `c_name`, `description`, `date`, `user_id` | `user_id` points to teacher/owner in `Users`. |
| `CourseBindUsers` / `CourseBindUser` | `id`, `course_id`, `user_id` | Student-course assignment. |
| `Modules` / `Module` | `id`, `m_name`, `course_id` | Course sections. |
| `TheoreticalMaterials` / `TheoreticalMaterial` | `id`, `tm_name`, `lecture_text`, `module_id` | Theory content. |
| `TheoreticalMaterialFiles` / `TheoreticalMaterialFile` | `id`, `description`, `path`, `theoretical_material_id` | File attachments for theory. |
| `TheoreticalMaterialLinks` / `TheoreticalMaterialLink` | `id`, `description`, `link`, `theoretical_material_id` | External links for theory. |
| `PracticalMaterials` / `PracticalMaterial` | `id`, `pm_name`, `module_id`, `is_public`, `tries_count`, grading thresholds | Practical block with test settings. |
| `PracticalBindUsers` / `PracticalBindUser` | `id`, `practical_material_id`, `user_id` | Student-practical assignment. |
| `Cases` / `Case` | `id`, `pm_name`, `case_text`, `practical_material_id` | Practical tasks. |
| `CaseFiles` / `CaseFile` | `id`, `path`, `case_id`, `user_id`, `is_accepted`, `grade` | Student task submission. |
| `CaseFileComments` / `CaseFileComment` | `id`, `cfc_text`, `is_generated`, `created`, `CaseFileId` | Comments and generated status messages for submitted files. |
| `Questions` / `Question` | `id`, `question_text`, `question_body`, `answer`, `weight`, `question_type_id`, `module_id` | Weighted test questions, with JSON body and answer. |
| `QuestionTypes` / `QuestionType` | `id`, `qt_name` | Seeded question types. |
| `PracticalMaterialBindQuestions` / `PracticalMaterialBindQuestion` | `id`, `question_id`, `practical_material_id` | Test question selection for practical material. |
| `TestResults` / `TestResult` | `id`, dates, `try_number`, `is_completed`, `score`, `max_score`, `user_id`, `practical_material_id` | Student test attempt and protocol root. |
| `Answers` / `Answer` | `id`, `answer`, `practical_material_bind_question_id`, `test_result_id` | Serialized protocol answer details. `UserId` was removed by a later migration. |

All current `UserId` relationships use local `long` IDs. The most important references are:

- `Courses.UserId`: teacher/owner.
- `CourseBindUsers.UserId`: assigned student.
- `PracticalBindUsers.UserId`: assigned student.
- `CaseFiles.UserId`: student submission owner.
- `TestResults.UserId`: student test attempt owner.

## Local Identity Usage

Local identity is used in these places:

- `AuthController.Login` hashes the request password with `HashHelper.GetSha256Hash`, looks up `Users` by `Login` and `Password`, includes `Role`, and creates a cookie with `ClaimTypes.Name` and `ClaimTypes.Role`.
- `AuthController.Logout` clears the cookie.
- `Program.cs` configures cookie authentication with `/login` and `/accessdenied`.
- `UserController.CreateUser` creates local users and stores SHA-256 password hashes.
- `UserController.GetRoles` exposes local roles.
- `StudentController` repeatedly resolves the current local `User.Id` from `User.Identity.Name`.
- `TeacherController.GetCourses` and `CreateCourse` resolve teacher ownership from `User.Identity.Name`.
- Authorization attributes use local role constants from `RoleConstants`.

## Business Logic Mixed With HTTP And EF

The current controllers combine request handling, authorization assumptions, EF queries, business decisions, file operations, and response shaping.

High-priority examples:

- `StudentController.UploadTest` calculates test score, writes answers, completes the attempt, and returns protocol summary.
- `StudentController.GetPracticalGrade` and `TeacherController.GetUserGrade` duplicate grade aggregation logic.
- `StudentController.UploadTaskFile` saves/deletes files, creates or updates `CaseFile`, creates generated comments, and returns UI DTOs.
- `TeacherController.UpdateCourseStudents`, `UpdatePracticalStudents`, and `UpdatePracticalMaterialQuestions` implement collection diff logic directly in endpoints.
- `TeacherController.DeleteTheoryDoc` deletes a physical file and database row in one controller action.
- `ScoreHelper` contains domain scoring rules but currently depends on EF entities and web-facing JSON models.
- `ApplicationContext` performs migrations in its constructor, mixing DbContext creation with deployment behavior.

## Target Project Structure

The target solution should be split gradually into these projects:

| Project | Responsibility |
| --- | --- |
| `Education.Domain` | Domain entities/value objects and pure domain rules such as scoring concepts, grades, attempts, assignments, and file submission state. No EF, ASP.NET, or external service references. |
| `Education.Application` | Use cases, service interfaces, authorization-facing current-user abstractions, transaction boundaries, DTO orchestration, validation rules that are not transport-specific. Depends on `Domain`. |
| `Education.Contracts` | HTTP request/response contracts shared by `Web` and clients/tests. Should preserve legacy contracts until an explicit frontend migration step changes them. |
| `Education.Infrastructure` | EF Core DbContext, legacy entity mappings, repositories/query services, file storage adapter, IdentityService integration adapters, migrations. Depends on `Application` and `Domain`. |
| `Education.Web` | ASP.NET Core host, Minimal API endpoint groups, authentication/authorization setup, OpenAPI, static frontend serving during transition. Depends on `Application`, `Contracts`, and `Infrastructure`. |
| `Education.Tests` | Integration and behavior tests for endpoint contracts, auth behavior, scoring, assignments, file flows, and migration safety. |

## Dependency Rules

- `Domain` depends on nothing in the solution.
- `Application` depends on `Domain`.
- `Contracts` should remain dependency-light and must not depend on EF or ASP.NET hosting.
- `Infrastructure` depends inward on `Application` and `Domain`; it implements interfaces declared by `Application`.
- `Web` composes the application and maps HTTP endpoints to application use cases.
- Tests may reference public surfaces from any project as needed, but integration tests should exercise the running web boundary.
- No layer except `Infrastructure` should know EF Core mapping details or legacy column names.
- No layer except `Web` should know Minimal API route mechanics, HTTP status code construction, or authentication middleware.

## IdentityService And Local User Mapping Strategy

`IdentityService` issues JWT RS256 tokens and owns authentication, refresh tokens, JWKS, and role claims (`Student`, `Teacher`, `Admin`). `Education` should stop validating local passwords in the new auth flow.

The new `Education.Web` target host is configured for JWT Bearer authentication through `Identity:Authority`, `Identity:Audience`, and `Jwt:*` settings. It defines role policies `AdminOnly`, `TeacherOnly`, `StudentOnly`, and `AuthenticatedEducationUser`, plus compatibility endpoints under `/api/v1/auth`. The legacy MVC `AuthController` remains only in the old project while that project is still used as the behavior reference.

The legacy schema still uses `long UserId` foreign keys, so the migration needs a bridge from external `Guid userId` to local `Users.id`.

Recommended staged strategy:

1. Add an application abstraction such as `ICurrentUser` exposing external identity ID, login/name, and roles from JWT claims.
2. Keep `Users` as the local profile table because existing FK references require `Users.id`.
3. Add a separate additive mapping table, `IdentityUserLinks`, only when code needs persistent mapping:
   - `id int8`.
   - `legacy_user_id int8` referencing `Users.id` with unique index.
   - `identity_user_id uuid` with unique index.
   - `created_at timestamptz`.
   - `is_active bool`.
4. Resolve `Guid identityUserId -> long legacyUserId` through `IEducationUserResolver`.
5. Do not replace existing FKs in `Courses`, `CourseBindUsers`, `PracticalBindUsers`, `CaseFiles`, or `TestResults` during the first stages.
6. Keep local `Users.role_id` and `Roles` physically present, but stop using them as the source of authorization once JWT roles are enabled.
7. Keep a compatibility path for existing records: admin migration or first-login linking should create mappings from known legacy logins to external users.

This keeps the database stable while allowing new code to trust external identity.

Prompt 04 adds the application-level `IEducationUserResolver` and `IUserProfileRepository` contracts. The additive SQL draft is stored in `docs/migration/identity-user-links-draft.sql`; it creates `IdentityUserLinks`, describes the backfill from IdentityService exports, and includes validation queries for unmapped users, duplicate mappings, and existing relations through `Users.id`.

## Migration Order Without Big Bang Rewrite

The solution now contains the new `src/*` and `tests/*` skeleton while the old `Education` project remains in the solution as the source of current runtime behavior. New features should move into the skeleton gradually, with old controllers used as the behavior reference until a feature is migrated and covered by integration tests.

1. Freeze current behavior with integration tests around login compatibility, student flows, teacher flows, admin user listing, scoring, and file operations.
2. Create the solution skeleton with the target projects while keeping the old project runnable.
3. Move contracts into `Education.Contracts` without changing JSON shapes.
4. Introduce `Application` services behind existing controllers or new Minimal API endpoints, one feature at a time.
5. Move EF and file storage behind infrastructure interfaces while preserving the current schema and migrations.
6. Add JWT Bearer authentication against `IdentityService`; initially run it beside cookie auth if needed for frontend transition.
7. Add the local/external user mapping table only as an additive migration when the identity bridge is implemented.
8. Migrate endpoint groups from MVC controllers to Minimal API feature groups.
9. Move frontend calls to the new contracts only after backend compatibility is tested.
10. Remove cookie/local-password flow after the frontend and admin migration process no longer depend on it.
11. Move deployment-time migration execution out of the DbContext constructor.
12. Clean up legacy controllers and helpers after equivalent tests pass on the new endpoints.

## Legacy Schema Preservation Rules

- Do not rename tables or columns.
- Do not change key types.
- Do not rewrite existing migrations.
- Do not remove `Users`, `Roles`, `role_id`, `password`, or current FK columns during the early migration.
- Prefer additive migrations.
- Prefer a separate identity mapping table over changing existing foreign keys.
- Keep JSON columns (`question_body`, `answer`, protocol answer JSON) compatible until contracts are explicitly versioned.
- Preserve current route and response contracts until the frontend migration prompt changes them.

## Risks

- Current role constants are stored as mojibake strings, while the target role names are `Admin`, `Teacher`, and `Student`.
- `Database.Migrate()` in the DbContext constructor can run unexpectedly during tests, design-time tooling, or service startup.
- Local file paths are built from user-visible filenames and should be hardened before exposing new file endpoints.
- Controllers do not consistently verify ownership. Some student protocol endpoints query by `testResultId` without checking that the attempt belongs to the current user.
- Some collection update endpoints mutate request lists while calculating diffs.
- Several queries rely on `.Last()` over comments and may fail when a file has no comments.
- Test scoring deserializes JSON dynamically and can fail at runtime if stored JSON shape changes.
- Existing frontend assets are bundled and may hide undocumented API contract expectations.
- There is no visible test project yet, so regression protection must be added before behavioral migration.

## Open Questions

- Which claim from `IdentityService` is the canonical external user ID: `sub`, `userId`, or another claim?
- Does `IdentityService` expose profile data needed to create or link local `Users` rows?
- Should admin user creation create users in `IdentityService`, local profile rows, or only local mappings?
- How should old local users be matched to external users: login, email, import file, or manual admin linking?
- Should file storage remain local disk or move to object storage during the file API migration?
- Which current endpoints are still used by the static frontend bundle?
- Should the API keep the exact legacy route names during Minimal API migration or introduce versioned routes?

## Integration Tests To Create

Because this stage is analytical and does not change runtime behavior, no new behavior tests were added here. The next code stages should create integration tests for:

- Anonymous login compatibility against seeded local admin while cookie auth still exists.
- JWT-authenticated request with `Student`, `Teacher`, and `Admin` roles once IdentityService integration starts.
- Student can list only assigned courses and practical materials.
- Student cannot read another student's test protocol or task file.
- Student can start a test, submit answers, and receive score, max score, try number, and grade.
- Weighted scoring for single choice, multiple choice, matching, and short answer.
- Teacher can create course/module/theory/practical/question/test without changing legacy response shapes.
- Teacher can assign students to courses and practical materials.
- Teacher can accept task files and add comments.
- Admin can list users and create a local profile during the compatibility period.
- File upload/download preserves public file names and blocks path traversal.
- Application startup does not run database migration from `DbContext` construction after that behavior is moved.
