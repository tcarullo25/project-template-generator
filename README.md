# project-template-generator

A command-line tool that creates a runnable full-stack project from a template.

```
project-template-generator new MahjongTracker
```

produces a `MahjongTracker/` directory containing an ASP.NET Core REST API with
Entity Framework Core and PostgreSQL, a React + TypeScript + Vite frontend,
Docker Compose for local development, test projects for both sides, and a README
describing how to run it all.

## Why this exists

Starting a personal project means reassembling the same stack every time:
solution layout, EF Core and Npgsql wiring, connection-string configuration,
health checks, a Vite app that can talk to the API, Docker Compose, test
projects, `.gitignore`, `.env.example`. Getting that right takes an afternoon and
it is the same afternoon each time.

This tool does it in a second, the same way every time.

Two properties matter:

- **Deterministic.** The same project name always produces the same output. No
  network access, no language model, no API key, no hidden state.
- **Generated projects are independent.** Nothing the tool emits references the
  generator. Delete this repository and the projects it created still build, run
  and make sense.

## Prerequisites

- .NET SDK 8.0 or newer (to build and run the generator).

That is all the generator needs. The *generated* project additionally needs
Node.js, Docker and `dotnet-ef` — its own README lists them.

## Build

```
git clone <this repository>
cd project-template-generator
dotnet build
```

## Run

During development:

```
dotnet run --project src/ProjectTemplateGenerator.Cli -- new MahjongTracker
```

Installed as a global tool, so that `project-template-generator` is on your PATH:

```
dotnet pack src/ProjectTemplateGenerator.Cli -c Release -o ./artifacts
dotnet tool install --global --add-source ./artifacts ProjectTemplateGenerator
```

Update or remove it later with `dotnet tool update` / `dotnet tool uninstall
--global ProjectTemplateGenerator`.

## Usage

```
project-template-generator new <ProjectName> [options]
project-template-generator list-templates
project-template-generator --help | --version
```

Options for `new`:

| Option | Default | Meaning |
| --- | --- | --- |
| `-o`, `--output <dir>` | current directory | Directory the project directory is created in |
| `-t`, `--template <id>` | `fullstack-aspnet-react` | Template to generate from |
| `--force` | off | Write into the target directory even if it already contains files |
| `--git-init` | off | Run `git init --initial-branch=main` in the generated project |

Exit codes: `0` success, `1` user error (bad input, refused operation), `2` an
unexpected failure — a bug, and the only case that prints a stack trace.

### Example

```
$ project-template-generator new MahjongTracker --output ~/code
  Creating project 'MahjongTracker' in /home/me/code/MahjongTracker...
  Creating backend...
  Creating frontend...
  Creating tests...
  Creating Docker configuration...
  Creating environment configuration...
  Creating project files...

Project created successfully: /home/me/code/MahjongTracker
  template: fullstack-aspnet-react
  files:    54

Next steps:
  cd MahjongTracker
  cp .env.example .env
  docker compose up
```

Project names are validated before anything is written: ASCII letters and
digits, starting with a letter, at most 64 characters. Names that are Windows
device names (`CON`, `NUL`, ...) or C# keywords are rejected. The name becomes a
directory name, a root namespace and several assembly names, so anything that
would need escaping downstream is refused up front.

## Generated project structure

`project-template-generator new MahjongTracker` produces:

```
MahjongTracker/
├── docker/
│   ├── api.Dockerfile
│   └── web.Dockerfile
├── src/
│   ├── Backend/
│   │   ├── MahjongTracker.Domain/          entities, domain rules; no dependencies
│   │   ├── MahjongTracker.Application/     use cases; interfaces infrastructure implements
│   │   ├── MahjongTracker.Infrastructure/  EF Core, PostgreSQL, migrations
│   │   └── MahjongTracker.Api/             controllers, DI composition, health checks
│   └── Frontend/
│       └── web/                            React + TypeScript + Vite
├── tests/
│   ├── MahjongTracker.UnitTests/
│   └── MahjongTracker.Api.Tests/
├── .env.example
├── .editorconfig
├── .gitignore
├── docker-compose.yml
├── Directory.Build.props
├── MahjongTracker.sln
└── README.md
```

