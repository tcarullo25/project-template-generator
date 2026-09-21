# {{ProjectName}}.Application

Application logic: use cases, orchestration, and the interfaces that
infrastructure implements.

The rule that keeps the layering honest: this project references
`{{ProjectName}}.Domain` and nothing else. Where it needs a database, an HTTP
client or a clock, it declares an interface here and
`{{ProjectName}}.Infrastructure` provides the implementation.

`Status/` is the one worked example that ships with the template — see
`IDatabaseProbe` (declared here) and `DatabaseProbe` (implemented in
Infrastructure). Delete it once you have real use cases.
