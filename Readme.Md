# Education

Backend системы обучения и тестирования, перенесенный из бакалаврского legacy-проекта на многослойную архитектуру и .NET 10.

## Что внутри

- `src/Education.Domain` - доменные сущности и правила: курсы, модули, теория, практики, вопросы, результаты тестов.
- `src/Education.Application` - сценарии приложения, команды, интерфейсы сервисов и репозиториев.
- `src/Education.Infrastructure` - EF Core, PostgreSQL, репозитории, хранение файлов и техническая инфраструктура.
- `src/Education.Contracts` - публичные API-контракты, маршруты, request/response модели и XML-документация для генерации клиентов.
- `src/Education.Web` - Minimal API, endpoint groups, FluentValidation, авторизация, OpenAPI и Scalar.
- `tests/Education.Tests` - интеграционные тесты с реальной PostgreSQL через Testcontainers.
- `docs` - планы миграции, регрессионная стратегия и заметки по cutover.
- `Education` - legacy-проект. Он оставлен для сверки поведения и сущностей, удалять его сейчас не нужно.

## Основные решения

- Целевая платформа: `.NET 10`.
- Идентификаторы новых доменных сущностей и контрактов: `Guid`.
- База данных: PostgreSQL.
- Доступ к данным: EF Core + репозитории.
- Валидация Web-запросов: FluentValidation.
- Документация API: OpenAPI/Scalar, XML-документация включена через `GenerateDocumentationFile`.
- Тесты: только интеграционные, с поднятием настоящей БД через Testcontainers.
- Identity-логика вынесена за границы этого backend-проекта; сиды пользователей, пароли и полноценное управление учетками здесь не развивать.

## Требования

- .NET SDK 10.
- Docker Desktop или совместимый Docker runtime для интеграционных тестов.
- PostgreSQL для локального запуска приложения.

## Быстрый старт

Восстановить зависимости:

```powershell
dotnet restore Education.sln
```

Собрать решение:

```powershell
dotnet build Education.sln --no-restore
```

Запустить тесты:

```powershell
dotnet test Education.sln --no-restore
```

Отформатировать код:

```powershell
dotnet format Education.sln --no-restore
```

Запустить API:

```powershell
dotnet run --project src/Education.Web/Education.Web.csproj
```

В development-режиме доступны:

- `/openapi/v1.json` - OpenAPI-документ.
- `/scalar/v1` - UI Scalar.
- `/health` - проверка работоспособности API.

## Конфигурация

Основные настройки передаются через `appsettings`, переменные окружения или user secrets:

- `ConnectionStrings:DefaultConnection` - строка подключения PostgreSQL.
- `Identity:Authority` - адрес Identity-сервиса.
- `Identity:Audience` - audience для JWT.
- `Jwt:RequireHttpsMetadata` - требование HTTPS metadata.
- `Jwt:NameClaimType` - claim с именем пользователя.
- `Jwt:RoleClaimType` - claim с ролью пользователя.
- `Cors:AllowedOrigins` - разрешенные frontend-origin.
- `FileStorage:RootPath` - корневая папка файлового хранилища.
- `FileStorage:MaxFileSizeBytes` - максимальный размер загружаемого файла.

## API и клиенты

Контракты находятся в `src/Education.Contracts`, маршруты вынесены в `ApiRoutes`.

Для генерации статических frontend-клиентов используйте OpenAPI-документ из `Education.Web`. Перед генерацией важно проверять, что у публичных API request/response моделей и endpoint-описаний есть XML-документация на русском языке.

## Тестирование

Все тесты backend должны быть интеграционными и работать с настоящей PostgreSQL через Testcontainers. Фейковые репозитории для тестирования бизнес-сценариев не добавлять.

Если тесты не стартуют, сначала проверьте, что Docker запущен.

## Важные ограничения

- Legacy-папку `Education` не удалять до отдельного решения о cutover.
- Frontend из папки `Frontend` считается временным: его планируется переписывать отдельно.
- Nullable warning пока не является блокером, если отдельно не договорились иначе.
- Identity-задачи не смешивать с Education backend: пользователи, пароли и полноценная авторизация должны жить в Identity-сервисе.
- Папка `Promts` игнорируется git и не должна попадать в коммиты.
