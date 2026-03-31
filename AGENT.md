# Agent guide — Frank.Channels.DependencyInjection

This file orients automated assistants and contributors to how this repository is structured and how to work on it safely.

## Purpose

Small NuGet library: registers `System.Threading.Channels.Channel<T>` (and matching `ChannelReader<T>` / `ChannelWriter<T>`) with `Microsoft.Extensions.DependencyInjection` as singletons, with optional bounded/unbounded configuration via `ChannelSettings`.

## Tooling

- **SDK**: .NET 10. `global.json` pins a minimum SDK (`10.0.100`) with `rollForward: latestFeature` so compatible patch/feature bands are accepted.
- **Solution**: XML solution file `Frank.Channels.DependencyInjection.slnx` (not legacy `.sln`). Build and test target the `.slnx` path.
- **Target framework**: `net10.0` for all projects via `Directory.Build.props`.

## Common commands

From the repository root:

```bash
dotnet restore
dotnet build Frank.Channels.DependencyInjection.slnx -c Release
dotnet test Frank.Channels.DependencyInjection.slnx -c Release
```

For iterative test runs: `dotnet watch test Frank.Channels.DependencyInjection.slnx` (also wired as the VS Code **watch** task).

## Project map

| Path | Role |
|------|------|
| `Frank.Channels.DependencyInjection/` | Packable library; public API in `ServiceCollectionExtensions`, channel factory types. |
| `Frank.Channels.DependencyInjection.Tests/` | xUnit tests and experimental host-based tests (`WorkerV1.cs`). Not published. |
| `Directory.Build.props` | Shared TFMs, nullable, package metadata, SourceLink, `InternalsVisibleTo` for tests and `LINQPadQuery`. |

## Conventions

- Keep **Microsoft.Extensions.DependencyInjection** on the current **10.x** line aligned with the `net10.0` target.
- `AddChannel<T>` requires `T : class` and throws if `Channel<T>` is already registered for that `T`.
- Prefer extending existing registration helpers rather than duplicating DI patterns.
- Match existing style (tabs in test files, XML indentation in props) when editing nearby code.
- Do not remove XML doc on public API without good reason; package builds with documentation file generation.

## CI

Workflows under `.github/workflows/` call reusable workflows in `frankhaugen/Workflows`. If builds fail only in CI, check whether the shared workflow’s SDK image supports .NET 10 yet.

## Further reading

- [docs/build-and-test.md](docs/build-and-test.md) — environment and verification steps.
- [docs/library-design.md](docs/library-design.md) — registration behavior and extension points.
