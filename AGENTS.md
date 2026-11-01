# AGENTS

Рабочие правила для Codex и других автоматических помощников в этом репозитории.

## Репозиторные skills

Перед работой загружать минимальный подходящий набор инструкций:

- [`focused-feature-flow`](.agents/skills/focused-feature-flow/SKILL.md) — экономный цикл реализации, таргетные проверки во время работы и полный финальный прогон;
- [`education-development`](.agents/skills/education-development/SKILL.md) — слои, контракты, PostgreSQL, Kafka и особенности Education;
- [`human-readable-code`](.agents/skills/human-readable-code/SKILL.md) — общий стандарт простого и понятного кода;
- [`csharp-readable-implementation`](.agents/skills/csharp-readable-implementation/SKILL.md) — правила реализации на C#, EF Core и CQRS;
- [`unsandboxed-codegen`](.agents/skills/unsandboxed-codegen/SKILL.md) — обязательный процесс для миграций, OpenAPI и другой генерации кода вне песочницы.

При реализации backend-фичи обычно применяются первые четыре skill. `unsandboxed-codegen` добавляется только при генерации артефактов. Явные правила этого файла имеют приоритет над общими рекомендациями skills.

## Контекст проекта

Это backend системы обучения и тестирования. Новая архитектура находится в `src`, интеграционные тесты - в `tests`, старый проект - в `Education`.

Legacy-папку `Education` не удалять и не переписывать без прямой команды. Она нужна для сверки моделей и поведения.

Frontend не является текущей зоной работ backend-рефакторинга. Если нужно фиксировать frontend-миграцию, лучше писать план в Markdown, а не править frontend-код.

## Архитектура

- `Education.Domain` содержит доменные сущности и правила. Не добавлять зависимости на Application, Infrastructure, Web или Contracts.
- `Education.Application` принимает команды и работает с доменом через интерфейсы сервисов/репозиториев.
- `Education.Contracts` содержит публичные API-контракты и маршруты. Web-маппинг преобразует request в application command.
- `Education.Infrastructure` реализует EF Core, репозитории, файловое хранилище и внешнюю инфраструктуру.
- `Education.Web` содержит Minimal API endpoints, маппинги, validators и регистрацию сервисов.

Endpoint-группы держать разнесенными по папкам. Для каждой функциональной группы предпочтительна структура:

- endpoint group;
- mapping;
- FluentValidation validators.

Маппинги не складывать в один общий файл-полотно.

## API и документация

Публичные контракты и API должны иметь XML-документацию на русском языке. Это важно, потому что по `swagger.json` будут генерироваться статические frontend-клиенты.

Для публичных типов, свойств, методов и интерфейсов добавлять XML-комментарии. `GenerateDocumentationFile` включен на уровне общих props.

Маршруты держать в `Education.Contracts.ApiRoutes`, а не размазывать строками по Web-слою.

## Данные и идентификаторы

В новом backend все идентификаторы доменных сущностей и связанных контрактов должны быть `Guid`.

Не возвращать `long` для идентификаторов, если это не legacy-код. Поля размера файла вроде `Length` или `MaxFileSizeBytes` могут оставаться `long`.

Сиды пользователей, пароли и полноценная identity-логика не относятся к этому проекту. Это задача Identity-сервиса.

## База данных и миграции

Схема `EducationDb` управляется миграциями EF Core в
`src/Education.Infrastructure/Persistence/Migrations`. `EnsureCreatedAsync` больше
не используется (он молча не догоняет схему на непустой БД — был баг `SMK-1`).

- **Dev:** `Program.cs` в `Development` применяет `Database.MigrateAsync()` при
  старте. Дев-БД, ранее созданные `EnsureCreatedAsync` (без
  `__EFMigrationsHistory`), нужно один раз сбросить:
  `docker compose -f ../Backend/compose.yaml down -v` либо
  `DROP DATABASE "EducationDb"` — иначе первая миграция упадёт на уже
  существующих таблицах.
- **Прод/стейджинг:** приложение накатывает миграции при старте, только если в
  конфиге задан `Database:ApplyMigrationsOnStartup=true` (по умолчанию `false`).
  Флаг включают в манифесте развёртывания — тогда `dotnet Education.Web.dll`
  сам догоняет схему на деплое (`MigrateAsync` берёт advisory-lock, несколько
  реплик стартуют безопасно). Альтернатива при выключенном флаге —
  `dotnet ef database update` / migrations bundle отдельным шагом пайплайна.
- **Новая миграция:** `dotnet tool restore` (манифест `.config/dotnet-tools.json`,
  `dotnet-ef` 10.0.4), затем
  `dotnet ef migrations add <Name> --project src/Education.Infrastructure --startup-project src/Education.Web --output-dir Persistence/Migrations`.
- `Roles` и `QuestionTypes` сидятся через `HasData` — их `InsertData` включён в
  `InitialCreate`, отдельного сидера нет.

## Тесты

Все backend-тесты должны быть интеграционными и поднимать реальную PostgreSQL через Testcontainers.

Не добавлять faker-репозитории для проверки сценариев приложения. Проверять реальные EF-маппинги, маршруты, авторизацию и работу с БД.

Перед завершением значимой backend-правки запускать:

```powershell
dotnet format Education.sln --no-restore
dotnet build Education.sln --no-restore --nologo -v:minimal
dotnet test Education.sln --no-restore --nologo -v:minimal
```

Если Docker недоступен, честно указать, что интеграционные тесты не удалось запустить.

## Стиль изменений

- Следовать существующей структуре папок и именованию.
- Не делать широкие рефакторинги без необходимости.
- Не откатывать пользовательские изменения без прямого разрешения.
- Не удалять legacy-проект.
- Не коммитить `Promts`, `bin`, `obj`, Rider cache и другие временные файлы.
- Nullable warning пока можно не считать блокером, если пользователь не попросил иначе.

## Git

Перед коммитом всегда смотреть `git status` и включать только файлы текущей задачи.

Если remote не настроен, не придумывать URL. Сообщить пользователю, что push заблокирован до настройки remote.

Коммиты делать короткими и осмысленными, например:

```text
Add project documentation
```
