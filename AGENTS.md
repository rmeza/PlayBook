# AGENTS.md

## What this repo is

Personal .NET Senior/Staff interview-prep **knowledge base** (Spanish). ~90% is markdown study notes; the only real code is the `CodeChallenge/` console app + test suite. `README.md` is the master dashboard/tracker — update it when adding/renaming topics.

## Layout

- `00-Meta/..09-Frontend/` — numbered study modules. Topic files follow `NN-ShortName.md`; module overviews live in `README.md`, not separate files.
- `_archivo-pdfs/` — **archive only**. All PDF content was already extracted to `.md`; do not re-extract or re-import these.
- `CodeChallenge/` — the only C# code. **Explicitly excluded** from PlayBook consolidation tasks; leave its structure alone.

## Content conventions

- Concepts/explanations in **Spanish**; interview answers in **English** one-liners (matches `07-Interview/`, `08-CheatSheet/`).
- Cross-references are shorthand, not full paths: `01-OOP-Fundamentals/03` means `01-OOP-Fundamentals/03-InterfaceVsAbstractClass.md`. Verify links resolve after edits.
- Keep cheat sheets (`08-CheatSheet/`, `09-Frontend/`) condensed: table + short snippet + one-liner per topic.

## CodeChallenge (net10.0 · xUnit v3 · FluentAssertions)

- Solution is `.slnx` (new XML format), not `.sln`. Run from `CodeChallenge/`:
  - `dotnet test CodeChallenge.slnx` — all tests
  - `dotnet test --filter "FullyQualifiedName~LongestSubstring"` — one suite (see tracker `CodeChallenge/00-CodeChallenge-Playbook.md` for the name list)
  - `dotnet run --project CodeChallenge` — console demo
- Per problem: a class with a `public static` solution method carrying an XML doc with O() complexity, plus a matching `*Tests.cs` using `[Theory]`/`[InlineData]` + FluentAssertions (`result.Should().Be(...)`). Console demo methods are named `Test*` and called from `Program.cs`.
- Each challenge is developed one at a time (coach workflow in `00-CodeChallenge-Playbook.md`); don't add solutions without a test suite.

## Environment

- `dotnet` 10.0.400 installed; target `net10.0`.
- Windows: Git emits `LF will be replaced by CRLF` warnings on add — expected, harmless.
- Git: only commit/push when explicitly asked (user commits manually).