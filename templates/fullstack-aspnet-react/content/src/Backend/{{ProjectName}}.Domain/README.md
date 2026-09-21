# {{ProjectName}}.Domain

Domain model: entities, value objects, domain-level rules and the exceptions
they raise.

This project is deliberately empty. It has no package or project references, and
it must stay that way — nothing here should know about EF Core, ASP.NET Core or
any other infrastructure.

Add types here as real domain concepts appear. If a type is only a database row
shape, it probably belongs in `{{ProjectName}}.Infrastructure` instead.
