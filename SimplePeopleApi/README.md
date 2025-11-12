# SimplePeopleApi

Minimal ASP.NET Core Web API with:
- SQL Server (local) via EF Core
- Simple JWT authentication (register/login)
- CRUD endpoints for Person
- Small static HTML client that consumes the API at `wwwroot/index.html`

Prerequisites:
- .NET 7 SDK
- Local SQL Server (or adjust `appsettings.json` connection string)

Quick run (PowerShell):

```powershell
cd c:\projeto\SimplePeopleApi
# restore
dotnet restore
# run
dotnet run --urls "https://localhost:5001;http://localhost:5000"
```

Notes:
- Update the JWT key in `appsettings.json` for any serious usage.
- The app uses `EnsureCreated()` to create the database automatically for simplicity.
- The front-end is intentionally tiny and not production-ready.
