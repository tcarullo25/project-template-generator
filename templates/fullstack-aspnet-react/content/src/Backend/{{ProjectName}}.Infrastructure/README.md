# {{ProjectName}}.Infrastructure

Everything that talks to the outside world: the EF Core `ApplicationDbContext`,
the PostgreSQL provider configuration, migrations, and implementations of the
interfaces declared in `{{ProjectName}}.Application`.

Migrations live in `Persistence/Migrations`. See the repository README for the
`dotnet ef` commands — this is the `--project`, and `{{ProjectName}}.Api` is the
`--startup-project`.
