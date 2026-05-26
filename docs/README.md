Doc generation

This folder contains a minimal DocFX project to generate API documentation from the project's XML comments.

How to generate (summary):

1. Build the solution to produce XML docs: `dotnet build`
2. Install DocFX on your machine.
3. Run `docfx metadata` then `docfx build` in `docs/docfx_project`.
4. Serve with `docfx serve _site` to preview locally.
