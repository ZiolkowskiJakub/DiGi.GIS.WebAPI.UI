# Implementation Plan — Issue #23: Typology area view — layout shell (3-column resizable split, left open / right closed)

> Part of the Typology area view tracking issue [#21](https://github.com/ZiolkowskiJakub/DiGi.GIS.WebAPI.UI/issues/21).
> **Labels:** `type: feature`, `priority: medium`, `ai: standard`
> **Status (at time of writing):** **Not yet implemented.** Verified against the working tree — the issue's premises still hold:
> - `Views/Typology/View.cshtml` is still the "in development" hero-card stub (area context only).
> - `wwwroot/js/typology-view.js` does not exist.
> - No `typology-*` layout CSS classes exist (the `gis-typology-*` definition-page classes are a different feature).
> - `TypologyViewViewModel` is still the context-only stub; `TypologyController.AreaView` still renders the stub.
>
> This is a **front-end shell** task: one Razor view body, a CSS block, a standalone JS file, and a (likely no-op) ViewModel touch. **No query/contract change and no new C# business logic.**
>
> **Audit (double-check pass):** the context strip must sit **above** the 3-column row, not as a flex child of it (see §5.1); the `typology-*` class/ID names are collision-free and `gltf-viewer.js` is not loaded on this page, so there is no interference (see §3, §7).

---

## 1. Goal

Replace the stub body of `Views/Typology/View.cshtml` with the shell that shares the 3D Viewer design language but with **inverted panel defaults**:

```
┌───────────────────────────────────────────────────────────────────────────┐
│  [ header strip: area type + code + id ]  [ Back to the definition page ] │
├──────────────┬──────────────────────────────────────────┬─────────────────┤
│  left aside  │        center viewport (SVG mount)        │  right aside    │
│   OPEN       │   (empty placeholder; colouring is a      │   CLOSED        │
│              │    later sub-issue of #21)                │                 │
│  resizer ‖   │    [◀ toggle]                  [▶ toggle] │  resizer ‖      │
└──────────────┴──────────────────────────────────────────┴─────────────────┘
```

The mechanics are ported from the 3D viewer (`gltf-layout` row, drag resizers with min/max clamps + `localStorage` persistence, collapsed-state modifier classes, aria-wired toggles). The real panel *content* is **out of scope** (later sub-issues of #21); this issue ships only the resizable/toggleable shell that mounts to them.

---

## 2. Guideline Compliance Checklist

| Concern | Guideline | Where |
|---|---|---|
| **Do NOT rename `AreaView`'s `id`/`code`/`administrativearealtype`** — a renamed query parameter breaks clients with no compile error and no runtime error (ASP.NET silently ignores an unknown parameter and returns the unfiltered result) | Coding – WebAPI Contracts §1 | §5.6 (action signature frozen) |
| `CancellationToken` last; `Async` suffix; XML `<param>` order mirrors signature | Coding – General §1.7–1.8 | §5.6 (unchanged action) |
| XML `<summary>` on any new public member (ViewModel) | Coding – General | §5.5 |
| Zero compiler warnings | Coding – General §1.4 | §6 Verification |
| English only (identifiers, comments, markup text) | Coding – General §1.1 | all new files |
| Area is keyed by `id`; `code` is **not** a key — the shell only *displays* the context, it performs no lookup | Coding – GIS Administrative Data | §5.5/§5.6 (no server lookup added) |
| Manual browser verification of front-end behaviour (no xUnit fact for pure markup/CSS/JS); `dotnet build` is the automated gate | Coding – Automatic Tests / Deployed WebAPI | §6 |
| Line-ending discipline on scripted edits (edit bytes, not lines) — only if the edit is scripted | Coding – General §1.14 | §6 (prefer `edit_file_tool`) |

**Out of scope for this issue** (later sub-issues of #21): the legend/properties panel *content*, the SVG colouring renderer, and any new server data action. The shell degrades gracefully when no definition is carried in `sessionStorage`.

---

## 3. Verified Current State (premise check)

Everything the issue references is present and matches:

| Premise | Verified location |
|---|---|
| `gltf-layout` row + panel/resizer/toggle mechanics | `Views/GLTF/GLTFSceneView.cshtml` (markup), `wwwroot/js/gltf-viewer.js` (logic), `wwwroot/css/gis-theme.css` (style) |
| Right-side resizer IIFE (clamp 200–600, key `gltf-side-panel-width`) | `gltf-viewer.js:481–529` |
| Left-side resizer IIFE (clamp 150–500, key `gltf-left-panel-width`) | `gltf-viewer.js:533–585` |
| Right toggle IIFE (`gltf-panel-collapsed`, `aria-expanded`) | `gltf-viewer.js:284–299` |
| Left toggle IIFE (`gltf-left-panel-collapsed`, `aria-expanded`, dispatches `resize`) | `gltf-viewer.js:301–322` |
| Header/footer collapse dispatches `resize` | `Views/Shared/_Layout.cshtml:94–101, 171–186` (`.gis-layout-collapsed`) |
| `.gltf-layout` height + collapse height | `gis-theme.css:1426–1437, 1917–1929` |
| `.gltf-resizer` handle + `.gltf-*` panel/toggle classes | `gis-theme.css:1450–1499, 1931–2048` |
| `noindex, follow` on the stub | `Views/Typology/View.cshtml:8` |
| Definition travels in `sessionStorage` (not a server session); redirect carries only `id`/`code`/`administrativearealtype` | `wwwroot/js/typology.js:2158–2168` |
| Definition-page responsive stacking at `max-width: 1100px` | `gis-theme.css:2075–2083` |

**Collision / interference checks (audit):**
- **No `typology-*` CSS class exists yet.** The definition page uses `gis-typology-*` (`gis-theme.css:2052–2413`); the load modal uses `typology-load-*` / `typology-import-*` / `typology-export-*` **IDs** (`Views/Typology/Start.cshtml`). My new classes (`typology-layout`, `typology-shell`, `typology-left-panel`, `typology-side-panel`, `typology-resizer`, `typology-panel-toggle`, `typology-viewport`, `typology-context`) and IDs (`typology-left-panel`, `typology-left-resizer`, `typology-viewport`, `typology-left-panel-toggle`, `typology-panel-toggle`, `typology-resizer`, `typology-side-panel`) are all collision-free.
- **`gltf-viewer.js` is NOT loaded on the Typology page.** It is referenced only by `Views/GLTF/GLTFSceneView.cshtml` and `Views/Communication/CommunicationSceneView.cshtml` (per-page `@section Scripts`), not in `_Layout` or any shared partial. Global scripts are only `_Layout`'s inline menu script and `wwwroot/js/user.js`. Even if it were loaded, its IIFEs guard on `#gltf-*` IDs that won't exist here. **No interference.**
- **`gis-theme.css` is linked globally** (`_Layout.cshtml:24`), so the new `typology-*` block in that file is picked up automatically.

**Latent defect to avoid porting:** the existing **left** resizer calls `leftPanel.getBClientRect()` (missing the `o`) at `gltf-viewer.js:560` and `:579`, whereas the **right** resizer correctly uses `getBoundingClientRect()` (`:505`, `:524`). The left drag handler therefore throws a `TypeError` on `mousedown` in the 3D viewer. **The port to `typology-view.js` must use `getBoundingClientRect()` in both resizers.** (The 3D viewer's own left resizer is a separate defect — flag it, do not fix it under this issue.)

---

## 4. New & Modified Files

```
DiGi.GIS.WebAPI.UI/
├── Views/Typology/
│   └── View.cshtml                      # BODY REPLACED: shell (keep @model, ViewData, noindex)
├── wwwroot/
│   ├── js/
│   │   └── typology-view.js             # NEW: resizer + toggle logic ported from gltf-viewer.js
│   └── css/
│       └── gis-theme.css                # + typology-* class block (new section, beside the gltf block)
└── ViewModels/
    └── TypologyViewViewModel.cs         # Likely NO-OP (Id/Code/Type/TypeName already carry the strip)
```

No controller change. No new NuGet. No new test project files (pure front-end).

---

## 5. Detailed Design

### 5.1 HTML shell — `Views/Typology/View.cshtml`

Keep the `@model`, the `ViewData` block (Title/Description/Robots `noindex, follow`), and replace the stub hero card with the shell. Wrap in `gis-panel gis-panel-full` (same as the 3D viewer) so it stretches to the window.

**Structure rule (the fix from the audit):** the `.typology-layout` row is a **flex row**. The header strip must therefore be a **sibling above the row inside a column wrapper** (`.typology-shell`) — *not* a child of the row. If the strip were placed inside `.typology-layout`, `display:flex` would lay it out as a fifth narrow column on the far right instead of a top strip. The wrapper also keeps the strip + row together within one viewport-height budget.

```html
<div class="gis-panel gis-panel-full">
    @* Column shell: the area-context strip on top, then the flexing 3-column row. *@
    <div class="typology-shell">

        @* Header strip: the area context (type + code + id). Always rendered — this is the
           "keep the area-context fallback for a missing definition" requirement. *@
        <div class="typology-context">
            <span class="typology-context-item"><span class="typology-context-label">Type</span><span class="typology-context-value">@Model.AdministrativeArealTypeName</span></span>
            <span class="typology-context-item"><span class="typology-context-label">Code</span><span class="typology-context-value">@Model.Code</span></span>
            <span class="typology-context-item"><span class="typology-context-label">Id</span><span class="typology-context-value">@Model.Id</span></span>
            <a href="~/typology" class="gis-button typology-context-back">Back to the definition page</a>
        </div>

        @* Inverted default: right panel CLOSED → row carries typology-panel-collapsed; left panel OPEN. *@
        <div class="typology-layout typology-panel-collapsed">

            @* LEFT aside — OPEN by default (the definition/legend panel; content is a later sub-issue). *@
            <aside id="typology-left-panel" class="typology-left-panel">
                <div class="gis-card typology-card">
                    <h3 class="typology-card-title">Definition</h3>
                    <p class="typology-muted">The typology legend appears here.</p>
                </div>
            </aside>

            @* Left resizer *@
            <div id="typology-left-resizer" class="typology-resizer" role="separator" aria-orientation="vertical" aria-label="Resize left panel"></div>

            @* CENTER viewport — the SVG mount point (colouring is a later sub-issue of #21).
               The two toggles dock in its corners, exactly like the 3D viewer container. *@
            <div id="typology-viewport" class="typology-viewport">
                <div id="typology-viewport-empty" class="typology-viewport-empty">
                    <p class="typology-muted">Buildings will be drawn here in the colours of the defined typology.</p>
                </div>

                @* Left toggle: OPEN by default → aria-expanded="true", label "Hide panel". *@
                <button type="button" id="typology-left-panel-toggle" class="typology-panel-toggle typology-left-panel-toggle"
                        title="Hide panel" aria-label="Hide panel" aria-controls="typology-left-panel" aria-expanded="true">
                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6"/></svg>
                </button>

                @* Right toggle: CLOSED by default → aria-expanded="false", label "Show panel". *@
                <button type="button" id="typology-panel-toggle" class="typology-panel-toggle typology-right-panel-toggle"
                        title="Show panel" aria-label="Show panel" aria-controls="typology-side-panel" aria-expanded="false">
                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="m9 18 6-6-6-6"/></svg>
                </button>
            </div>

            @* Right resizer *@
            <div id="typology-resizer" class="typology-resizer" role="separator" aria-orientation="vertical" aria-label="Resize right panel"></div>

            @* RIGHT aside — CLOSED by default (the properties/results panel; content is a later sub-issue). *@
            <aside id="typology-side-panel" class="typology-side-panel">
                <div class="gis-card typology-card">
                    <h3 class="typology-card-title">Properties</h3>
                    <p class="typology-muted">Building properties appear here when selected.</p>
                </div>
            </aside>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/typology-view.js" asp-append-version="true"></script>
}
```

**Notes:**
- `typology-view.js` has **no module imports** (unlike the three.js `gltf-viewer.js`), so it loads as a classic `<script>`, not `type="module"`. It runs at parse time via IIFEs; the section renders after `@RenderBody()` (after `user.js`), so all targets exist — the same pattern the 3D viewer relies on.
- `@Model.Code` / `@Model.Id` / `@Model.AdministrativeArealTypeName` are Razor HTML-encoded by default — correct for display; no `Html.Raw` needed. A `null` `Code` renders as an empty value under its label (acceptable for a shell).
- The two side panels stay **definition-agnostic**: they render a header + an empty-state line whether or not a definition is in `sessionStorage`. Placeholder copy is English-only and clearly labelled as a shell placeholder.
- `role="separator" aria-orientation="vertical"` is the correct annotation for a vertical resize divider (a cheap a11y correctness win over the 3D viewer, which omits `aria-orientation`). See §5.4 for the keyboard decision.

### 5.2 Inverted defaults — the one behavioural delta vs the 3D viewer

The toggle/resizer CSS and JS are symmetric (they only *toggle* a class and read the resulting state), so the only difference from the 3D viewer is the **initial** state, set in the HTML:

| | 3D Viewer (`gltf-*`) | Typology (`typology-*`) |
|---|---|---|
| Row class on load | `gltf-layout gltf-left-panel-collapsed` | `typology-layout typology-panel-collapsed` |
| Left panel | closed | **open** |
| Right panel | open | **closed** |
| Left toggle | `aria-expanded="false"`, "Show controls" | `aria-expanded="true"`, "Hide panel" |
| Right toggle | `aria-expanded="true"`, "Hide panel" | `aria-expanded="false"`, "Show panel" |

### 5.3 CSS — new `typology-*` block in `wwwroot/css/gis-theme.css`

Add a new section beside the existing `gltf-*` block (~lines 1419–2048), reusing the theme variables (`--primary-color`, `--bg-card`, `--text-muted`, `--text-dark`, `--border-color`). Mirror the `gltf-*` rules, renamed:

```css
/* Typology area view shell (#23): the 3D viewer's 3-column mechanics, inverted defaults. */
.typology-shell {
    display: flex;
    flex-direction: column;
    gap: 12px;
    height: calc(100vh - 180px);
    min-height: 480px;
}

/* Header strip: the area context across the top (the flexing child below takes the rest). */
.typology-context { display: flex; flex-wrap: wrap; align-items: center; gap: 16px; }
.typology-context-item { display: flex; flex-direction: column; gap: 2px; }
.typology-context-label { font-size: 0.75rem; color: var(--text-muted); }
.typology-context-value { font-weight: 600; color: var(--text-dark); }
.typology-context-back { margin-left: auto; }        /* right-align the back button */

/* The 3-column row (the flexing child of the shell). */
.typology-layout {
    display: flex;
    gap: 0;
    align-items: stretch;
    flex: 1 1 auto;
    min-height: 0;
    transition: height 0.3s ease;
}

/* Center viewport: the flexing SVG mount point between the two docked panels. */
.typology-viewport {
    position: relative;
    flex: 1 1 auto;
    min-width: 0;
    height: 100%;
    border-radius: 8px;
    overflow: hidden;
    background: #171a21;
}

    .typology-viewport-empty {
        position: absolute;
        inset: 0;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .typology-muted { color: var(--text-muted); font-size: 0.85rem; }
    .typology-card-title { font-weight: 600; }

/* Docked side panels (identical metrics to .gltf-side-panel / .gltf-left-panel). */
.typology-left-panel { flex: 0 0 280px; display: flex; flex-direction: column; gap: 12px; height: 100%; overflow-y: auto; }
.typology-side-panel { flex: 0 0 340px; display: flex; flex-direction: column; gap: 12px; height: 100%; overflow-y: auto; }

/* Splitter handle (identical to .gltf-resizer). */
.typology-resizer { width: 16px; background: transparent; cursor: col-resize; flex: 0 0 auto; position: relative; user-select: none; z-index: 10; }
    .typology-resizer::after { content: ''; position: absolute; top: 0; left: 50%; transform: translateX(-50%); width: 2px; height: 100%; background: rgba(0, 0, 0, 0.05); transition: background 0.2s ease, width 0.2s ease; }
    .typology-resizer:hover::after,
    .typology-resizer.is-dragging::after { background: var(--primary-color); width: 4px; }

/* Corner toggles: base metrics on the shared class; each corner class anchors its side (mirrors .gltf-panel-toggle / .gltf-left-panel-toggle). */
.typology-panel-toggle { position: absolute; top: 10px; z-index: 5; width: 34px; height: 34px; display: inline-flex; align-items: center; justify-content: center; padding: 0; background: rgba(255, 255, 255, 0.92); color: var(--text-dark); border: 1px solid rgba(0, 0, 0, 0.12); border-radius: 8px; cursor: pointer; transition: background 0.2s ease, color 0.2s ease; }
.typology-left-panel-toggle  { left: 10px; }
.typology-right-panel-toggle { right: 10px; }
    .typology-panel-toggle:hover { color: var(--primary-color); background: #ffffff; }
    .typology-panel-toggle svg { transition: transform 0.2s ease; }

/* Collapsed-state modifiers (mirror .gltf-panel-collapsed / .gltf-left-panel-collapsed). */
.typology-layout.typology-panel-collapsed .typology-side-panel { display: none; }
.typology-layout.typology-panel-collapsed #typology-resizer { display: none; }
.typology-layout.typology-panel-collapsed .typology-right-panel-toggle svg { transform: rotate(180deg); }
.typology-layout.typology-left-panel-collapsed .typology-left-panel { display: none; }
.typology-layout.typology-left-panel-collapsed #typology-left-resizer { display: none; }
.typology-layout:not(.typology-left-panel-collapsed) .typology-left-panel-toggle svg { transform: rotate(180deg); }

/* Header/footer collapse (mirror .gis-layout-collapsed .gltf-layout). */
.gis-layout-collapsed .typology-shell { height: calc(100vh - 40px); }

/* Responsive stacking below ~1100 px, like the definition page (#23 DoD). */
@media (max-width: 1100px) {
    .typology-shell { height: auto; }
    .typology-layout { flex-direction: column; }
    .typology-resizer { display: none; }
    .typology-left-panel, .typology-side-panel { height: auto; overflow: visible; }
    .typology-viewport { min-height: 320px; }
}
```

> The block is the **design intent**; final metrics should be copied from the live `gltf-*` rules so the two pages read as one product. Deliberate deviations: the inverted initial classes, the `typology-*` naming, the `.typology-shell` column wrapper (so the strip is above the row), and the responsive stacking.

### 5.4 JS — NEW `wwwroot/js/typology-view.js`

Standalone classic script, no imports. Port the four IIFEs from `gltf-viewer.js` (right resizer `:481–529`, left resizer `:533–585`, right toggle `:284–299`, left toggle `:301–322`), with three changes:

1. **Element IDs** → `#typology-resizer`/`#typology-side-panel`, `#typology-left-resizer`/`#typology-left-panel`, layout selector `.typology-layout`, toggles `#typology-panel-toggle`/`#typology-left-panel-toggle`.
2. **Collapsed classes** → `typology-panel-collapsed` / `typology-left-panel-collapsed`.
3. **Storage keys** → `typology-side-panel-width` and `typology-left-panel-width` (the issue's suggested keys; keep the 3D viewer's `gltf-*` keys untouched so the two pages persist independently).

**Fix the ported left resizer:** use `getBoundingClientRect()` (do **not** copy `getBClientRect()` from `gltf-viewer.js:560/579`).

Clamps: keep the 3D viewer's values — right panel `200–600`, left panel `150–500`. Both resizers and both toggles `window.dispatchEvent(new Event('resize'))` on change, so the header/footer collapse in `_Layout` and any future SVG renderer re-fit through the same channel.

**Keyboard / a11y decision (conscious, not an oversight):** the toggles are `<button type="button">` — natively focusable, Enter/Space triggers `click`, so they are keyboard operable (satisfies the DoD). The resizers are **mouse-drag only** — the 3D viewer's resizers are too, and the DoD only requires the *toggles* to be keyboard operable. Full keyboard resize (WAI-ARIA "resize handle": `tabindex="0"` + arrow-key handlers) is a deliberate non-goal to match the existing product; if it is ever wanted, it is a follow-up, not part of this shell.

Skeleton:

```js
// Typology area view shell (#23): resizer + toggle logic ported from gltf-viewer.js,
// typology-* ids/classes, its own localStorage keys. No module imports.
(function () {
    const resizer = document.getElementById('typology-resizer');
    const sidePanel = document.getElementById('typology-side-panel');
    if (!resizer || !sidePanel) return;

    const saved = localStorage.getItem('typology-side-panel-width');
    if (saved) { const w = parseInt(saved, 10); if (w >= 200 && w <= 600) sidePanel.style.flex = `0 0 ${w}px`; }

    resizer.addEventListener('mousedown', (mouseDownEvent) => {
        mouseDownEvent.preventDefault();
        resizer.classList.add('is-dragging');
        const startX = mouseDownEvent.clientX;
        const startWidth = sidePanel.getBoundingClientRect().width;
        const onMouseMove = (e) => { const nw = Math.max(200, Math.min(600, startWidth - (e.clientX - startX))); sidePanel.style.flex = `0 0 ${nw}px`; window.dispatchEvent(new Event('resize')); };
        const onMouseUp = () => { resizer.classList.remove('is-dragging'); document.removeEventListener('mousemove', onMouseMove); document.removeEventListener('mouseup', onMouseUp); localStorage.setItem('typology-side-panel-width', sidePanel.getBoundingClientRect().width); window.dispatchEvent(new Event('resize')); };
        document.addEventListener('mousemove', onMouseMove);
        document.addEventListener('mouseup', onMouseUp);
    });
})();
// (same IIFE for #typology-left-resizer / #typology-left-panel with +deltaX and clamp 150–500,
//  using getBoundingClientRect() — do NOT copy the getBClientRect() typo)
// (same IIFEs for the two toggles: layout.classList.toggle('typology-panel-collapsed' | 'typology-left-panel-collapsed'),
//  set aria-expanded + aria-label/title, dispatch resize)
```

### 5.5 ViewModel — `ViewModels/TypologyViewViewModel.cs`

The strip renders `AdministrativeArealTypeName`, `Code`, `Id` — **all already present** on the current model. The expected change is therefore a **no-op**.

**The "area name" gap (decided):** the issue says "area name/type header strip," but the Load-modal redirect carries **no name** — only `id`/`code`/`administrativearealtype` (`typology.js:2166–2168`). A human-readable name would require a new server lookup keyed by `id` (never `code` — Coding – GIS Administrative Data). That is beyond a layout shell. **Decision:** ship the strip with type + code + id now (what the query already carries, matching the current stub); a name lookup is a follow-up only if the panel-content sub-issue needs it. Any member added later must carry an XML `<summary>` and an ordered `<param>` doc.

### 5.6 `TypologyController.AreaView` — contract frozen

**Do not change the signature or query names.** The current binding is already correct and must stay:

```csharp
[HttpGet("view")]
public IActionResult AreaView(
    [FromQuery(Name = "id")] int id,
    [FromQuery(Name = "code")] string? code = null,
    [FromQuery(Name = "administrativearealtype")] AdministrativeArealType? administrativeArealType = null)
```

- Renaming/removing any of `id`/`code`/`administrativearealtype` breaks the `typology.js` redirect and any saved/bookmarked URL **with no compile error and no runtime error** (Coding – WebAPI Contracts §1). Keep the nullable `AdministrativeArealType` binding and the `id <= 0 || type is null || Undefined` guard exactly as-is.
- The action body already returns `View("View", …)`; the only thing that changes is what that view renders. **No controller edit is required.**

---

## 6. Verification (Definition of Done mapping)

| DoD item | How it is met / verified |
|---|---|
| Shell renders left open / right closed; both panels toggle + resize with min/max clamps; widths persist across reload | Manual browser: load `/typology/view?id=…&administrativearealtype=…`; confirm left visible, right hidden, strip on top; drag both resizers (clamps 150–500 / 200–600); reload and confirm widths persist via `localStorage` keys `typology-left-panel-width` / `typology-side-panel-width` |
| `aria-expanded`/`aria-controls` correct through collapse/expand; toggles keyboard operable | Manual + devtools: toggle each side, assert `aria-expanded` flips and the chevron rotates; Tab to a toggle and press Enter/Space |
| Header/footer collapse identical to the 3D viewer; center viewport re-fits on the dispatched `resize` | Manual: click the `gis-layout-toggle`; confirm the `.typology-shell` height animates to `calc(100vh - 40px)` and the toggles/resizers dispatch `resize` |
| `AreaView` signature and query names unchanged; a no-definition visit still shows the area context | `git diff` shows no controller change; load the view URL directly with an empty `sessionStorage` and confirm the `typology-context` strip renders |
| `dotnet build` — zero warnings; responsive stacking below ~1100 px like the definition page | `dotnet build` clean; resize the browser to <1100 px and confirm the three columns stack (left → viewport → right) and resizers hide |

**Build gate:** `dotnet build` (zero warnings) is the automated check; the only C#-build-affected files are the Razor `View.cshtml` (must stay well-formed) and, if touched, the ViewModel. The front-end behaviour is verified by manual browser testing (there is no C# logic to xUnit in this issue). Prefer `edit_file_tool` for all edits to avoid the line-ending rewrite trap (Coding – General §1.14).

---

## 7. Risks / Open questions

1. **`code` is not a key.** A future human-readable area *name* must resolve by `id`, never `code` (Coding – GIS Administrative Data). Out of scope here — the shell only echoes the context the query already carries (see §5.5 decision).
2. **Two panels with empty content.** Until the follow-up content sub-issues land, the side panels show placeholder cards. Intended (shell-only), but the copy must make clear the content is not yet implemented, matching the current stub's honesty.
3. **Don't copy the `getBClientRect()` typo.** The 3D viewer's left resizer is currently broken by it; the port must use `getBoundingClientRect()`. Consider separately reporting the 3D viewer defect (a different issue, not this one).
4. **CSS/JS duplication vs reuse.** The issue explicitly asks for *new* `typology-*` classes rather than reusing `gltf-*`. This duplicates ~120 lines of CSS and ~120 lines of JS. The trade-off is decoupling: the Typology page does **not** load the heavy three.js `gltf-viewer.js`, keeps independent defaults, independent `localStorage` keys, and independent collapse state. Accept the duplication per the issue's stated scope. Verified: the names are collision-free and `gltf-viewer.js` is not on this page (§3).
5. **Resizers are mouse-only.** The DoD requires only the *toggles* to be keyboard operable; the resizers match the 3D viewer (mouse drag). Full keyboard resize is a conscious non-goal (§5.4). Flag if a11y reviewers push back.
6. **Responsive stacking order.** At <1100 px the panels stack (left → viewport → right) and resizers hide; the corner toggles remain usable for show/hide. Confirm the visual order and the `min-height: 320px` viewport with a quick manual check — the issue leaves the exact stacked layout open.
