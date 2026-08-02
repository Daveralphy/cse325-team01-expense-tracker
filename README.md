# Expense Tracker

A .NET 8 Blazor Web App that helps users securely record and understand their
personal expenses.

## Authentication foundation

The initial application includes:

- ASP.NET Core Identity with SQLite and Entity Framework Core
- User registration, login, and logout
- Unique email addresses and an eight-character minimum password length
- An authenticated-only **My Expenses** page
- A scoped `CurrentUserService` that future expense features can use to save
  and query records for only the signed-in user
- Automatic application of checked-in database migrations at startup

The local `Data/app.db` file is generated when the application starts and is
excluded from source control. Local data-protection keys are also generated in
`Data/DataProtectionKeys` and must not be committed.

## Run locally

Requirements: .NET 8 SDK.

```powershell
dotnet restore
dotnet run
```

Open the HTTPS URL shown in the terminal. Register an account, log in, and open
**My Expenses** to verify that protected access works.

## Azure deployment notes

The application is currently aligned with the team checkpoint agreement to stay on
.NET 8 and to prepare for Azure App Service deployment.

Recommended deployment approach:

1. Create an Azure App Service for a .NET 8 web app.
2. Set the runtime stack to `DOTNETCORE|8.0`.
3. Configure the production connection string using the
   `ConnectionStrings__DefaultConnection` application setting.
4. Keep the application database on SQLite for this course project, or replace
   it with Azure SQL later if the team needs a managed production database.

For Azure App Service, the app now uses forwarded headers so request scheme and
host information are preserved correctly behind the reverse proxy.

## Development workflow

1. Pull the latest `main` branch.
2. Create a separate feature branch.
3. Build and test the change locally.
4. Push the branch and request review before merging.

Do not commit local database files, credentials, or application secrets.

## Team members

- Benjamin Asante (Group Leader, Week 2)
- John Mark Bacod Manuel
- Raphael Daveal
- Olakunle Obademi
- Ka Kan Ho
