# KlikajZGlowa

KlikajZGlowa is an educational web application for creating, publishing, and studying online courses. It provides a Blazor WebAssembly client, an ASP.NET Core Web API, SQL Server LocalDB persistence, JWT authentication, role-based authorization, and AI-assisted lesson generation.

## Features

- User registration, login, profile management, and password changes
- JWT-based authentication with role-based authorization
- Course browsing and enrollment
- Course and lesson management for authors and administrators
- Author applications and administration workflows
- AI-assisted lesson draft generation through the Gemini API
- Swagger/OpenAPI documentation for the API
- EF Core migrations and a database seeder with sample users and course data

## Technology stack

- .NET 10
- ASP.NET Core Web API
- Blazor WebAssembly
- Entity Framework Core with SQL Server LocalDB
- ASP.NET Core Identity
- JWT Bearer authentication
- AutoMapper
- Swagger / Swashbuckle
- Google Gemini API for AI-assisted content generation
- Bootstrap for the client UI

## Solution structure

| Project | Description |
| --- | --- |
| `KlikajZGlowa.Client` | Blazor WebAssembly frontend |
| `KlikajZGlowa.API` | ASP.NET Core API, authentication, authorization, and controllers |
| `KlikajZGlowa.Service` | Application services, DTOs, mappings, token handling, and AI integration |
| `KlikajZGlowa.Database` | EF Core `DbContext`, SQL Server configuration, and migrations |
| `KlikajZGlowa.Model` | Domain entities, user roles, and shared models |
| `KlikajZGlowa.Seeder` | Database migration and sample data seeding utility |

## Prerequisites

- Visual Studio 2026 with the ASP.NET and web development workload, or the .NET 10 SDK
- SQL Server LocalDB (`MSSQLLocalDB`)
- A Gemini API key if AI lesson generation is required
- A trusted HTTPS development certificate

## Configuration

The API reads the following settings:

- `ConnectionStrings:DefaultConnection` for the API database context
- `JWT:Issuer`
- `JWT:Audience`
- `JWT:SigningKey`
- `JWT:ExpireMinutes`
- `AiSettings:ApiKey`
- `AiSettings:Model`

Do not commit JWT signing keys, API keys, or other credentials. Use .NET User Secrets, environment variables, or a local, ignored configuration file for development. For example, from the API project directory:

```powershell
dotnet user-secrets init
dotnet user-secrets set "JWT:SigningKey" "replace-with-a-long-random-development-key"
dotnet user-secrets set "JWT:Issuer" "https://localhost:7173"
dotnet user-secrets set "JWT:Audience" "https://localhost:7173"
dotnet user-secrets set "AiSettings:ApiKey" "replace-with-your-gemini-api-key"
```

The current database context uses a SQL Server LocalDB `.mdf` file. Before running the application on another machine, check the database file path in `KlikajZGlowa.Database/Db.cs` and update it if necessary.

## Running locally

1. Clone the repository and open `KlikajZGlowa.slnx` in Visual Studio.
2. Restore dependencies and build the solution:

   ```powershell
   dotnet restore KlikajZGlowa.slnx
   dotnet build KlikajZGlowa.slnx
   ```

3. Ensure that SQL Server LocalDB is installed and that the configured database file is accessible.
4. Apply migrations and optionally create sample data by running the seeder:

   ```powershell
   dotnet run --project KlikajZGlowa.Seeder
   ```

5. Start the API:

   ```powershell
   dotnet run --project KlikajZGlowa.API --launch-profile https
   ```

6. Start the Blazor client in a second terminal:

   ```powershell
   dotnet run --project KlikajZGlowa.Client --launch-profile https
   ```

The default development URLs are:

- Blazor client: `https://localhost:7121` or `http://localhost:5221`
- API: `https://localhost:7173` or `http://localhost:5022`
- Swagger UI: `https://localhost:7173/swagger`

The client is configured to call the API at `https://localhost:7173`. If the API URL changes, update the named HTTP client in `KlikajZGlowa.Client/Program.cs` and the API CORS policy in `KlikajZGlowa.API/Program.cs`.

## Sample development accounts

The seeder creates these accounts when the database has no users:

| Account | Email | Password |
| --- | --- | --- |
| Author | `author@local` | `P@ssw0rd1` |
| Student | `student@local` | `P@ssw0rd1` |

These credentials are for local development only. Change or remove them before using the application outside a local environment.

## API overview

Controllers are exposed under `/api`:

- `ApplicationUser` — registration, login, profile and account operations
- `Course` — public course browsing, enrollment, and author/admin course management
- `Lesson` — lesson browsing and author/admin lesson management
- `Author` — author profiles and management
- `AuthorApplication` — author application workflow
- `EnrolledUser` — enrolled-user operations
- `AIClient/generate-lesson` — AI lesson draft generation for authors and administrators

Authenticated requests use the following header:

```text
Authorization: Bearer <jwt-token>
```

The main application roles are `Admin`, `Author`, `EnrolledUser`, and `Guest`.

## Database migrations

Migrations are stored in `KlikajZGlowa.Database/Migrations`. The seeder runs `Database.MigrateAsync()` before inserting sample data. To create a new migration after changing the model, run:

```powershell
dotnet ef migrations add MigrationName --project KlikajZGlowa.Database --startup-project KlikajZGlowa.API
```

Then update the database with:

```powershell
dotnet ef database update --project KlikajZGlowa.Database --startup-project KlikajZGlowa.API
```

## Security notes

- Keep API keys and JWT signing keys outside source control.
- Use HTTPS when running the API and client together.
- Replace the seeded development passwords in any shared environment.
- Review the LocalDB connection configuration before deployment; LocalDB is intended for development and is not a production database setup.

## License

No license has been specified for this repository yet.
