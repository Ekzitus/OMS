# HelloApp 👋

Учебный проект на ASP.NET Core, созданный для практики и экспериментов.

## 📌 Цель проекта

На данный момент проект не имеет конкретной цели. Это "песочница" для изучения и отработки:

- ASP.NET Core (MVC, Web API)
- Entity Framework Core
- Аутентификации и авторизации
- Подключения баз данных
- REST API и JSON
- Работа с Git и GitHub
- Миграций, моделей и прочих базовых вещей

> **Примечание:** Эта ветка предназначена для работы с ASP.NET MVC.

## 🛠️ Технологии

- [.NET 8 / .NET 6](https://dotnet.microsoft.com/)
- ASP.NET Core
- Entity Framework Core
- ASP.NET MVC
- SQLite (используется в текущей версии)

## 📂 Структура проекта

```
HelloApp/
├── Controllers/       # Контроллеры
├── Models/            # Модели данных
├── Views/             # Представления MVC
├── Data/              # Контекст БД и миграции
├── Migrations/        # Миграции базы данных
├── Properties/        # Конфигурация проекта
├── wwwroot/           # Статические файлы
├── appsettings.json   # Настройки
├── Program.cs         # Точка входа
└── README.md          # Этот файл


HelloApp.Tests/
├── AccountControllerTests.cs  # Тесты для AccountController
├── HomeControllerTests.cs     # Тесты для HomeController
└── HelloApp.Tests.csproj      # Файл проекта тестов
```

## 🚀 Как запустить

```bash
dotnet run
```
Открой в браузере: https://localhost:5005

## 🧪 Что нового?

- Добавлена база данных SQLite с использованием Entity Framework Core.
- Реализована аутентификация с использованием Cookie-based Authentication.
- Добавлена авторизация с разделением ролей (Admin, User).
- Созданы тесты для контроллеров с использованием xUnit и Moq.
- Добавлены миграции для создания таблицы пользователей.

## 🧠 Заметки для себя

Это личный проект для практики. Я не боюсь что-то сломать, пробую новое, и иногда откатываюсь назад.

## 📜 Лицензия

Пока нет — проект учебный.