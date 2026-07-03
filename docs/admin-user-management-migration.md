# Admin user management migration note

Education no longer creates passwords or assigns authorization roles for new users.

The legacy `Users`, `Roles`, `Users.Password`, and `Users.RoleId` schema stays in place during migration so existing relations keep working. New administrative endpoints create a local educational profile and link it to an external `IdentityUserId` through `IdentityUserLinks`. Passwords and role membership remain owned by IdentityService.

For existing users, create or backfill `IdentityUserLinks` records that map each legacy `Users.id` to the corresponding IdentityService user id. After the link is active, Education resolves the current educational profile from the authenticated identity token and does not use the local `Roles` table for endpoint authorization.

New profiles still receive a technical legacy `RoleId` only to satisfy the existing foreign key during the migration period. This value is not accepted from API requests and is not used for authorization.
