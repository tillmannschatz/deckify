# Knowledge — deckify

Living document. Project state, conventions, COM quirks, modernization roadmap, troubleshooting. Grows over time — record "aha" moments, Office quirks, workarounds, deployment notes.

## Current State vs. Target Architecture

| Aspect | Today | Target (Bootstrap blueprint) |
|---|---|---|
| Layout | `src/Deckify/Modules/<Area>/`, `src/Deckify.Core/`, `tests/Deckify.Tests/` (Phase 1) | `src/Ribbon/`, `src/TaskPanes/`, `src/Services/`, `src/Interop/`, `tests/` |
| COM strategy | GC-only (zero `Marshal.ReleaseComObject` calls) | ComScope wrapper for every COM access; no chaining |
| Logging | `System.Diagnostics.Trace` + file append (~30 lines, [src/Deckify.Core/Common/Logger.cs](src/Deckify.Core/Common/Logger.cs)) | Serilog with structured logging |
| Task panes | WinForms UserControls | WPF UserControls hosted via ElementHost |
| Architecture doc location | `docs/architecture/README.md` (moved in Phase 0) | `docs/architecture/` |

## Modernization Roadmap

### Phase 0 — Foundation *(complete)*
- Antigravity → Claude Code config migration: `CLAUDE.md`, `KNOWLEDGE.md`, `.claude/{settings,commands,skills}`
- `ARCHITECTURE.md` moved to `docs/architecture/README.md`
- No production code changes

### Phase 1 — Structural refactor: src/-layout + Interop layer *(complete)*
- Verzeichnis-Refactor durchgefuehrt: `Deckify/Modules/<Area>/` → `src/Deckify/Modules/<Area>/`, `Deckify/Common/` → `src/Deckify/Common/` (nur `ErrorHandler` + `SystemUtilities`), `Deckify.Tests/` → `tests/Deckify.Tests/`.
- Leerer `src/Deckify/Interop/` mit `.gitkeep` als Phase-2-Vorbereitung.
- **Pure-Logic-Helper extrahiert** in neue netstandard2.0-Library [src/Deckify.Core/](src/Deckify.Core/): `Constants`, `Logger`, `ReferenceHelpers`, `SpacingHelpers`, `VersionHelpers` (gekuerzt), `ColorModel`, `IPaletteService`, `PaletteService`, `UpdateInfo`. Namespaces unveraendert (`Deckify.Common`, `Deckify.Modules.Colors`, `Deckify.Modules.AboutManager`) — keine `using`-Aenderungen im VSTO-Projekt noetig.
- **VersionHelpers-Aufspaltung:** `IsNewerVersion` + `GetExecutingAssemblyVersion` in Core (netstandard2.0). ClickOnce-Logik (`System.Deployment.Application` ist Framework-only) wandert in [src/Deckify/Modules/AboutManager/ClickOnceVersionProvider.cs](src/Deckify/Modules/AboutManager/ClickOnceVersionProvider.cs); 3 Call-Sites aktualisiert (`UpdateService.cs`, `TaskPaneAbout.cs` x2).
- `tests/Deckify.Tests/` referenziert nur noch `Deckify.Core`. **`dotnet test` funktioniert nativ** — kein `vswhere`/`vstest`-Workaround mehr noetig.
- Solution `Deckify.sln` enthaelt drei Projekte (`Deckify`, `Deckify.Core`, `Deckify.Tests`). `dotnet sln add` hat ausserdem `src/`+`tests/` als Solution-Folders sowie x86/x64-Configurations angelegt (alle als `Any CPU` aliased).
- `/build` weiterhin VS-MSBuild via `vswhere` (wegen VSTO-Targets), neu mit `-t:Restore;Build` und `-p:RestorePackagesConfig=true` damit packages.config + PackageReference in einem Lauf restored werden. `/test` ist jetzt `dotnet test`.
- **Strong-Naming-Workflow-Hinweis:** Core ist via Conditional `<SignAssembly>` nur unter VS-MSBuild signiert (PFX-Signing wird vom .NET SDK nicht unterstuetzt). Beim Wechsel zwischen `dotnet test` und VS-MSBuild **einmalig** `bin/` + `obj/` cleanen — sonst stolpert der incrementelle VSTO-Build ueber unsigned Core-DLLs aus dem dotnet-test-Lauf.
- **`effensify.pfx` ist gitignored** (Code-Signing-Zertifikat). Bei Worktree-Setup oder Fresh-Clone vom main-Workspace nach `src/Deckify/effensify.pfx` kopieren; sonst bricht der Build mit MSB3322.

