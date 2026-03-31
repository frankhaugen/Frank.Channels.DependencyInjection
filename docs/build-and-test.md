# Build and test

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (10.0.100 or newer; see repository `global.json` for roll-forward behavior).

## Restore, build, test

```bash
dotnet restore
dotnet build Frank.Channels.DependencyInjection.slnx -c Release
dotnet test Frank.Channels.DependencyInjection.slnx -c Release
```

## Solution file

This repo uses the **SLNX** format (`Frank.Channels.DependencyInjection.slnx`), supported by the .NET CLI (SDK 9.0.200+) and Visual Studio 2022 17.14+. Older tooling that cannot open `.slnx` needs an updated IDE or CLI; do not commit ad-hoc `.sln` copies unless the maintainers decide to dual-publish solution formats.

## Packaging

The library project is packable; test project sets `IsPackable` to `false`. To produce a package locally:

```bash
dotnet pack Frank.Channels.DependencyInjection/Frank.Channels.DependencyInjection.csproj -c Release
```

Package metadata and symbols (SourceLink) are configured in `Directory.Build.props`.
