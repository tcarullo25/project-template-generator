# {{ProjectName}}

A full-stack application:

- **Backend** — ASP.NET Core REST API ({{TargetFramework}}) with Entity Framework Core and PostgreSQL.
- **Frontend** — React + TypeScript, built with Vite.
- **Local development** — Docker Compose runs PostgreSQL, the API and the dev server.

There is no business functionality yet. What exists is the wiring: configuration,
dependency injection, data access, error handling, health checks, tests, and a
single `/api/status` endpoint the frontend calls to prove the whole path works.

## Prerequisites

| Tool | Version | Needed for |
| --- | --- | --- |
| .NET SDK | {{DotnetSdkVersion}} | building and running the backend |
| Node.js | {{NodeMajorVersion}} or newer | building and running the frontend |
| Docker (with Compose v2) | any recent | running PostgreSQL and the full stack |
| `dotnet-ef` | matching EF Core {{EfCoreVersion}} | creating and applying migrations |

Install the EF Core tools once per machine:

```
dotnet tool install --global dotnet-ef --version {{EfCoreVersion}}
```

## Getting started

```
cp .env.example .env      # then edit POSTGRES_PASSWORD
docker compose up
```

- Frontend: <http://localhost:{{WebDevPort}}>
- API: <http://localhost:{{ApiHttpPort}}/api/status>
- Swagger UI (Development only): <http://localhost:{{ApiHttpPort}}/swagger>

The frontend should show the application name, environment, and `connected` for
the database. If it does, the React → ASP.NET Core → PostgreSQL path is working.

### Running the apps natively

Docker for the database only, everything else on the host:

```
docker compose up postgres
dotnet run --project src/Backend/{{ProjectName}}.Api     # http://localhost:{{ApiHttpPort}}
npm --prefix src/Frontend/web install
npm --prefix src/Frontend/web run dev                    # http://localhost:{{WebDevPort}}
```

The Vite dev server proxies `/api` to the backend, so no CORS configuration is
needed in development.

## Project structure

```
{{ProjectName}}/
├── docker/
│   ├── api.Dockerfile            Multi-stage build for the API
│   └── web.Dockerfile            Dev-server image for the frontend
├── src/
│   ├── Backend/
│   │   ├── {{ProjectName}}.Domain/          Entities and domain rules. No dependencies.
│   │   ├── {{ProjectName}}.Application/     Use cases; interfaces infrastructure implements.
│   │   ├── {{ProjectName}}.Infrastructure/  EF Core, PostgreSQL, migrations.
│   │   └── {{ProjectName}}.Api/             Controllers, DI composition, health checks.
│   └── Frontend/
│       └── web/                  React + TypeScript + Vite
├── tests/
│   ├── {{ProjectName}}.UnitTests/   Application-layer tests (no I/O)
│   └── {{ProjectName}}.Api.Tests/   In-process API tests
├── .env.example                  Environment variable template
├── docker-compose.yml            Local development stack
├── Directory.Build.props         Shared .NET settings (target framework, nullability)
└── {{ProjectName}}.sln
```

Dependencies point inward: `Api` → `Infrastructure` → `Application` → `Domain`.
`Application` declares interfaces (for example `IDatabaseProbe`) that
`Infrastructure` implements, so the inner layers never reference a database
driver or a web framework.

## Backend commands

```
dotnet build {{ProjectName}}.sln
dotnet run --project src/Backend/{{ProjectName}}.Api
dotnet test {{ProjectName}}.sln
dotnet format {{ProjectName}}.sln
```

## Frontend commands

Run from `src/Frontend/web` (or with `npm --prefix src/Frontend/web`):

```
npm install
npm run dev          # dev server with hot reload
npm run build        # type-check, then production build into dist/
npm run typecheck
npm test             # vitest, single run
npm run test:watch
```

## Database

PostgreSQL runs as the `postgres` service in `docker-compose.yml` with its data
in the `postgres-data` volume. `docker compose down -v` deletes that volume and
resets the database.

The `Default` connection string is configuration key `ConnectionStrings:Default`;
as an environment variable that is `ConnectionStrings__Default`. Compose builds
it from the `POSTGRES_*` values in `.env`.

