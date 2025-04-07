# TestProject

Это API проект, разработанный с использованием ASP.NET Core и PostgreSQL. Он предоставляет два REST-метода для работы с данными в базе данных.

## Требования

1. **.NET 8.0**
2. **PostgreSQL** для работы с базой данных
3. **pgAdmin**

## Настройка проекта

### Шаг 1: Клонировать репозиторий
git clone https://github.com/BorisovVadim/TestProject.git

### Шаг 2: Настройка базы данных
Установите и настройте PostgreSQL на вашем компьютере.
Создайте новую базу данных.
Откройте файл appsettings.json и обновите строку подключения для базы данных:
"DefaultConnection": "Host=localhost;Port=5432;Database=testprojectdb;Username=postgres;Password=postgres"

### Шаг 3: Применить миграции
В терминале выполните команду для применения миграций:
dotnet ef database update

### Шаг 4: Запуск приложения
Запустите приложение:
dotnet run
По умолчанию приложение будет доступно по адресу http://localhost:5219. Вы можете открыть Swagger UI для тестирования API по адресу http://localhost:5219/swagger.

### Тестирование через Swagger
Перейдите по следующему адресу в браузере: http://localhost:5219/swagger.

В Swagger UI вы увидите два метода API:
POST api/Data/save — Сохраняет данные в базу данных.
GET api/Data/get — Возвращает данные из базы данных.

Пример запроса для POST api/Data/save:
Отправьте JSON в формате:
[
  { "1": "value1" },
  { "5": "value2" },
  { "10": "value3" }
]

Пример запроса для GET /get:
Пример фильтрации данных:
GET api/Data/get?filter=5
