---
name: com-interop-discipline
description: Use when reading, writing, or refactoring code that touches a COM object (RCW). Triggers on Marshal.ReleaseComObject, Microsoft.Office.Interop.*, dynamic COM access, src/Interop/, or PowerPoint object-model property chains like ActivePresentation.Slides[i].Shapes.
---

# COM Interop Discipline

> **Note:** this skill describes the **target architecture**. Existing code in this repo is currently GC-only (zero `Marshal.ReleaseComObject` calls). The migration to ComScope discipline happens in Phase 2 — see [KNOWLEDGE.md](../../../KNOWLEDGE.md). Apply this skill when:
> - writing **new** COM-touching code (use ComScope from the start);
> - refactoring an identified hotspot listed in KNOWLEDGE.md;
> - reviewing a Phase-2 PR.
> Do **not** spontaneously rewrite existing GC-only code to ComScope outside of Phase 2.

## Core Principle

Every COM object you receive is yours — and you must release it. The garbage collector alone is not enough; un-released RCWs prevent PowerPoint from exiting cleanly.

## Pattern: ComScope

```csharp
public sealed class ComScope<T> : IDisposable where T : class
{
    public T Value { get; }
    public ComScope(T comObject) => Value = comObject;
    public void Dispose()
    {
        if (Value != null && Marshal.IsComObject(Value))
            Marshal.ReleaseComObject(Value);
    }
}
```

Usage:
```csharp
using var pres = new ComScope<Presentation>(app.ActivePresentation);
using var slides = new ComScope<Slides>(pres.Value.Slides);
using var slide = new ComScope<Slide>(slides.Value[1]);
using var shapes = new ComScope<Shapes>(slide.Value.Shapes);
// work with shapes.Value here
```

Each `using` releases its RCW at the end of the enclosing scope.

## No Chaining

NOT:
```csharp
var shape = app.ActivePresentation.Slides[1].Shapes[1];
```

Each property access creates a transient RCW that no one releases. Instead, assign every intermediate to its own `ComScope`.

## Loops over COM collections

The loop-body item is also a COM object — release it each iteration:

```csharp
using var slides = new ComScope<Slides>(pres.Value.Slides);
for (int i = 1; i <= slides.Value.Count; i++)
{
    using var slide = new ComScope<Slide>(slides.Value[i]);
    // process slide.Value
} // slide released each iteration
```

Cache primitive properties before the loop (`Count`, `Width`, `Name`) instead of re-reading them inside the loop.

## Verification

After every COM-touching change:
1. Load the add-in, run the affected action, unload.
2. Close PowerPoint.
3. Task Manager: is `POWERPNT.EXE` gone? If not → RCW leak.

## Common Mistakes

- **`ReleaseComObject` on a non-COM object** → exception. Guard with `Marshal.IsComObject`.
- **Double-release** on the same RCW → undefined behavior.
- **Releasing the `Application`-level COM object** in an add-in context → crash. Never release `ThisAddIn.Application`.
- **Capturing COM objects in lambdas, async continuations, or events** → references survive past the intended scope, leak. Marshal primitive IDs across boundaries instead.
- **Forgetting to release in a `finally` block** when an exception fires mid-iteration. (`using` handles this automatically.)
- **Returning a `ComScope.Value` from a method** → the caller has no contract for ownership. If a method must return a COM object, it must return the `ComScope` itself.

## What this skill does NOT prescribe

- Calling `GC.Collect` / `WaitForPendingFinalizers` routinely. Don't.
- Wrapping every primitive read (`shape.Width`, `slide.SlideID`). The chain only matters when the intermediate is itself a COM RCW.
- Refactoring stable, working GC-only code outside Phase 2 windows.