### Migrations

`{{ProjectName}}.Infrastructure` holds `ApplicationDbContext` and the migrations;
`{{ProjectName}}.Api` supplies the design-time configuration.

Create a migration:

```
dotnet ef migrations add <Name> \
  --project src/Backend/{{ProjectName}}.Infrastructure \
  --startup-project src/Backend/{{ProjectName}}.Api \
  --output-dir Persistence/Migrations
```

Apply migrations:

```
dotnet ef database update \
  --project src/Backend/{{ProjectName}}.Infrastructure \
  --startup-project src/Backend/{{ProjectName}}.Api
```

Remove the most recent, unapplied migration:

```
dotnet ef migrations remove \
  --project src/Backend/{{ProjectName}}.Infrastructure \
  --startup-project src/Backend/{{ProjectName}}.Api
```

Migrations are **not** applied automatically at startup. Run
`dotnet ef database update` yourself, or add a deliberate migration step to your
deployment process.

There are no entities yet, so there are no migrations yet. The first one appears
once you add a `DbSet<T>` to `ApplicationDbContext`.

## Testing

```
dotnet test {{ProjectName}}.sln              # backend
npm --prefix src/Frontend/web test           # frontend
```

- `{{ProjectName}}.UnitTests` — plain xUnit tests over the application layer.
  Dependencies are substituted by hand; there is no mocking package.
- `{{ProjectName}}.Api.Tests` — hosts the real API in-process with
  `WebApplicationFactory` and checks `/health/live`. This catches a broken
  `Program.cs` or a missing service registration without needing a database.
  Tests that genuinely need PostgreSQL should start their own instance (for
  example with Testcontainers) rather than depend on a developer's local one.
- Frontend tests use Vitest with jsdom and Testing Library; `fetch` is stubbed,
  so they run without a backend.

## Docker

```
docker compose up                  # full stack
docker compose up postgres         # database only
docker compose up --build          # rebuild images after dependency changes
docker compose logs -f api
docker compose down                # stop
docker compose down -v             # stop and delete the database volume
```

`docker/web.Dockerfile` runs the Vite dev server with the source mounted, so it
is for development only. A production frontend image would run `npm run build`
and serve `dist/` from a static web server. `docker/api.Dockerfile` is already a
multi-stage Release build and is closer to production-ready, but has not been
hardened for it.

## Environment variables

Copy `.env.example` to `.env`; `docker compose` reads it automatically. `.env`
is git-ignored — keep real secrets out of source control.

| Variable | Used by | Purpose |
| --- | --- | --- |
| `POSTGRES_DB` | postgres, api | Database name |
| `POSTGRES_USER` | postgres, api | Database user |
| `POSTGRES_PASSWORD` | postgres, api | Database password (required) |
| `POSTGRES_PORT` | postgres | Host port for PostgreSQL |
| `ConnectionStrings__Default` | api | Full connection string when running natively |
| `ASPNETCORE_ENVIRONMENT` | api | `Development`, `Staging`, `Production` |
| `API_PORT` | api, web | Host port for the API |
| `WEB_PORT` | web | Host port for the dev server |
| `VITE_API_BASE_URL` | web | API base URL; empty means same origin |

`VITE_*` variables are inlined into the frontend bundle at build time and are
visible to anyone who loads the page. Never put a secret in one.

## Health checks

| Endpoint | Meaning |
| --- | --- |
| `GET /health/live` | The process is running. Used as a liveness probe. |
| `GET /health/ready` | The process can also reach PostgreSQL. Used as a readiness probe. |
| `GET /api/status` | Application name, environment and database connectivity, as JSON. |

## Where to start

1. Delete the status example: `StatusService`, `IDatabaseProbe`,
   `StatusController`, `DatabaseHealthCheck` and the tests and frontend code that
   go with them. (Keep `DatabaseHealthCheck` if you want the readiness probe.)
2. Add your first entity to `{{ProjectName}}.Domain`, a `DbSet<T>` to
   `ApplicationDbContext`, and create the first migration.
3. Add use cases to `{{ProjectName}}.Application` and controllers to
   `{{ProjectName}}.Api`.