### Rebrand: PPTXML → deckify (effensify-Familie) *(complete)*
- Produkt heisst nun **deckify** (lowercase im Branding/UI), Teil der effensify-Produktfamilie. Code-Identitaeten in PascalCase: AssemblyName/Namespace `Deckify`, Library `Deckify.Core`, Tests `Deckify.Tests`.
- Mews-/MD-Hardcodes komplett raus: Default-Palette ist jetzt **leer** (User customized), Logger schreibt nach `%TEMP%\deckify_Log.txt`, Palette/Settings unter `%AppData%\deckify\`, Mail/Subject-Templates auf `tillmannschatz@gmail.com` und `[deckify]`-Subject. `<SupportUrl>` zeigt auf <https://github.com/tillmannschatz/deckify> (1-Click-Install-Endpoint); Code-Repo bleibt `tillmannschatz/PPTXML`.
- **Distribution-Setup** *(complete)*: ClickOnce-Pages auf `https://tillmannschatz.github.io/deckify/`. csproj-`<PublishUrl>`/`<InstallUrl>`/`<UpdateUrl>` zeigen dorthin, `<MapFileExtensions>true</MapFileExtensions>` damit GH Pages keine `.dll`-Files blockiert. Cross-repo Action [.github/workflows/publish.yml](.github/workflows/publish.yml) (in PPTXML) baut + deployed nach `tillmannschatz/deckify` (`gh-pages` branch + Releases). Trigger via [`/publish`](.claude/commands/publish.md) Slash-Command (`patch`/`minor`/`major`/`X.Y.Z`). Setup-Anforderungen + Token-Scopes siehe [DEPLOYMENT.md](DEPLOYMENT.md).
- **Side-by-Side Debug**: Conditional `<AssemblyName>` in [src/Deckify/Deckify.csproj](src/Deckify/Deckify.csproj) auf `Configuration=Debug` -> `Deckify.Debug`. Damit hat das Debug-Manifest eine andere `assemblyIdentity` als Production; ClickOnce sieht beide als getrennte Apps. Tab-IDs sind `deckify` (Production) bzw. `deckify-debug` (Debug); zwei Ribbon-XMLs sind embedded, [src/Deckify/Ribbon.cs](src/Deckify/Ribbon.cs) waehlt zur Laufzeit anhand `Assembly.GetExecutingAssembly().GetName().Name`. `debug_setup.ps1` registriert in `HKCU:\…\Addins\Deckify.Debug` (separater Key vom Production `Addins\Deckify`).
- **Hard cutover** fuer existierende ClickOnce-Installationen unter altem `PPTXML`-Identity: kein Auto-Update; User muessen in Apps & Features deinstallieren und neuen Installer beziehen. Frühphase, bewusst akzeptiert.
- **User-Settings-Migrations-Hinweis**: Section-Namen in `app.config` aenderten sich (`PPTXML.SettingsMD` -> `Deckify.DeckifySettings`). Alte `user.config`-Werte sind nicht automatisch sichtbar; LicenseKey usw. einmalig neu eintragen.

