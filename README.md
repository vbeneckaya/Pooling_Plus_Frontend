**Для локальной сборки и запуска приложения потребуются:**

- dotnet SDK https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.402-windows-x64-installer
- Node js https://nodejs.org/dist/v10.16.3/node-v10.16.3-x64.msi
- Postgresql https://www.postgresql.org/download/
- pgadmin(Обозреватель базы данных) https://www.pgadmin.org/download/

**Как собрать и запустить бекенд?**
- Сбилдить "dotnet build" в папке "src/backend/API"
- Запустить "dotnet run" в папке "src/backend/API"

**Как изменить строку подключения к базе данных?**
Изменить в файле "src/backend/API/appsettings.json" ConnectionStrings->DefaultDatabase

**На каком порту запуститься бекенд?**
на http://localhost:5000

**Как проверить работоспособность бекенда?**
открыть http://localhost:5000/swagger/index.html должно отображаться данные об API


**В какой момент выполняются миграции?**
При старте приложение пытается выполнить новые миграции

**Как запустить фронт?**
- выполнить "npm i" в папке "src/backend/API/frontend"
- выполнить "npm start" в папке "src/backend/API/frontend"


**Как задаётся порт для фронта?**
?????

**Как задаётся базовый url для апи на фронте?**
?????