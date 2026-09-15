# Implementation Plan — Issue #24: Typology area view — left panel: typology tree with calculated counts and the dynamic pie chart

> Part of the Typology area view tracking issue [#21](https://github.com/ZiolkowskiJakub/DiGi.GIS.WebAPI.UI/issues/21).
> **Labels:** `type: feature`, `priority: medium`, `ai: heavy`. Depends on #22 (closed, `7fcd75b`) and #23 (closed, `7070bef`) — **unblocked**.
> **Repo:** `DiGi.GIS.WebAPI.UI`, branch `0.8.9`, working tree clean.

## Context

The `/typology/view` page (#23) ships a resizable 3-column shell whose left panel is a placeholder card ("The typology legend appears here"). `POST /typology/buildings` (#22) answers the solved typology for the area. Nothing on the page calls it yet, and the left panel renders nothing. This issue fills the left panel: a nested tree of the solved typology (swatch + name + count per node, expand/collapse, click/keyboard selection) over a hand-rolled SVG pie of the children of the current selection, with a legend of counts and percentages. The selection is reported outward so #25 (dimming) and #26 (inspector) can consume it; #27 owns the full loading/error/empty-state orchestration.

## Issue validity — premises re-verified against the working tree (2026-09-15)

| Issue claim | Verified | Note |
|---|---|---|
| Solve DTO has `tree[]` nodes with `{path, name, color, count, children[]}` | **Partly wrong** | The DTO is `TypologyBuildingsViewModel { Root, Buildings }` — a single `Root` node (`ViewModels/TypologyTreeNodeViewModel.cs`: `Name, Description, Color, Path, Children`) and **no `Count`**. No `area`/`columns` members either. |
| Root = whole area | Correct | `SolveBuildingsAsync` calls `table.VisualTypology(filter, referenceCoreColumn, includeReferences: true)` without `typologyItem_Root`, so `Root` is the unnamed whole-area node (`Name` null, `Path` `[]`); its `Children` are the first-level buckets. |
| Node colour always present | **Not guaranteed** | `Color` is null when the rule maps no appearance (`Create/TypologyBuildingsViewModel.cs` → `Query.Hex(Query.Color(...))`). #21 promised a deterministic fallback; #22 left it to the view. |
| Load-modal aria pattern to copy | Correct | `wwwroot/js/typology.js:2091` rows: `role="option" tabindex="0" aria-selected`; `:2214-2230` click + Enter/Space (Space `preventDefault`), `setLoadSelection` toggles `aria-selected` + a selected class. |
| `gis-card`/`gltf-card` styles exist | Correct | `gis-theme.css:166` `.gis-card`; `:1091-1145` `.gltf-card` compact padding + collapsible title. `typology-left-panel` is a column flex with `overflow-y:auto` (`:2131`). |
| No chart library in the repo | Correct | Only three.js for the 3D viewer; the 2D stack (`administrative.js`) is hand-rolled SVG. |
| Solve endpoint is wired on the page | **No** | `typology-view.js` holds only resizers/toggles; nothing reads `sessionStorage['digiTypology.definition']` on the view page. |

Verdict: **still valid**, with two decisions taken (user-confirmed): (1) add `Count` to the DTO server-side; (2) ship a minimal solve fetch in this issue so the panel is verifiable live, to be superseded by #27.

## Implementation notes (post-execution)

- The solver files references on bucket nodes only, never on the root, so `Count` is `max(References.Count, sum of children)`: the root sums its children, a bucket keeps its own membership. The live county (0201, id 5, 33 687 rows) confirmed it: level-1 buckets summed to exactly the building list, and the root read 0 before the fix.
- The definition validator refuses a null colour, so the client-side fallback palette is reachable only through a document written elsewhere; verified with a canned DTO.
- The left-panel cards use 14 px side padding (not the card default 25 px): at the 150 px minimum a row's fixed parts overflowed by 2 px with the default.

## Decisions

| Decision | Choice |
|---|---|
| Node counts | Server-side: `TypologyTreeNodeViewModel.Count` = the solver node's `References.Count` (`includeReferences: true` stores every reference on the matched node and all its ancestors). A parent may exceed Σ children (rows the solver dropped at a lower level); the pie shows that remainder as one neutral slice so slices always sum to the selected node's count. |
| Colour fallback | Client-side `colorOf(node)`: `node.color` or the 12-entry palette of `typology.js:37` indexed by the node's last path index (deterministic, sibling-distinct). Exposed on the panel module so #25 paints dots identically. Root has no swatch. |
| Selection reporting | The panel module takes an `onSelectionChange(path, node)` callback **and** dispatches `document` `CustomEvent('typology:selectionchange', {detail:{path, node}})`; the bootstrap keeps `digiTypologyView.selection`. #27 may replace the state holder; the event contract stays. |
| Fetch | One bootstrap IIFE in `typology-view.js`: read the session definition, POST `/typology/buildings` (same fetch/error shape as `typology.js:1385-1400`), render. Error/empty text is plain in-panel copy; #27 replaces it. |
| Keyboard model | The issue's stated pattern: every row `tabindex="0"`, Enter/Space selects, click selects, clicking the selected row again clears; a separate `<button>` chevron per node expands/collapses (natively keyboard operable). Arrow-key roving focus is **not** required by the DoD and is left out. |
| Pie geometry | `viewBox="0 0 200 200"`, r=80 at (100,100); one `<path>` per slice (`M c L p1 A r r 0 large 1 p2 Z`), `<title>` tooltip per slice; <2 slices → empty-state text, no SVG. |
| Files | New `wwwroot/js/typology-panel.js` (tree + pie, classic script, one module object like `digiTypology`); CSS appended to the existing `typology-*` block in `gis-theme.css`; view markup in `View.cshtml`; DTO/factory/test touch for `Count`. No controller change, no new NuGet/CDN. |

## Files

```
DiGi.GIS.WebAPI.UI/DiGi.GIS.WebAPI.UI/
├── ViewModels/TypologyTreeNodeViewModel.cs      # + int Count (ctor param before children; XML docs)
├── Create/TypologyBuildingsViewModel.cs         # Flatten: count = visualTypology.References?.Count ?? 0
├── Views/Typology/View.cshtml                   # left aside: tree card + pie card; data-area-* attrs on .typology-shell; load typology-panel.js
├── wwwroot/js/typology-panel.js                 # NEW: digiTypologyPanel (tree render/select/clear, pie, legend, colorOf)
├── wwwroot/js/typology-view.js                  # + solve bootstrap IIFE (sessionStorage → POST → render; selection state + event)
└── wwwroot/css/gis-theme.css                    # + typology-tree-* / typology-pie-* rules inside the typology block (~2052-2268)
DiGi.Test/DiGi.GIS.WebAPI.UI.xUnit/Facts/
├── Create_TypologyBuildingsViewModel.cs         # root references propagated; assert Count on root/children; remainder case
└── TypologyBuildingsViewModel_Serialization.cs  # populate Count in the round-trip instance
```

## Design

### 1. DTO `Count` (C#)

- `TypologyTreeNodeViewModel`: add `public int Count { get; set; }` with `<summary>` "the number of buildings filed under this node — the solver's reference set, so a parent's count can exceed the sum of its children when rows resolve to no bucket at a lower level". Constructor becomes `(string? name, string? description, string? color, List<int> path, int count, List<TypologyTreeNodeViewModel>? children)`; `<param>` order mirrors it. Update the class `<summary>` (mentions the centroid join) to also name `Count`.
- `Create/TypologyBuildingsViewModel.cs` `Flatten`: `int count = visualTypology.References?.Count ?? 0;` passed at both `new TypologyTreeNodeViewModel(...)` sites. No other logic change. Explicit types, no `var`.
- Tests (build the library first — HintPath consumption, `dotnet build DiGi.GIS.WebAPI.UI.csproj -c Debug -m:1`):
  - `Create_TypologyBuildingsViewModel`: also `root.AddReference("A1"/"A2"/"B1")` (as the solver propagates), assert `rootNode.Count == 3`, `node_A.Count == 2`, `node_B.Count == 1`; add a case where root carries one extra reference (`"X"`) absent from every child → `Count 4`, `Buildings.Count 3` (documents the remainder the pie shows).
  - `TypologyBuildingsViewModel_Serialization`: set `Count` on every node in the instance and assert it after the round trip (adding a member means populating it in the existing fact).

### 2. Markup — `Views/Typology/View.cshtml` (left aside only; shell untouched)

```html
<div class="typology-shell" data-area-id="@Model.Id" data-area-code="@Model.Code" data-area-type="@((int)Model.AdministrativeArealType)">
…
<aside id="typology-left-panel" class="typology-left-panel">
    <div class="gis-card gltf-card typology-tree-card">
        <h3 class="typology-card-title">Typology
            <button type="button" id="typology-tree-clear" class="gis-button gis-button-icon gis-button-secondary" title="Clear selection" aria-label="Clear selection" disabled>&times;</button>
        </h3>
        <p id="typology-tree-status" class="typology-muted">Solving the typology for the area…</p>
        <div id="typology-tree" class="typology-tree" role="tree" aria-label="Typology tree" hidden></div>
    </div>
    <div class="gis-card gltf-card typology-pie-card">
        <h3 class="typology-card-title" id="typology-pie-title">Distribution</h3>
        <div id="typology-pie" class="typology-pie" aria-labelledby="typology-pie-title"></div>
        <p id="typology-pie-empty" class="typology-muted"></p>
        <ul id="typology-pie-legend" class="typology-pie-legend"></ul>
    </div>
</aside>
…
@section Scripts {
    <script src="~/js/typology-panel.js" asp-append-version="true"></script>
    <script src="~/js/typology-view.js" asp-append-version="true"></script>   @* after the panel: the bootstrap calls digiTypologyPanel *@
}
```

The type goes on the wire as the **integer** (`(int)Model.AdministrativeArealType`) — never a name (WebAPI Contracts). Razor encodes `@Model.Code`. Keep `ViewData` Title/Description/Robots and the right panel unchanged.

### 3. `wwwroot/js/typology-panel.js` — `const digiTypologyPanel = (function () { 'use strict'; … })();`

Public surface: `render(viewModel)`, `select(path)`, `clear()`, `selection()` → `{path, node}|null`, `colorOf(node)`, `setSelectionCallback(fn)`, `showStatus(text)` (for the bootstrap's error/empty copy).

Tree:
- `render` builds the DOM in one pass into a `DocumentFragment`; string building with `escapeHtml` (copy the 5-entity helper from `typology.js:980` — `typology.js` is not loaded on this page). Root row first (name "Whole area", no swatch), children nested in `<div role="group">`; every row: `<div class="typology-tree-row" role="treeitem" tabindex="0" aria-selected="false" aria-level="n" aria-expanded="true|false" data-path="0.2.1">` containing `<button class="typology-tree-toggle" aria-label="Collapse …">` (only when children exist; leaves get a spacer), `<span class="typology-tree-swatch" style="background:#…">`, `<span class="typology-tree-name">`, `<span class="typology-tree-count">`. Counts formatted with `toLocaleString()` (display only).
- Node index: `Map<string, node>` keyed by `path.join('.')` (`"”` = root) for O(1) selection lookup.
- Expand/collapse: toggle button click → `aria-expanded` flip + `hidden` on the sibling `role="group"`; button `aria-label` updates. Deeper levels start expanded (the chain is short); persist nothing.
- Selection (Load-modal pattern): one `click` listener and one `keydown` listener on `#typology-tree`; `closest('.typology-tree-row')`; keydown acts on Enter/Space with `preventDefault()`; toggle-button clicks `stopPropagation` so they never select. Selecting the already-selected row clears. `setSelection(path)` flips `aria-selected` + `typology-tree-row-selected` on every row (as `setLoadSelection`), enables/disables `#typology-tree-clear`, re-renders the pie, invokes the callback and dispatches the `typology:selectionchange` event on `document`. Escape inside the tree clears. `#typology-tree-clear` click clears.
- `colorOf(node)`: `node.color || palette[(node.path.length ? node.path[node.path.length - 1] : 0) % palette.length]`.

Pie (`renderPie(node)`, node = selection or root):
- Slices = `node.children` (each `{name, count, color}`) plus, when `node.count > Σ children.count`, one neutral slice `{name:'Not classified at the next level', count: remainder, color:'#9e9e9e'}`. Zero-count children are skipped for the arcs but still listed in the legend with 0 / 0.0 %.
- `< 2` slices with count > 0 → clear the SVG, `#typology-pie-empty` = "Select a node with two or more sub-typologies to chart its distribution." (leaf: "…is a leaf; nothing below it to chart."). Otherwise build `<svg viewBox="0 0 200 200" role="img" aria-label="Distribution of N buildings">` with one `<path fill=… class="typology-pie-slice" data-path=…>` per slice + `<title>name: count (p %)</title>`; angle from −90°, `large-arc` when the fraction > 0.5; a 100 % single-child case cannot occur (that is the <2 branch).
- Legend `<li>` per slice: swatch, name, `count.toLocaleString()`, `(count / total * 100).toFixed(1) + ' %'`. Title of the card gets the focused node's name in a muted suffix ("Distribution — Storeys 1-2").

### 4. Bootstrap — `wwwroot/js/typology-view.js` (append one IIFE; existing four untouched)

```js
// Solve bootstrap (#24): minimal; #27 owns loading/error/empty states and the shared page state.
(function () {
    const shell = document.querySelector('.typology-shell');
    if (!shell || typeof digiTypologyPanel === 'undefined') { return; }
    window.digiTypologyView = { viewModel: null, selection: null };
    let definition = null;
    try { definition = JSON.parse(window.sessionStorage.getItem('digiTypology.definition')); } catch (error) { /* absent or blocked: no definition carried */ }
    if (!definition || !Array.isArray(definition.levels)) { digiTypologyPanel.showStatus('No definition was carried to this page. Go back to the definition page and press Load.'); return; }
    const base = (window.AppBaseUrl || '/').replace(/\/$/, '');
    fetch(base + '/typology/buildings', { method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ definition: definition, id: parseInt(shell.dataset.areaId, 10), code: shell.dataset.areaCode || null, administrativeArealType: parseInt(shell.dataset.areaType, 10) }) })
      .then(response => { if (response.ok) { return response.json().then(vm => { window.digiTypologyView.viewModel = vm; digiTypologyPanel.setSelectionCallback((path, node) => { window.digiTypologyView.selection = path === null ? null : { path, node }; }); digiTypologyPanel.render(vm); }); }
                          return response.json().catch(() => null).then(body => digiTypologyPanel.showStatus(Array.isArray(body) ? body.join(' ') : statusText(response.status))); })
      .catch(() => digiTypologyPanel.showStatus('The request could not be sent.'));
})();
```
`statusText`: 404 → "The area has no buildings.", 413 → body text (server sends the actionable list), 503 → "The building data service is unavailable.", else "The typology could not be solved (HTTP n)." Wire names are camelCase (`definition`, `id`, `code`, `administrativeArealType`) — the default MVC options, exactly as `typology.js` posts the definition.

### 5. CSS — append inside the `typology-*` block of `gis-theme.css` (before the `@media (max-width:1100px)` rule)

`.typology-tree-card { flex: 1 1 auto; min-height: 0; display:flex; flex-direction:column }` with `#typology-tree { overflow-y:auto; min-height:0 }` so the tree scrolls and the pie card keeps its size; `.typology-pie-card { flex: 0 0 auto }`. Rows: flex, `gap 6px`, `padding 4px 6px`, `border-radius 4px`, `cursor:pointer`; indent by `aria-level` via `[role="group"] { padding-left: 14px }`; `.typology-tree-row:focus-visible { outline: 2px solid var(--primary-color); outline-offset: 1px }` (as `.gis-column-item:focus-visible`, `:2367`); `.typology-tree-row-selected { background: var(--primary-light); border-color: var(--primary-color) }` (as `.gis-typology-result-selected`, `:2654`); swatch 12×12 rounded; count `margin-left:auto; color: var(--text-muted); font-variant-numeric: tabular-nums`; toggle button 18×18 borderless with a rotating chevron. Pie: `.typology-pie svg { width:100%; height:auto; max-height: 220px; display:block }` — scales with any panel width (150–500 px clamp); legend `list-style:none`, rows like tree rows without interaction; `.typology-pie-slice:hover { opacity: .85 }`. Respect the `<1100px` stacking rule already present (no change needed).

## Guideline compliance

| Concern | Guideline | Where |
|---|---|---|
| Explicit types, target-typed `new`, collection expressions, block-scoped namespace, XML docs on every public member, `<param>` order = signature, zero warnings | Coding – General §1 | DTO + factory edits |
| Add a member ⇒ populate it in the existing serialization fact | Coding – Automatic Tests | `TypologyBuildingsViewModel_Serialization.cs` |
| Build the library before `dotnet test` (HintPath) | Coding – Automatic Tests | Verification |
| Enums as integers on the wire; `AreaView` query contract (`id`/`code`/`administrativearealtype`) untouched; no `_type` in the browser | Coding – WebAPI Contracts | `data-area-type` integer; no controller edit |
| No new NuGet/CDN; hand-rolled SVG | Issue / #21 | `typology-panel.js` |
| Interactive DoD verified in a real browser (Playwright, host shell, scratchpad, server killed in `finally`; rebuild before re-verifying `.cshtml`) | Coding – Browser Testing | Verification |
| Live check against `api.digiproject.uk` is read-only, sequential (the solve already is) | Coding – Deployed WebAPI §4 | Verification |
| Edits via the Edit tool (CRLF/BOM safety); check `git diff --ignore-cr-at-eol` before commit | Coding – General §1.14 / memory | Execution |
| Issue premise corrections posted as a comment with evidence (DTO shape, no `count`, nullable colour) — body not edited | GitHub – Issues §2 | Closing comment |
| Closing comment: commits, files, tests, live verification | GitHub – Issues §3 | Closing comment |

## Verification

1. `dotnet build DiGi.GIS.WebAPI.UI/DiGi.GIS.WebAPI.UI.csproj -c Debug -m:1` → zero warnings.
2. `dotnet test DiGi.Test/DiGi.GIS.WebAPI.UI.xUnit` → `Create_TypologyBuildingsViewModel` and `TypologyBuildingsViewModel_Serialization` green with the `Count` assertions.
3. Live (read-only): start the built app (`ASPNETCORE_URLS=http://localhost:5010`-style per Browser Testing §3), open `/typology`, build a 2-level definition (e.g. a unique-value column then a range column), Load a county (~30k buildings) → the view solves; tree shows root count = `Buildings.length` + remainder, expand/collapse works, swatches match the definition colours, fallback colours appear only for nodes the definition left uncoloured.
4. Playwright script in the scratchpad (host `python`, `channel="msedge"`, headless): seed `sessionStorage['digiTypology.definition']` via `page.evaluate` before `goto`, or intercept `POST /typology/buildings` with `page.route` and a canned DTO (3 levels, a node with a remainder, a node with a null colour) for deterministic asserts:
   - full chain depth rendered; per-row `aria-level`, swatch, name, count text equal to the DTO;
   - toggle button click and Enter on it collapse/expand (`aria-expanded`, group `hidden`);
   - `Tab` to a row + `Enter`/`Space` selects (`aria-selected="true"`, selected class, `:focus-visible` outline computed); clicking again clears; Escape clears; clear button enabled only while selected;
   - pie: after selecting a node with ≥2 children, `path` count equals slice count, legend counts equal `children[].count` (+ remainder), percentages sum to 100.0 ± 0.1; on clear the pie returns to the root distribution; a leaf selection shows the empty-state text and no `<svg>`;
   - `document` receives `typology:selectionchange` with the path on select and `null` on clear;
   - drag the left resizer to 150 px and 500 px: no horizontal overflow in the aside, the SVG width tracks the panel; `<1100px` viewport still stacks (unchanged shell behaviour);
   - no-definition visit: status copy shown, no fetch issued; 413/503 routes: server text surfaces in the status line.
5. Screenshot of the solved county for the closing comment.

## Out of scope / hand-offs

- Dimming of map points and the building grid consume `typology:selectionchange` / `digiTypologyView.selection` (#25, #26). Full loading/error/empty-state UI and the definitive page-state object (#27) — the bootstrap here is deliberately minimal and documented as such.
- Pie slice/legend click-to-drill and arrow-key roving focus in the tree: optional follow-ups, not in the DoD.
- `Count` counts references the solver stored; buildings dropped at a lower level are counted on their last matching ancestor but are absent from `Buildings[]` (a #22 property, surfaced by the remainder slice — noted in the closing comment).