### Phase 2 — COM discipline: ComScope everywhere, no chaining
- `src/Interop/ComScope.cs` (`IDisposable<T>` wrapper with `Marshal.ReleaseComObject`)
- Typed wrappers: `PresentationWrapper`, `SlideWrapper`, `ShapeWrapper`, `SelectionWrapper`
- Refactor all services and Ribbon callbacks to go through the Interop layer
- **Bundled defensive fixes** (spotted during Phase 0 testing):
  - `PaletteService.LoadPalette` should fall back to the default palette for **empty or corrupt** files, not only when the file is missing. Today an empty `palette.json` yields an empty palette (caught while fixing `RemoveColor` test in PR #97). Same pattern likely worth checking in any other JSON-on-disk persistence introduced before Phase 2.
- **Acceptance:** zero COM chains in `src/Services/` and `src/Ribbon/`; PowerPoint exits cleanly (no zombie `POWERPNT.EXE`); manual test checklist passes; corrupt/empty `palette.json` recovers to defaults instead of empty list
- **Risk:** double-release, release on `Application` (= crash), high diff count
- **Effort:** ~30–50h (defensive fixes add ~1h)

### Phase 3 — Logging: Serilog
- Add NuGet: `Serilog`, `Serilog.Sinks.File`, optionally `Serilog.Sinks.Debug`
- Rewrite `Logger.cs` (keep API backwards-compatible: `LogError`, `LogInfo`; add `LogWarning`, `LogDebug`, structured-context overload)
- Verify ClickOnce bundles Serilog DLLs
- **Acceptance:** structured logs written, ClickOnce installs and runs, all existing log call sites work unchanged
- **Risk:** ClickOnce assembly bundling
- **Effort:** ~5–8h

### Phase 4 — UI: WPF task panes via ElementHost
- Migrate 5 task panes (About, Settings, Screenshot, Colors, Dashboard) to WPF UserControls hosted in `ElementHost`
- Add references: `PresentationCore`, `PresentationFramework`, `WindowsBase`, `System.Xaml`
- ViewModel + XAML per pane
- **Acceptance:** feature parity, focus/keyboard works, DPI scaling clean across 100/125/150%
- **Risk:** ElementHost focus/input quirks, WebView2 hybrid in Dashboard pane
- **Effort:** ~30–50h

## COM hotspots (candidates for Phase 2 prioritization)

Inventory built during Phase 0 — places where COM iteration is frequent or in performance-sensitive paths. Expand during the Phase 2 audit.

- **`MirrorService.Application_WindowSelectionChange`** ([Deckify/Modules/Mirror/MirrorService.cs](Deckify/Modules/Mirror/MirrorService.cs)) — fires on every selection change, iterates `shape.Tags` collection (`Tags.Count` + `Tags.Name(i)` + `Tags.Value(i)` loop)
- **`ShapeService` (most methods)** — looping over `Slides` and `Shapes`, applied from Ribbon callbacks
- **`AgendaManager`** — slide iteration to build agenda
- **`DashboardManager.SlideShowManager`** — slide iteration for dashboard rendering
- **`RibbonMD.cs` callbacks (any touching `Selection.ShapeRange`)** — invoked per click; `DynamicGroupActivateDeactivate` runs on every selection change

## PowerPoint quirks

- **`Slide.Shapes` iteration with mutation:** modifying the collection during iteration causes index drift. Iterate by index in reverse, or build a list of identifiers first.
- **Master / Layout / Slide hierarchy:** layouts inherit from master; per-slide overrides exist on each slide. `SlideMaster.CustomLayouts` lists all layouts.
- **`Application` RCW:** never call `Marshal.ReleaseComObject` on `ThisAddIn.Application` — crashes the host.
- **Tags on shapes:** stored as ordered `(name, value)` pairs, not as a dictionary. Iterate via `Tags.Count` + `Tags.Name(i)` / `Tags.Value(i)`. `Add` overwrites if the name already exists.
- **Selection types:** `PpSelectionType` distinguishes shapes / text / slides / none. Always check before accessing `Selection.ShapeRange` — it throws otherwise.

## Deployment notes

- **ClickOnce:** publish via Visual Studio's Publish wizard. Output: `setup.exe` + `Deckify.vsto` + `Application Files/`. See [DEPLOYMENT.md](DEPLOYMENT.md).
- **Registry:** add-in registers under `HKCU\Software\Microsoft\Office\PowerPoint\Addins\Deckify`. Debug-registration helper script at the repo root: [debug_setup.ps1](debug_setup.ps1).
- **Auto-update:** ClickOnce checks `InstallUrl` periodically (default 7 days). Configurable in Project Properties → Publish → Updates.
- **Update check in-app:** `IUpdateService` ([Deckify/Modules/AboutManager/UpdateService.cs](Deckify/Modules/AboutManager/UpdateService.cs)) compares the installed ClickOnce version against the latest GitHub release tag.

## Manual test checklist (for COM-touching changes)

When changing code in `Modules/Services/`, `Modules/Mirror/`, `Modules/AgendaManager/`, `Modules/DashboardManager/`, or any Ribbon callback that touches PowerPoint COM:

1. Start PowerPoint with the add-in enabled.
2. Open an existing deck.
3. Run the modified command(s) on:
   - Empty slide
   - Slide with many shapes
   - Selected shapes (single + multi)
   - No selection / no presentation open
4. Save, close, reopen — confirm no corruption.
5. Verify add-in unload / PowerPoint exit without hanging (Task Manager: no `POWERPNT.EXE` after close).

## Code quality backlog (deferred / optional)

- **Logger improvements (without Serilog):** add severity levels (`Debug` / `Warning`) and a structured context dictionary. Lower-cost option if Phase 3 is deferred. ~2–3h.
- **Defensive boundaries:** audit public service methods for null-checks on `ActivePresentation` / `ActiveWindow.Selection` and consistent `COMException` handling.
- **Test coverage:** expand from current 4 test files (`VersionHelpersTests`, `ReferenceHelpersTests`, `SpacingHelpersTests`, `PaletteServiceTests`). After Phase 2, services become unit-testable via Interop wrapper fakes.

## Known bugs / workarounds

> Empty so far. Add entries as they surface — symptoms first, then root cause and fix.
