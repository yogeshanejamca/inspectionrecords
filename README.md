# Inspection Records

A local inspection app adapted from Inspections and Inspections.Web. The React/TypeScript frontend calls an ASP.NET Core API backed by EF Core and Microsoft SQL Server. The API supports inspection CRUD, pause history, image uploads and downloads, templates, checklists, and state records. The React app includes the inspection list, creation and editing, image viewing, and state-record screens.

## Backend architecture

Open `api/InspectionRecords.sln` in Visual Studio. Set `InspectionRecords.Api` as the startup project.

| Project | Responsibility |
| --- | --- |
| `InspectionRecords.Domain` | Inspection, template, checklist, and state-record entities |
| `InspectionRecords.Application` | Use cases, request/response contracts, and persistence interfaces |
| `InspectionRecords.Infrastructure` | SQL Server contexts, EF Core migrations, repository implementations, and optional demo seed data |
| `InspectionRecords.Api` | HTTP endpoints, dependency registration, and startup |

Application depends on Domain. Infrastructure implements Application's persistence interfaces. API wires the projects together. The React frontend lives in `web/` and uses the same HTTP routes.

## Run locally on Windows

Prerequisites are the .NET 10 SDK, Node.js 22.12+ or 24, and SQL Server or SQL Server Express LocalDB. Use a new demo database that your Windows login can create.

1. In a PowerShell terminal, set the API environment.

   ```powershell
   cd api
   $env:ASPNETCORE_ENVIRONMENT = 'Development'
   ```

2. The default connects to `(localdb)\MSSQLLocalDB` and creates the `InspectionRecordsDemo` database. If you run SQL Server Express instead, override the connection for this terminal. Change the server name to match your installed instance. For a default SQL Server instance, use `localhost`.

   ```powershell
   $env:ConnectionStrings__Inspections = 'Server=.\SQLEXPRESS;Database=InspectionRecordsDemo;Integrated Security=True;TrustServerCertificate=True'
   ```

   If you use SQL authentication, keep the local connection string in .NET user secrets under `ConnectionStrings:Inspections` or set it in your shell environment. `TrustServerCertificate=True` is for local development.

3. Initialize the schema and fictional sample data, then start the API.

   ```powershell
   dotnet run --project InspectionRecords.Api -- --initialize
   dotnet run --project InspectionRecords.Api
   ```

4. In a separate terminal from the repo root, start React.

   ```powershell
   cd web
   npm ci
   npm run dev
   ```

5. Open http://localhost:5173. Click **Add inspection**, choose **Facility inspection**, fill in the details, and optionally attach a sample image. Open the saved inspection and try changing its status to **Paused** and then **InProgress**. The pause history is available at `GET /api/inspections/{id}/pauses`.

The local API listens on http://localhost:5080. Vite proxies `/api` requests to it. `GET /api/health` checks database connectivity. All uploaded images and demo records live in the local SQL Server database. The app uses a loopback-only API and requires no application sign-in.

To apply the migrations without inserting sample records, use `dotnet run --project InspectionRecords.Api -- --initialize --schema-only`. Normal startup does neither. Both existing EF migration histories are preserved, so an initialized local database can be reused.

If you use the user secrets CLI from the `api/` directory, pass `--project InspectionRecords.Api`.

## Checks

- From `api`, run `dotnet build InspectionRecords.sln`.
- From `web`, run `npm run build` and `npm test -- --runInBand`.