The generated project has no business functionality — no `User`, no `Product`,
no sample CRUD. What it has is one `/api/status` endpoint that reports database
connectivity and a frontend that calls it, so a fresh clone can demonstrate
React → ASP.NET Core → PostgreSQL working before any code is written. The
generated README explains how to delete it.

## Repository layout

```
project-template-generator/
├── src/
│   ├── ProjectTemplateGenerator.Core/      generation engine (no console I/O)
│   └── ProjectTemplateGenerator.Cli/       argument parsing, console output, exit codes
├── templates/
│   └── fullstack-aspnet-react/
│       ├── template.json                   manifest: id, description, pinned versions
│       └── content/                        the files that get generated
└── tests/
    └── ProjectTemplateGenerator.Tests/
```

## How templates work

A template is a directory under `templates/` containing a `template.json`
manifest and a `content/` directory. `content/` is the generated project, file
for file — open it and you are reading the code that will be emitted.

Generation applies two rules:

**1. `{{VariableName}}` is replaced by its value**, in file contents and in path
segments. That is the entire template language: no conditionals, no loops, no
expressions. Template files stay valid C#, TypeScript and YAML that an editor
can parse and a linter can check. A placeholder with no value is an error, so a
typo fails generation instead of shipping a broken project.

**2. A leading underscore in a file or directory name becomes a dot.**
`content/_gitignore` is generated as `.gitignore`. Dot-files are stored
underscored so that the template tree is not itself affected by tooling (git,
npm) that treats dot-files specially.

Variables come from two places:

| Source | Variables |
| --- | --- |
| Derived from the project name (`ProjectVariables`) | `ProjectName`, `ProjectNamespace`, `ProjectSlug`, `DatabaseName`, `DatabaseUser` |
| `template.json` → `variables` | every pinned version and port: `TargetFramework`, `EfCoreVersion`, `NpgsqlEfCoreVersion`, `ReactVersion`, `ViteVersion`, `PostgresImageTag`, `NodeImageTag`, `ApiHttpPort`, ... |

For `MahjongTracker` the derived values are:

| Variable | Value | Used for |
| --- | --- | --- |
| `ProjectName` | `MahjongTracker` | directories, assemblies, titles |
| `ProjectNamespace` | `MahjongTracker` | C# root namespace |
| `ProjectSlug` | `mahjong-tracker` | npm package name, Compose project name |
| `DatabaseName` | `mahjong_tracker` | PostgreSQL database |
| `DatabaseUser` | `mahjong_tracker` | PostgreSQL role |

### Changing what gets generated

Edit the files under `templates/fullstack-aspnet-react/content/`. They are
ordinary source files; there is no code generator to update.

### Updating versions

Every pinned version lives in `templates/fullstack-aspnet-react/template.json`.
Bumping .NET, EF Core, React, Vite or the PostgreSQL image is an edit to that one
file — generator code contains no version strings. Two tests enforce the
arrangement: one fails if a template file uses a placeholder nobody defines, the
other fails if the manifest defines one nothing uses.

### Adding a template

Create `templates/<your-id>/` with a `template.json` and a `content/` directory.
The catalog discovers it from disk; there is no registration list. Generate from
it with `--template <your-id>`.

## Tests

```
dotnet test
```

61 tests, under a second, no network. They cover project-name validation, the
naming derivations, template rendering and path transformation, the CLI parser,
the manifest, and generation against the real shipped template — including that
the generated tree matches the documented structure, that substitution reached
both paths and contents, that no `{{placeholder}}` survives, that generation is
deterministic, and that nothing generated mentions the generator.

