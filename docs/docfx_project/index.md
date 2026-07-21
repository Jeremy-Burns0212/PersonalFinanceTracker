# PersonalFinanceTracker API Docs

This documentation is generated from the project's XML documentation and source code comments.

To generate the API docs locally:

Windows (recommended):

1. Install DocFX (see https://dotnet.github.io/docfx/):
   - Using Chocolatey: `choco install docfx -y`
   - Or from the DocFX releases: https://github.com/dotnet/docfx/releases

2. Build the project (to emit XML docs):

```powershell
cd PersonalFinanceTracker
dotnet build -c Debug
```

3. From the repo root run DocFX build:

```powershell
cd docs\docfx_project
docfx metadata
docfx build
docfx serve _site
```

If you prefer using Docker or a different host, consult the DocFX documentation.
