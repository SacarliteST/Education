# Стратегия тестирования миграции

## Цель

Зафиксировать старое критичное поведение системы тестирования до удаления legacy-кода и дать единый шаблон для новых регрессионных тестов в следующих промтах.

## Уровни тестирования

| Уровень | Где размещать | Что проверять |
| --- | --- | --- |
| Domain regression tests | `tests/Education.Tests/*Domain*` | Правила оценивания, лимиты попыток, инварианты сущностей на сущностях, загруженных из реальной тестовой БД. |
| Application regression tests | `tests/Education.Tests/*Resolver*` и будущие service tests | Маппинг identity user -> legacy user, сохранение старых связей, ошибки доступа через реальные репозитории. |
| API integration tests | `tests/Education.Tests/{Courses,Practicals,Files,AdminProfiles}` | REST-контракты, политики ролей, доступ к данным через реальную PostgreSQL БД. |
| Frontend contract tests | `tests/Education.Tests/FrontendContracts` | Формы ответов и маршруты, от которых зависит фронт после перехода на JWT. |
| Smoke tests | `tests/Education.Tests/WebSmokeTests.cs` | Готовность тестовой инфраструктуры: health, auth, Testcontainers, seed data. |

## Тестовая инфраструктура

- `TestWebApplicationFactory` поднимает приложение через `WebApplicationFactory<Program>`.
- PostgreSQL запускается через `Testcontainers.PostgreSql`.
- `TestSeedDataBuilder` создаёт предсказуемый набор пользователей, курсов, модулей, практик, заданий, файлов и результатов тестов.
- `TestSeedSnapshot` отдаёт ожидаемые id seeded-сущностей для новых тестов.
- `TestAuthHandler` имитирует JWT claims и поддерживает оба тестовых заголовка:
  - `Test Student`
  - `Bearer Student`
- `TestAuthFixtures` содержит готовые наборы ролей Student/Teacher/Admin.

## Уже покрытые сценарии

| Область | Покрытие |
| --- | --- |
| Authentication/authorization | 401 без токена, 403 при неверной роли, Student/Teacher/Admin policies, Bearer contract. |
| Course/module access | Преподаватель видит свои курсы, студент видит назначенный курс, shared endpoints требуют авторизацию. |
| Practical/test flow | Старт теста, отправка ответов, расчёт результата, лимит попыток, доступ преподавателя к протоколам. |
| Files | Upload, download, запрет path traversal, запрет доступа к чужим файлам. |
| Migration | Identity user link -> legacy user, сохранение legacy-связей пользователя, admin profile link/deactivate. |
| Frontend contracts | `/api/v1/auth/me`, protected Education request with Bearer, 401/403, старые routes не обслуживаются. |

## Правила добавления новых регрессионных тестов

- Все тесты должны использовать `TestWebApplicationFactory` и реальную PostgreSQL БД через Testcontainers.
- Не использовать fake repositories/current user и не создавать доменные сущности вручную в тесте, если сценарий можно проверить через seeded data.
- Для роли использовать `client.AuthenticateWithBearer(TestAuthFixtures.StudentRoles)` или аналогичные fixtures.
- Для seeded id брать `factory.Seed`, если тесту важна конкретная сущность.
- Новые frontend-зависимые response contracts добавлять в `FrontendContracts`.
- Если frontend временно зависит от старого route, перед удалением старого кода добавить тест старый route -> новый route или старый route -> `404`, если migration уже завершён.

## Остаточные риски

- XML documentation warnings в тестовом проекте пока не являются ошибкой сборки.
- Nullable warnings пока сознательно игнорируются.
- Для полного переписывания фронта ещё нужны backend endpoints, перечисленные в `FRONTEND_MIGRATION.md`.