Generation tests write only into a unique directory under the system temp path
and delete it afterwards. Running the suite never touches this repository or
anything outside that directory.

Two further tests actually build and test a generated project — `dotnet build`,
`dotnet test`, `npm install`, `npm run build`, `npm test`. They need Node.js and
network access for package restore and take minutes, so they are opt-in:

```
PTG_SMOKE_TESTS=1 dotnet test
```

## Architectural decisions

**The generator is a .NET console app.** It is what the generated backend is
written in, it is cross-platform, it packs as a `dotnet tool`, and it means one
language to maintain rather than two.

**No third-party dependencies in the generator.** Argument parsing is ~60 lines
for one command with four options; a parsing library would be the tool's only
runtime dependency and would not earn it. `System.Text.Json` reads the manifest.
The test project takes xUnit and `Xunit.SkippableFact` (so the opt-in smoke tests
report as skipped rather than silently passing). Reconsider the parser if the
command set grows; the CLI is deliberately separated from `Core` so that
swapping it changes one project.

**Templates are files on disk, not strings in C#.** `content/` is a working
project: readable, diffable, editable in an IDE with syntax highlighting, and
reviewable in a pull request. The alternative — source files as C# string
literals — is unreadable at the size this needs to be.

**Templates are copied next to the assembly, not embedded as resources.**
Embedding mangles paths into resource names and makes the shipped layout differ
from the source layout. A `templates/` folder beside the binary keeps them
identical. (`dotnet pack` includes it in the tool package.)

**Substitution only — no template engine.** Scriban, Handlebars and friends buy
conditionals and loops that V1 does not need, at the cost of a dependency and of
template files that no longer parse as the language they are written in. If
conditional templates become necessary, that is the point to reconsider.

**Stack knowledge lives in the template, not in code.** `ProjectGenerator` knows
about names, paths, files and safety. It contains no mention of ASP.NET Core,
React or PostgreSQL. Adding a second stack is a directory, not a code change.

**Four backend projects.** `Domain` / `Application` / `Infrastructure` / `Api` is
more structure than a tiny app needs on day one, but the split is the reason
`Application` cannot accidentally reference Npgsql, and retrofitting it later
means moving every file. Each project ships nearly empty; the layering is a
constraint, not a framework.

**No git repository by default.** Generating files and creating a repository are
separate decisions, and the generator should not require git to be installed.
`--git-init` runs `git init --initial-branch=main` with a fixed argument list —
nothing the user types reaches a shell — and a failure is a warning, not an
error, because the generated project is complete without it. Nothing is ever
committed or pushed.

**Failing safely beats repairing.** If the target directory exists and is not
empty, generation stops before writing anything and says so. `--force` allows
writing into it, but the generator never deletes a directory tree and never
writes outside the project directory: every resolved path is checked against the
project root, so a `..` in a template path aborts generation.

## Limitations and deferred work

- **One template.** `fullstack-aspnet-react` is the only stack. The catalog and
  the manifest exist so a second one does not require code changes.
- **No `add entity` / `add auth` / `add feature` commands.** Modifying an
  existing project is a different problem from creating one (parsing and
  rewriting code that already exists) and V1 does not attempt it. The template
  and variable machinery is reusable when it does.
- **No project configuration file.** Everything is a command-line option.
  `ProjectSpecification` is separate from the CLI so a config file or an
  interactive prompt can produce one later.
- **Text templates only.** Every file is read and written as UTF-8 text. A
  template containing a binary file (an icon, say) would be corrupted; that needs
  a per-file "copy verbatim" rule in the manifest.
- **The generated frontend Docker image is development-only.** It runs the Vite
  dev server. A production image would build and serve static output.
- **No generated CI configuration.** Deliberate: a workflow file is opinionated
  about a CI provider in a way the rest of the template is not.
- **Migrations are not applied automatically** in the generated project, by
  design. See its README.
