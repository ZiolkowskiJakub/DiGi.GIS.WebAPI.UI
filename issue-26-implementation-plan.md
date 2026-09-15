# Implementation Plan — Issue #26: Typology area view — right panel: typology property inspector, filterable building grid and building info

> Part of tracking issue [#21](https://github.com/ZiolkowskiJakub/DiGi.GIS.WebAPI.UI/issues/21). **Labels:** `type: feature`, `priority: medium`, `ai: heavy`. Depends on #22 (`7fcd75b`), #23 (`7070bef`) — both closed; #24 (`12dfb91`), #25 (`0025578`) and WebAPI#34 also closed → **unblocked**.
> **Repo:** `DiGi.GIS.WebAPI.UI`, branch `0.8.9`, working tree clean at planning time.

## Context

The `/typology/view` page has a shipped left panel (tree + pie, #24) and centre map (dots + dimming, #25). The right `<aside id="typology-side-panel">` is still the #23 placeholder card ("Building properties appear here when selected"), closed by default. `typology-map.js` already dispatches `typology:buildingselect` on dot click with nobody listening — reserved for this issue. This issue fills the panel with three cards: the selected typology node's properties, a windowed, text-filterable grid of its buildings (30k+ rows must stay responsive), and an info card for the building picked from the grid or the map, with `Details` / `3D Viewer` links. #27 later owns loading/error orchestration.

## Issue validity — premises re-verified (2026-09-15)

| Issue claim | Verified | Note |
|---|---|---|
| Right panel closed by default, opens on first selection | Correct / **no API** | Closed via `typology-panel-collapsed` class on `.typology-layout` (`View.cshtml:21`, CSS `gis-theme.css:2293-2301`). The toggle IIFE (`typology-view.js:124-140`) only reacts to clicks — there is no programmatic open/close; one must be added. |
| Breadcrumb "over the chain columns (`columns[]` of the DTO)" | **Wrong** | DTO is `{root, buildings}` (`ViewModels/TypologyBuildingsViewModel.cs`); no `columns[]`. Node `name` already reads "column name + rule text". Breadcrumb = ancestor node names along the `path`; level column names are available client-side from `sessionStorage['digiTypology.definition'].levels[i].name` (level i ↔ path index i), already parsed in `typology-view.js:181-188`. |
| Info card shows `id`; 3D link `/buildingmodel/buildingmodelbyid?id=&countyid=` | **DTO gap** | `TypologyBuildingViewModel` carries `Reference, CountyId, Path` only. Live catalog `GET gis/BuildingData/columns` has `database_id` ("Database Id", long, Identity), populated (9157 for `000313ED-…` on part 5) and equal to `Building2DReference.Id` from `building2Dreferencebyreference` — so the id can be projected in the existing solve fetch at no extra request. **Decision (user-confirmed): add `Id` to the DTO.** |
| x/y from the centroid lookup | Correct | `digiTypologyMap.pointOf(reference, countyId)` → `{x, y, path}` (EPSG:2180 m), `typology-map.js:283-286`. |
| `GET /building2D/itembyid` partial reusable | Correct but unsuitable | `_Building2DView` is a wide master-detail layout needing `building.js`; a 340 px panel uses **links** instead: `Details` → `/building2D/detailsbyreference?reference=&countyid=` (full page, also accepts `x`/`y` when countyid absent), `3D Viewer` → `/buildingmodel/buildingmodelbyid?id=&countyid=`. |
| Windowed rendering needed | Correct | No virtualised list primitive exists in the repo (only `building.js initTablePager`). County 0201 (area id 5) solves 33 687 rows. |
| `typology:selectionchange` contract | Correct | `document` event, `detail: {path:int[]|null, node|null}`, dispatched by `digiTypologyPanel.setSelection` (`typology-panel.js:254`). |
| Buildings per node | Nuance | `buildings[]` holds **leaf** buckets only (`Create/TypologyBuildingsViewModel.cs:44-58`); a non-leaf node's grid = every building whose `path` starts with the node's path (same `isUnder` as the map). A node whose `count` > 0 but with no leaf rows shows "This typology lists no buildings." |

Verdict: **still valid.** Two corrections to post as issue comments (never body edits, `GitHub - Issues.md` §2): no `columns[]` in the DTO; `Id` added to the DTO from `database_id`.

## Implementation notes (post-execution)

- The Browser pane of the desktop app runs hidden (0x0 viewport): `scroll` events and `requestAnimationFrame` never fire there, so the windowed grid cannot be exercised in it. The DoD ran through the guideline's Playwright recipe (Edge headless, host python, script in the scratchpad) - 34 assertions green; the only console error is the pre-existing `favicon.ico` 404.
- The first slice was rendered while the panel was still collapsed (grid height 0 -> 21 rows) and the follow-up `resize` re-render depends on `requestAnimationFrame`; the inspector now opens the panel before scoping the grid so the first render sees the real height.
- County 0201 (id 5, 33 687 rows): every DTO entry carried a `database_id` (first row 9157 = `Building2DReference.Id`), `Storeys [3, 5]` = 2 002 rows, 32 DOM rows at a 305 px grid, last-row index correct at the bottom of a 52 052 px spacer.

## Decisions

| Decision | Choice |
|---|---|
| Building id | Server-side: project `database_id`, new `long Id` on `TypologyBuildingViewModel` (0 = unknown). 3D link hidden when `Id` is 0. |
| Dot click outside the tree selection | **User-confirmed:** call `digiTypologyPanel.select(building.path)` so the tree/pie/dimming/grid re-scope, then highlight the row. An unclassified dot (`path === null`) only fills the info card. |
| Panel open/close | Replace the anonymous right-toggle IIFE in `typology-view.js` with a named `digiTypologyLayout` module exposing `setRightPanelCollapsed(bool)` / `isRightPanelCollapsed()` (keeps aria/title/resize dispatch in one place). Inspector opens the panel on every non-null selection while collapsed, closes + resets on null. |
| Grid windowing | Fixed 26 px rows; scroll container → spacer of `rows × 26px` → absolutely positioned slice translated to `first × 26px`, re-rendered on `scroll` (rAF-throttled) and `resize`; overscan 10. DOM never holds more than visible + 20 rows. |
| Filter | Case-insensitive substring on `reference` over a precomputed lowercase array; 150 ms debounce; count label "n of m". |
| Breadcrumb | Ancestor node names (root = "Whole area"), separator `›`, each item `title` = definition level column name; last item `aria-current`. |
| Share | `node.count / root.count`, same `formatPercent` as the pie legend. |
| Loop safety | Inspector never dispatches `typology:buildingselect`; it calls `digiTypologyMap.selectBuilding/clearBuilding` directly. |
| Files | New `wwwroot/js/typology-inspector.js` (`const digiTypologyInspector = (function(){'use strict';…})()`, classic script like panel/map); markup in `View.cshtml`; CSS appended to the `typology-*` block; no new NuGet/CDN; `AreaView` query contract untouched. |

## Files

```
DiGi.GIS.WebAPI.UI/DiGi.GIS.WebAPI.UI/
├── Constants/BuildingData.cs                    # + DatabaseIdName = "Database Id"
├── ViewModels/TypologyBuildingViewModel.cs      # + long Id (ctor: reference, id, countyId, path; XML docs)
├── Create/TypologyBuildingsViewModel.cs         # + id_ByReference param; Flatten becomes a local function (no private static helper)
├── Controllers/TypologyController.cs            # SolveBuildingsAsync: project database_id, collect id_ByReference, pass to factory
├── Views/Typology/View.cshtml                   # right aside: 3 cards; load typology-inspector.js before typology-view.js
├── wwwroot/js/typology-inspector.js             # NEW: digiTypologyInspector
├── wwwroot/js/typology-view.js                  # digiTypologyLayout module; bootstrap calls inspector.render(viewModel, definition)
└── wwwroot/css/gis-theme.css                    # + "right panel (#26)" rules before the 1100px media query (~L2514)
DiGi.Test/DiGi.GIS.WebAPI.UI.xUnit/Facts/
├── Create_TypologyBuildingsViewModel.cs         # assert Id mapped / 0 when absent
└── TypologyBuildingsViewModel_Serialization.cs  # populate Id in the round-trip instance
DiGi.GIS.WebAPI.UI/issue-26-implementation-plan.md   # this plan
```

## Design

### 1. Server — `Id` on the building DTO (C#)

- `Constants/BuildingData.cs`: `public const string DatabaseIdName = "Database Id";` with XML doc ("the `Building2DReference.Id` the 2D details and 3D viewer routes address").
- `ViewModels/TypologyBuildingViewModel.cs`: ctor `(string reference, long id, int countyId, List<int> path)`; `public long Id { get; set; }` documented "0 when the column was not projected or the row carried none"; class `<para>` notes `Id` serves the links, not the centroid join.
- `Create/TypologyBuildingsViewModel.cs`: signature `TypologyBuildingsViewModel(this VisualTypology? visualTypology, Dictionary<string, int>? countyId_ByReference = null, Dictionary<string, long>? id_ByReference = null)`; convert `private static Flatten` into a recursive **local function** closing over `buildings`, both maps (removes a pre-existing "no private static helper in Create" violation, `Coding - General.md` §2); leaf branch adds `long id = id_ByReference is not null && id_ByReference.TryGetValue(reference, out long databaseId) ? databaseId : 0;`. Body otherwise unchanged.
- `Controllers/TypologyController.cs` `SolveBuildingsAsync` (L259-442): after the `"reference"` catalog lookup (L340-344) add the same for slug `"database_id"` → `columnUniqueIds`; declare `Dictionary<string, long> id_ByReference = [];`; in the part loop read `int index_Id = partTable.GetColumnIndex(Constants.BuildingData.DatabaseIdName);` and, per row with a string reference, file `long`/`int` cell values (`Create.Table` converts cells to the declared type, `long` expected; `int` defensive); call `visualTypology.TypologyBuildingsViewModel(countyId_ByReference, id_ByReference)`. Update the "chain columns + reference" comment. Missing catalog column → nothing projected, `Id` stays 0.
- Tests: `Create_TypologyBuildingsViewModel.cs` adds `id_ByReference` (`A1`→9157, `B1`→42) and asserts `A1.Id == 9157`, `A2.Id == 0`, `B1.Id == 42`; `TypologyBuildingsViewModel_Serialization.cs` constructs `new("A1", 9157, 101, [0])` and asserts `Id` after round trip (`Coding - Automatic Tests.md` §4: populate the new member in the existing fact).

### 2. Layout API — `wwwroot/js/typology-view.js` L124-140

Replace `initRightPanelToggle` with:

```js
const digiTypologyLayout = (function () {
    'use strict';
    const layout = document.querySelector('.typology-layout');
    const toggle = document.getElementById('typology-panel-toggle');
    function setRightPanelCollapsed(collapsed) { /* classList.toggle(cls, collapsed); aria-expanded; aria-label/title Show/Hide panel; window resize event */ }
    function isRightPanelCollapsed() { return layout !== null && layout.classList.contains('typology-panel-collapsed'); }
    if (toggle) { toggle.addEventListener('click', function () { setRightPanelCollapsed(!isRightPanelCollapsed()); }); }
    return { setRightPanelCollapsed, isRightPanelCollapsed };
})();
```

Bootstrap `initSolve`: after `digiTypologyPanel.render(viewModel)` add `if (typeof digiTypologyInspector !== 'undefined') { digiTypologyInspector.render(viewModel, definition); }`. Update the header comments (mention #26). Left toggle untouched.

### 3. Markup — `Views/Typology/View.cshtml` (replace aside body L93-99)

Three `gis-card gltf-card` cards inside `#typology-side-panel`, reusing `.typology-card-title`, `.typology-muted`, `.typology-area-list/-item/-label/-value` (from the #24/`f7d8684` Administrative Area card), `.typology-tree-swatch`, `.gis-column-filter`, `.gis-button gis-button-secondary`:

- **Typology** (`.typology-inspector-card`): `#typology-inspector-empty` line; `#typology-inspector` (hidden) with `#typology-inspector-swatch` + `#typology-inspector-name`, `<nav id="typology-inspector-breadcrumb" class="gis-path-breadcrumb typology-inspector-breadcrumb" aria-label="Typology path">`, `<dl>` rows `#typology-inspector-count` ("Buildings") and `#typology-inspector-share` ("Share"), `#typology-inspector-description` (hidden when empty).
- **Buildings** (`.typology-grid-card`): title with `#typology-grid-count`; `<input type="search" id="typology-grid-filter" class="gis-column-filter" placeholder="Filter by reference" aria-label=… autocomplete="off" disabled>`; `#typology-grid-empty` line; `<div id="typology-grid" class="typology-grid" role="listbox" aria-label="Buildings of the selected typology" tabindex="0" hidden>` → `#typology-grid-spacer` → `#typology-grid-slice`.
- **Building** (`.typology-building-card`): `#typology-building-empty`; `#typology-building` (hidden) `<dl>` rows Reference (`.typology-mono`), Id, County id, X, Y, Typology; `.typology-building-links` with `<a id="typology-building-details" target="_blank" rel="noopener">Details</a>` and `<a id="typology-building-model" … hidden>3D Viewer</a>`.
- Scripts: `<script src="~/js/typology-inspector.js" asp-append-version="true">` after `typology-map.js`, before `typology-view.js`.

### 4. `wwwroot/js/typology-inspector.js` (new)

Header doc comment in the `typology-map.js` style (purpose, event contract, classic script). Helpers mirror the siblings: `element`, `escapeHtml`, `pathKey`, `baseUrl`, `isUnder` (copy of map's), `formatCount`/`formatPercent` (as panel), `formatMetre` (`toFixed(2)`).

**State:** `viewModel, definition, root, nodesByKey (Map pathKey→node), buildings, pathKeys[], referencesLower[], indexByKey (Map "ref|county"→index), scoped[], filtered[], selectedKey, selectedBuildingIndex, focusIndex, filterTerm, filterTimer, rafHandle`. Constants `rowHeight = 26` (must equal the CSS row height — comment both sides), `overscan = 10`, `filterDebounceMs = 150`, `rootName = 'Whole area'`.

**`render(model, definitionModel)`**: index nodes, precompute `pathKeys`/`referencesLower`/`indexByKey`, reset to empty state.

**`fillInspector(node)`**: name (root → `rootName`, swatch hidden), swatch colour `digiTypologyPanel.colorOf(node)`, breadcrumb from `nodesByKey.get(pathKey(path.slice(0, d)))` for `d = 0..path.length` (item `title` = `definition.levels[d-1].name` when present; last item `aria-current="page"`), count, share `formatPercent(node.count, root.count)`, description shown only when non-empty.

**Grid:** `scopeGrid(key)` → `scoped` = indices with `isUnder(pathKeys[i], key)` (empty when key null) → `applyFilter()` → `filtered`, count label `"n of m"`, empty-line text (no selection / no match / no leaf rows), spacer height `filtered.length * rowHeight`, `scrollTop = 0`, `renderSlice()`. `renderSlice()` computes `first = max(0, floor(scrollTop/rowHeight) − overscan)`, `visible = ceil(clientHeight/rowHeight) + 2·overscan`, builds the row HTML string (`<div class="typology-grid-row" role="option" id="typology-grid-row-{i}" data-index="{buildingIndex}" aria-selected>` with `.typology-grid-reference` mono + `.typology-grid-county`), sets `slice.innerHTML` and `transform: translateY(first*rowHeight px)`, `aria-activedescendant` for the focus row. `scroll` → rAF-throttled `renderSlice`; `window resize` → `renderSlice`. Filter `input` → debounce → `applyFilter`. Click delegation on the slice → `selectBuildingByIndex`. Keydown on `#typology-grid`: ArrowUp/Down/Home/End move `focusIndex` + `ensureVisible`, Enter/Space select, Escape `clearBuilding` (`preventDefault` on handled keys).

**`fillBuilding(index)`**: reference; id (`> 0` else "–"); county id (`> 0` else "–"); x/y from `digiTypologyMap.pointOf` (guard `typeof`); typology = `nodesByKey.get(pathKeys[index]).name` or "Not classified". Links: Details `base + '/building2D/detailsbyreference?reference=' + encodeURIComponent(ref)` + `&countyid=` when > 0 else `&x=&y=` when a centroid exists; 3D `base + '/buildingmodel/buildingmodelbyid?id=' + id [+ '&countyid=']`, `hidden` unless `id > 0`.

**Wiring:**
- `selectBuildingByIndex(index)`: set state, `focusIndex = filtered.indexOf(index)`, `renderSlice`, `fillBuilding`, `digiTypologyMap.selectBuilding(reference, countyId)`.
- `clearBuilding()`: hide card, `digiTypologyMap.clearBuilding()`, `renderSlice`.
- `typology:selectionchange` null → `selectedKey = null`, hide inspector, `scopeGrid(null)`, filter cleared + disabled, `clearBuilding()`, `digiTypologyLayout.setRightPanelCollapsed(true)`. Non-null → `fillInspector`, enable filter, `scopeGrid`, drop the selected building if it left the scope, open the panel if collapsed.
- `typology:buildingselect` → `index = indexByKey.get(ref|county)`; not found (unclassified) → info card only + open panel; found and outside `selectedKey` → `digiTypologyPanel.select(buildings[index].path)` (synchronous; re-scopes via the event) then `selectBuildingByIndex(index)` + `ensureVisible`.
- Export `{ render, selectBuilding(reference, countyId), clearBuilding }`.

### 5. CSS — `wwwroot/css/gis-theme.css`, block "Typology area view right panel (#26)" before the `@media (max-width: 1100px)` (~L2514)

Cards: inspector/building `flex: 0 0 auto`, 14 px side padding (the #24 convention); grid card `flex: 1 1 auto; min-height: 220px; display: flex; flex-direction: column`. `.typology-grid { flex: 1 1 auto; min-height: 0; overflow-y: auto; position: relative; border: 1px solid var(--border-color); border-radius: 4px }` + `:focus-visible` outline; `.typology-grid-spacer { position: relative }`; `.typology-grid-slice { position: absolute; top:0; left:0; right:0; will-change: transform }`; `.typology-grid-row { display:flex; justify-content:space-between; height: 26px; box-sizing: border-box; padding: 0 8px; cursor: pointer; user-select: none; white-space: nowrap }` with `:hover`, `.typology-grid-row-focus` (primary outline inset), `.typology-grid-row-selected` (`--primary-light`); `.typology-grid-reference` mono + ellipsis; `.typology-grid-county` muted tabular-nums; breadcrumb/name/description/link rules; `.typology-mono`. Inside the existing 1100 px media query add `.typology-grid { max-height: 320px }` so the stacked layout keeps a bounded list.

## Guideline compliance

| Concern | Guideline | Where |
|---|---|---|
| Explicit typing, `[]`/`new()`, block-scoped namespace, XML docs with `<param>` order = signature, ≤7 params one line, zero warnings | `Coding - General.md` §1, `XML Documentation - Create.md` | all C# edits |
| No private static helper in `Create` | `Coding - General.md` §2 | `Flatten` → local function |
| Column names in `Constants`, slugs beside the existing `"reference"` literal | `Coding - General.md` §2 | `Constants/BuildingData.cs`, controller |
| Populate new member in existing serialization fact; build lib before `dotnet test` | `Coding - Automatic Tests.md` §4 | two facts |
| Deployed host read-only, sequential; premise checks were GET + one 3-row paging read | `Coding - Deployed WebAPI.md` §3-4 | verification |
| Byte-level edits, CRLF preserved; `git diff --stat --ignore-cr-at-eol` before commit | `Coding - General.md` §1.14 | all files |
| Interactive DoD in a real browser (Playwright from scratchpad, host python, server killed in `finally`, rebuild before `.cshtml` re-check) | `Coding - Browser Testing.md` §2-6 | verification |
| Corrections as issue comments via `--body-file`, closing comment with SHA/branch/files/tests/live check + screenshot | `GitHub - Issues.md` §1-3 | close-out |
| Plan file uses relative paths | `README.md` portability rule | `issue-26-implementation-plan.md` |

## Verification

1. `dotnet build` (solution, `-m:1`) → 0 warnings; `dotnet test` `DiGi.Test/DiGi.GIS.WebAPI.UI.xUnit` → the two updated facts green.
2. Live run: `dotnet DiGi.GIS.WebAPI.UI.dll` (`ASPNETCORE_ENVIRONMENT=Development`, `ASPNETCORE_URLS=http://localhost:<port>`), Playwright (Python, Edge headless) script in the scratchpad against county 0201 (area id 5, 33 687 rows), seeding `sessionStorage['digiTypology.definition']` and visiting `/typology/view?id=5&code=0201&administrativearealtype=2`. Assert:
   - initially `.typology-layout.typology-panel-collapsed`, `#typology-panel-toggle[aria-expanded=false]`;
   - click a level-1 tree row → panel open (`aria-expanded=true`), `#typology-inspector-name` = row name, count = tree row count, breadcrumb has 2 items with the level column name in `title`, share = count/root;
   - `#typology-grid-count` = "n of n" with n computed from `window.digiTypologyView.viewModel.buildings` by path prefix; spacer height = n×26; `.typology-grid-row` count < 100;
   - type a 4-char prefix → count shrinks, every rendered row contains it; clear filter restores n;
   - `scrollTop = scrollHeight` → after rAF the last rendered row's `data-index` = last filtered index, DOM rows still < 100;
   - click a row → `#typology-map-marker circle` present, info card fields filled, `#typology-building-details` href `/building2D/detailsbyreference?reference=…&countyid=…` and `#typology-building-model` href `/buildingmodel/buildingmodelbyid?id=…&countyid=…` (visible, id > 0); `fetch` both → 200;
   - dispatch `typology:buildingselect` for a building of another bucket → tree selection moves, row highlighted, card filled;
   - `#typology-tree-clear` → panel collapsed, inspector/building cards show empty lines, `#typology-grid` hidden, marker layer empty;
   - `set_viewport_size` < 1100 px → no horizontal overflow; screenshot of the open panel for the closing comment.
3. `git diff --stat --ignore-cr-at-eol` shows no whole-file rewrites; commit `feat(typology): right panel - typology inspector, windowed building grid and building info (#26)` on `0.8.9`; post the two premise-correction comments and the resolution comment (SHA, files, tests, live check, screenshot); close #26 and tick it in #21.

## Out of scope / hand-offs

- Loading/error/empty-state orchestration and replacing `window.digiTypologyView` → #27.
- Map zoom/pan to the selected building (fit-to-outline only, per #21).
- Embedding the `_Building2DView` partial in the panel — links only.
