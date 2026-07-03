# Cutover и rollback plan

## Статус legacy-проекта

- Папка `Education` физически сохраняется как legacy reference/archive.
- Новым runtime-хостом считается `src/Education.Web`.
- Cookie authentication, MVC controllers и старые routes не должны попадать в OpenAPI нового хоста.
- Старые migrations, legacy `Users.password`, `Users.role_id`, таблица `Roles` и FK на `Users.id` физически сохраняются до отдельного этапа перепроектирования БД.

## Checklist перед внедрением

- Собран актуальный backup PostgreSQL БД.
- Скопирован backup файлового storage.
- IdentityService развернут и доступен по `Identity:Authority`.
- JWT `Audience`, `RoleClaimType`, `NameClaimType` согласованы между IdentityService и Education API.
- Все старые пользователи связаны с `IdentityUserLinks`.
- `Cors:AllowedOrigins` содержит адрес фронта.
- Фронт отправляет `Authorization: Bearer <accessToken>`.
- Swagger/OpenAPI содержит только новые `/api/v1/*` routes и `/health`.
- Проверены сценарии Student/Teacher/Admin, тестирования, файлов и администрирования профилей.

## Rollback

- Вернуть frontend base URL на прежний legacy host.
- Вернуть запуск legacy-проекта `Education`, если новый host не проходит smoke-check.
- Восстановить PostgreSQL backup, если cutover изменил данные некорректно.
- Восстановить backup файлового storage, если появились ошибки загрузки или скачивания файлов.
- Откатить конфигурацию Identity/JWT/CORS к предыдущей версии.

## Smoke-check после внедрения

- `GET /health` возвращает `200`.
- `GET /openapi/v1.json` доступен в development/staging.
- `POST /api/Auth/Login` возвращает `404` на новом host.
- `GET /api/v1/auth/me` без токена возвращает `401`.
- `GET /api/v1/auth/me` с Bearer-токеном возвращает данные пользователя.
- Student видит назначенные курсы и может начать назначенную практику.
- Teacher видит свои курсы и протоколы своих практических материалов.
- Admin видит локальные учебные профили.
- Скачивание файлов без прав возвращает `403`.
