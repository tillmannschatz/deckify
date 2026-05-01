---
name: vsto-powerpoint
description: Use when writing or modifying code that touches PowerPoint VSTO interop, Ribbon callbacks, Task Panes, or VSTO add-in lifecycle. Triggers on Microsoft.Office.Interop.PowerPoint usage, IRibbonExtensibility, ThisAddIn, ElementHost, ComVisible, RCW lifecycle questions, or Ribbon XML changes.
---

# VSTO PowerPoint Skill

> **Note:** this skill describes the **target architecture**. Existing code is mid-migration — see [KNOWLEDGE.md](../../../KNOWLEDGE.md) for the phase plan and current status. When advising changes, consider both target patterns and the practical state of the file you're touching.

## Object-Model Hierarchy

```
Application → Presentations → Presentation → Slides → Slide → Shapes → Shape
                                                              ↓
                                                          TextFrame → TextRange → Runs
```

Every arrow is a COM object that must be acquired, used briefly, and released. See the `com-interop-discipline` skill for ComScope details.

## Ribbon (XML + Callbacks)

- Ribbon UI is defined in `RibbonMD.xml`. Do **not** use the Visual Studio Ribbon Designer.
- Callbacks live in `RibbonMD.cs`. The class must be `[ComVisible(true)]` and implement `Office.IRibbonExtensibility`.
- `Ribbon_Load(IRibbonUI)` stores the `IRibbonUI` reference for later `Invalidate()` calls (e.g. dynamic-group activation in `ThisAddIn.DynamicGroupActivateDeactivate`).
- Standard callback signatures: `OnAction(IRibbonControl)`, `GetEnabled(IRibbonControl)`, `GetVisible(IRibbonControl)`, `GetImage(string)`, `GetLabel(IRibbonControl)`.
- **Wrap every callback in `ErrorHandler.SafeExecute`** to prevent unhandled exceptions from crashing PowerPoint.
- Keep callbacks small: validate context → resolve service via `Globals.ThisAddIn.ServiceProvider.GetService<T>()` → delegate to the service → handle exceptions gracefully.

## Task Panes

- Task panes are added per `DocumentWindow`, keyed by `HWND` in `ThisAddIn.taskPaneSets` (see `EnsureTaskPanesInitializedForWindow`).
- UI logic stays in the pane control. Business logic goes into services that ideally don't depend on COM types.
- Pass minimal data into panes (IDs, primitive strings) rather than COM objects.
- Toggling visibility goes through `ThisAddIn.TogglePaneVisibility(pane, width)`.

## Threading

- The Office `Application` object is **STA-bound**. All COM access must happen on the main UI thread.
- Background work pattern:
  1. UI thread: extract minimal primitive data (IDs, strings, ints) from COM objects.
  2. Background thread: process primitive data only.
  3. UI thread (back via `BeginInvoke` or `await`): re-resolve COM objects from the IDs and apply results.
- `async/await` works in event handlers; the captured `SynchronizationContext` is the WindowsForms context, so resumption is on the UI thread by default.

## Output template — "COM Interop Notes"

For every change that touches PowerPoint/VSTO/Interop APIs, include this section in the PR description (or commit message body for direct commits):

```
### COM Interop Notes

- COM touchpoints: <objects/APIs used — Application, ActivePresentation, Slides, Shapes, Selection, TextRange>
- Hotspot scan:
  - Loops over COM collections? (YES/NO)
  - Event handlers added/modified? (YES/NO)
  - Frequently-invoked path (selection change, ribbon callback)? (YES/NO)
  - Async/background code interacting with COM? (YES/NO)
- COM lifetime strategy:
  - "GC-only" — explain how proxies and round-trips were minimized; OR
  - "ComScope/targeted release" — list each scope and the COM objects acquired
- Safety checks: state validations added (no presentation, no selection, mid-action change), COMException handling
- Manual test checklist run? (see KNOWLEDGE.md)
```

## Common Mistakes

- **Method-chaining COM properties** (`app.ActivePresentation.Slides[1].Shapes[1]`) → RCW leak. Break into separate variables.
- **Long operations in a Ribbon callback** → Office UI hangs. Push to background; marshal results back.
- **Trying to migrate to .NET 6+** → VSTO requires .NET Framework. Don't propose it.
- **WPF in TaskPane without `System.Xaml`** → ElementHost won't host the WPF control.
- **WPF DataBinding across the ElementHost boundary** → requires manual marshaling.
- **Storing COM objects in fields or static caches** → leaks; PowerPoint won't exit.
- **Releasing the `Application` RCW** → crash. Never release `ThisAddIn.Application` or anything routinely retrieved from it without care.
- **Calling `GC.Collect`/`WaitForPendingFinalizers` routinely** → masks problems, harms performance. Use only in justified shutdown paths.

## Build / Run / Debug

- Build: `/build` (MSBuild Debug). Building a VSTO project also registers the add-in for the local PowerPoint installation.
- Run: F5 in Visual Studio launches PowerPoint with the add-in loaded.
- Debug-register manually: [debug_setup.ps1](../../../debug_setup.ps1) writes the `HKCU` registry key pointing at `bin/Debug/Deckify.vsto`.
- Tests: `/test`.
- Logs: `%TEMP%\Deckify_Log.txt` (file-based, written by `Deckify.Common.Logger`).
