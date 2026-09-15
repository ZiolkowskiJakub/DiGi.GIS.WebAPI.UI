# Implementation Plan — Issue #27: Typology area view — end-to-end orchestration, loading/error/empty states and live verification

> Part of tracking issue [#21](https://github.com/ZiolkowskiJakub/DiGi.GIS.WebAPI.UI/issues/21). **Labels:** `type: feature`, `priority: medium`, `ai: heavy`. Depends on #22 (`7fcd75b`), #23 (`7070bef`), #24 (`12dfb91`), #25 (`0025578`), #26 (`e45c5d0`) — **all closed** → **unblocked**.
> **Repo:** `DiGi.GIS.WebAPI.UI`, branch `0.8.9`, working tree clean at planning time. Front-end-only change (JS + markup + CSS) — no C# edits, no new NuGet, no `DiGi.Test` facts (the feature is UI orchestration, verified per `Coding - Browser Testing.md`).

## Context

The `/typology/view` page already renders all three panels: the left tree + pie (#24), the centre SVG map with colour-coded dots + dimming (#25), and the right inspector + windowed grid + building card (#26). The `POST /typology/buildings` solve endpoint (#22) answers the full DTO (`{root, buildings}`) with the agreed error contract (400 with a message list, 404 no buildings, 413 ceiling, 503 upstream).

What is **not** there is the page-level orchestration that #21's architecture intends ("the panels are pure renderers of the one solve payload"). Today `typology-view.js` is the "deliberately minimal" bootstrap the #24 commit left behind — its own header says *"the loading/error/empty-state UI and the shared page state belong to the orchestration sub-issue (#27)"*. Concretely:

- The two map fetches (outline + centroids) are owned by `digiTypologyMap.load()`; the solve fetch is owned by the view. They are not coordinated as one unit, and there is no single place that knows when the page is "done".
- Every non-200 and the no-definition case collapse to a one-line status in the left panel (`digiTypologyPanel.showStatus`). There is **no** `gis-loader` during the solve, **no** `gis-modal` error dialog, and the three empty states (no stored definition, area with no buildings, null solve) are not distinct, actionable surfaces.
- The selection's cross-cutting controls (the `#typology-tree-clear` button, the Escape clear) are wired inside `typology-panel.js`, so the view does not own the single selection state.
- None of the interactive DoD has been exercised end-to-end against the live API, and the umbrella #21 is still open.

This issue makes `typology-view.js` the single orchestrator and owner of the page state, gives the page proper loading/error/empty surfaces, verifies the full flow in a real browser against the live API, and closes the umbrella.

## Issue validity — premises re-verified (2026-09-15)

| Issue claim | Verified | Note |
|---|---|---|
| Depends on #22–#26 | Correct | All five closed (`7fcd75b`, `7070bef`, `12dfb91`, `0025578`, `e45c5d0`). |
| `typology-view.js` owns the three parallel fetches + the join + routing + selection | **Not yet** | `digiTypologyMap.load()` owns the outline + centroids fetches (`typology-map.js:366`); the view owns only the solve. The join is in the map's `renderPoints` (`typology-map.js:175`). This issue moves the fetches + orchestration into the view. |
| `gis-loader` during solve | **Missing** | No loader in `View.cshtml`/`typology-view.js`; only the status line "Solving the typology for the area…". `.gis-loader` + `.gis-loader-text` already exist in `gis-theme.css:584,1291` and are used by `typology.js` and `GLTFSceneView.cshtml` — reuse, do not reinvent. |
| Error modal (`gis-modal` + message list) for 400/413/503 | **Missing** | Errors render to `showStatus`. `.gis-modal-overlay` / `.gis-card.gis-modal` are an established pattern (`typology.js:1578-1590`, `Views/Typology/Start.cshtml:53-61`). The solve answers 400/413 with a JSON message list and 503 with no body (`TypologyController.SolveBuildingsAsync`). |
| Empty states: no stored definition / no buildings / null solve | **Partial** | No-definition already degrades to the area context (DoD met) but only via a status line; 404 → "The area has no buildings."; null solve → the panel's "The solve answered no typology." None are distinct actionable surfaces. |
| Own the single selection state (active path, clear, Escape) | **Partial** | `#typology-tree-clear` is wired in `typology-panel.js:335`; Escape in `typology-panel.js:308-315` (tree) and `typology-inspector.js:510-513` (grid — `preventDefault` without `stopPropagation`, so a document-level Escape handler would double-fire). The view only mirrors the selection via `setSelectionCallback`. |
| Live verification on `gis.digiproject.uk` (read-only) | **Not done** | Both hosts confirmed live: `gis.digiproject.uk/typology` → 200; `api.digiproject.uk/information/health` → Healthy, `ServiceVersion` 0.8.8. |
| Close the umbrella with a structured resolution comment | **Open** | #21 open; its roll-up criteria are all still unchecked. |

**Premise to correct (post as a comment, never a body edit — `GitHub - Issues.md` §2):** the issue lists "null solve" as an *empty state*. On the current wire, a solve that produces no typology is a **400** with the message *"The definition could not be solved: a column named by the chain is absent from the table, or a level carries no rule."* (`TypologyController.cs` `SolveBuildingsAsync`) — i.e. an *error*, not a 200-with-null-root. A 200 always carries a non-null `root` (`Create/TypologyBuildingsViewModel.cs` returns a node whenever the input is non-null). So the client must treat "null solve" two ways: the 400 → **error modal** (with the server's message), and the defensive 200-with-`root === null` → **empty state** ("The solve answered no typology.", which the panel already renders). This is a client-handling clarification, not a backend change.

**Verdict: still valid and unblocked.** One premise correction to post (the null-solve wire mapping). No backend change is required.

## Decisions

| Decision | Choice |
|---|---|
| Orchestration owner | `typology-view.js` becomes the single owner of the page state: it fires the three fetches (outline, centroids, solve) in parallel, awaits them, routes the result to the three panels, and owns the selection's cross-cutting controls (clear button, Escape). The map/panel/inspector stay pure renderers of the data the view hands them (the #21 "panels are pure renderers" principle). |
| Where the join lives | The join (centroid ↔ building by `(reference, countyId)`) stays in the map's renderer as a draw detail (`typology-map.js` `renderPoints` already does it correctly and is shipped/verified). The view guarantees the join's two inputs (centroids + solve DTO) are both present before the map draws, and is the single place that knows the page state. This keeps the refactor surgical — the map's fit/project/draw/dim/mark logic is untouched. *(If the owner wants the join literally in the view's code, that is a follow-up extract, not part of this issue.)* |
| Map fetches | `digiTypologyMap.load(areaId)` (the two fetches) is split into `setOutline(outlines)` + `setCentroids(centroids)` (data-in, no network); the fetches move to the view. `render(viewModel)` (join + draw) is unchanged. |
| Selection ownership | The panel remains the selection UI (tree rows; `select`/`clear`/`setSelectionCallback` are its public API, `clear()` is idempotent). The view owns the **clear button** and the **Escape clear of the selection** (both call one `clearSelection()` → `digiTypologyPanel.clear()`, which dispatches `typology:selectionchange` with `path: null`). Escape becomes one chain owned by the view (bubble-phase `document` handler): (1) error modal open → close it; (2) `event.defaultPrevented` → an inner control already handled it (the grid cleared its building); (3) selection active → `clearSelection()`. The panel's own Escape branch is removed (single owner), and the grid's Escape case is guarded so it claims Escape only when a building is actually selected — otherwise it bubbles to the view. Map + inspector keep reacting to the one `typology:selectionchange` event — one source of truth, no duplicate state, no double-firing. |
| Loading state | `gis-loader` + `gis-loader-text` in the tree card, shown while the solve is in flight, hidden on settle (success or failure). The map's outline/centroid arrival keeps its own status line (it renders as it arrives). |
| Error modal | One `gis-modal-overlay` + `gis-card gis-modal` dialog, created on demand (the `typology.js` pattern), listing the server's message array for 400/413 and a single actionable line for 503. One Close button. Focus moves into the dialog, Escape closes it (while it is open) and does not also clear the selection. |
| Empty states | Three distinct surfaces in the left panel: **no stored definition** (area context already in the Administrative Area card + its existing "Back to the definition page" link, plus an explicit tree-card line), **no buildings** (404), **null solve** (200-with-null-root / the panel's "answered no typology"). Each is actionable copy, not a status flash. |
| Direct-URL degradation | A visit with no carried definition still draws the outline + neutral dots (the area context) and shows the no-definition empty state — the DoD "degrades to the area context". No solve is fired. |
| No console errors | Interpret as "no *new* console errors from this feature". The pre-existing `favicon.ico` 404 (noted in the #26 plan) is out of scope; do not add a favicon here. |
| Live verification | Run the built app locally (`dotnet bin/DiGi.GIS.WebAPI.UI.dll`, `ASPNETCORE_ENVIRONMENT=Development`, `ASPNETCORE_URLS=http://localhost:<port>`) — it proxies to the live `api.digiproject.uk` (read-only) — and drive it with Playwright (Edge headless, host python, script + screenshots in the scratchpad), per `Coding - Browser Testing.md`. This is the same code + same live API as the deployed `gis.digiproject.uk` host. |
| Close-out | Comment the premise correction + a resolution comment on #27 and close it **first** (#21's "all sub-issues closed" criterion needs it); then verify #21's roll-up criteria, check them off (Python script, `GitHub - Issues.md` §1 integrity assertions), post a structured resolution comment on #21 (§3) and close the umbrella. |
| Files | `wwwroot/js/typology-view.js` (main), `wwwroot/js/typology-map.js` (split `load`), `wwwroot/js/typology-panel.js` (drop Escape branch + clear-button wiring), `wwwroot/js/typology-inspector.js` (guard the Escape case), `Views/Typology/View.cshtml` (loader markup), `wwwroot/css/gis-theme.css` (state styles). No C#, no new packages. |

## Files

```
DiGi.GIS.WebAPI.UI/DiGi.GIS.WebAPI.UI/
├── wwwroot/js/typology-view.js          # main: orchestration (3 parallel fetches, routing), selection ownership (clear + Escape chain), states (loading / error modal / empty); window.digiTypologyView retired
├── wwwroot/js/typology-map.js           # load(areaId) -> setOutline(outlines) + setCentroids(centroids); render/join/draw/dim/mark unchanged
├── wwwroot/js/typology-panel.js         # drop the tree Escape branch (308-315) + the clear-button wiring (335) - the view owns both
├── wwwroot/js/typology-inspector.js     # Escape case (510-513) claims the key only when a building is selected; otherwise bubbles to the view
├── Views/Typology/View.cshtml           # tree-card gis-loader (empty-state copy reuses #typology-tree-status; area card + back link already present)
└── wwwroot/css/gis-theme.css            # state placement (loader in tree card, error-modal reuse of .gis-modal + .typology-error-list)
DiGi.GIS.WebAPI.UI/issue-27-implementation-plan.md   # this plan
```

The panel and inspector remain renderers — the only edits to them are the two Escape/clear ownership moves above (no behaviour added). No C# changes. No `DiGi.Test` facts.

## Design

### 1. Orchestration — `wwwroot/js/typology-view.js`

Replace the `initSolve` IIFE with a named `digiTypologyView` module (same classic-script, no-imports shape as the siblings) that is the single owner of the page state.

**State:** `area` (`{id, code, type}` read once from the `.typology-shell` `data-*` attributes), `definition` (from `sessionStorage['digiTypology.definition']`, the same key `typology.js:confirmLoadSelection` writes; the bootstrap's existing shape check — an object with a non-empty `levels[]` — is kept), `activePath` (null until a selection), `modalOpen`, `settled` (bool), and the three in-flight results. The `window.digiTypologyView` global is **retired** (verified: no reader outside `typology-view.js`); the state lives in the module's closure. The rest of the file — the two resizer IIFEs, the `digiTypologyLayout` module (#26), the left-panel toggle IIFE — is **untouched**.

**Flow:**

1. Read `area` + `definition`. Build `base = (window.AppBaseUrl || '/').replace(/\/$/, '')`.
2. **No definition** → do not fire the solve. Fire the two context fetches in parallel (outline, centroids); on arrival call `digiTypologyMap.setOutline(outlines)` / `setCentroids(centroids)` so the outline + neutral dots render (the area context). Render the **no-definition empty state**: `digiTypologyPanel.showStatus('No definition was carried to this page. Define one on the typology page and press Load.')` (the panel shows the line and hides the tree — the right surface); the actionable "Back to the definition page" link is already in the Administrative Area card above it. Settle. (This is the direct-URL degradation the DoD requires.)
3. **Definition present** → fire **three** fetches in parallel:
   - `GET base + '/administrativeareal2D/svg/polygonsbyid?id=' + area.id`
   - `GET base + '/building2D/point2dsbyadministrativeareal2Did?administrativeareal2Did=' + area.id`
   - `POST base + '/typology/buildings'` with `{ definition, id: area.id, code: area.code, administrativeArealType: area.type }` (enums as integers — already the case).
   Show the `gis-loader` in the tree card while the solve is in flight.
4. **Route on settle** (all three resolved, or the solve resolved — the outline/centroids render as they arrive, independently):
   - Outline → `digiTypologyMap.setOutline(outlines)` (fit + draw; unchanged map logic).
   - Centroids → `digiTypologyMap.setCentroids(centroids)` (store; the map draws them once the fit is in).
   - Solve 200 `{root, buildings}` → `digiTypologyMap.render(viewModel)` (join + colour the dots, unchanged), `digiTypologyPanel.render(viewModel)` (tree + pie), `digiTypologyInspector.render(viewModel, definition)` (inspector + grid + building card).
   - Solve **404** → **no-buildings empty state**: `digiTypologyPanel.showStatus('The area has no buildings to solve for.')` (the outline + context still show).
   - Solve **400 / 413** → **error modal** listing the server's message array (each line rendered via `textContent` / `escapeHtml`, never raw `innerHTML`).
   - Solve **503** (no body) → **error modal** with the single line "The building data service is unavailable. The area outline is shown; the typology could not be solved."
   - Solve 200 with `root === null` (defensive) → still call `digiTypologyPanel.render(viewModel)`: the panel itself shows the **null-solve empty state** ("The solve answered no typology.") and an empty pie; the map draws neutral dots; the inspector shows its empty line. No special view branch needed.
5. Hide the `gis-loader`; set `settled`.

**Helpers:** `fetchJson(url, options)` (wraps `fetch`, resolves to `{ok, status, json}` and never rejects on network failure — a failed context fetch degrades to that surface's "unavailable" line, not a console error); `messagesFromBody(body, fallback)` (400/413 answer a string array; 503 answers nothing → `[fallback]`); `showLoader()` / `hideLoader()` (toggle `#typology-tree-loader`, hiding `#typology-tree-status` while the loader is up and vice versa); `showErrorModal(lines)` / `closeErrorModal()` (create/close the on-demand dialog, move focus to the Close button, track `modalOpen`). The two plain empty-state lines go through the existing `digiTypologyPanel.showStatus`.

**Selection ownership (the view's single `clearSelection()`):**
- `clearSelection()` = `digiTypologyPanel.clear()` (idempotent — it early-returns when nothing is selected; dispatches `typology:selectionchange` with `{path: null, node: null}`, which re-scopes the map dimming + inspector and closes the right panel — the existing #26 behaviour).
- Wire `#typology-tree-clear` → `clearSelection()`; remove the panel's own wiring (`typology-panel.js:335`) so the button has exactly one owner. The panel's disabled-state management of the button stays (it knows `selectedKey`).
- **Escape — one chain, owned by the view** (bubble-phase `document` `keydown` listener, checked in order): (1) `modalOpen` → `closeErrorModal()` + `preventDefault`; (2) `event.defaultPrevented` → an inner control already claimed the key (the grid cleared its building) → return; (3) `activePath !== null` → `clearSelection()` + `preventDefault`.
- To make that chain hold, remove the panel's tree Escape branch (`typology-panel.js:308-315`) and guard the grid's Escape case (`typology-inspector.js:510-513`) so it claims the key (with `preventDefault`) **only when a building is actually selected** — otherwise it bubbles to the view. Net UX: Escape clears the innermost active thing first (modal → building → typology selection), and nothing else.
- Keep `digiTypologyPanel.setSelectionCallback` mirroring `activePath` (the inspector's dot-click re-scope path still goes through `digiTypologyPanel.select`).

This leaves the panel as the selection UI, the map + inspector reacting to the one `typology:selectionchange` event, and the view owning the clear button + the Escape chain — one source of truth, no duplicate selection state, no double-firing.

### 2. Map — `wwwroot/js/typology-map.js`

Split `load(areaId)` (currently: two `fetch`es + the arrival logic) into two data-in methods, and move the network calls to the view:

- `setOutline(outlines)` → the exact arrival block `load` ran on outline arrival: `renderOutline(outlines)` (fit + draw), and on success `hideStatus()` / `statusAfterCentroids()` / `renderPoints()`; on failure (null/invalid, `renderOutline` returns false) the viewport's `showStatus('The area outline is unavailable.')`. A failed outline fetch from the view arrives as `setOutline(null)`.
- `setCentroids(centroids)` → the existing centroid-arrival block (build `centroidsByKey`, then `statusAfterCentroids()` + `renderPoints()` when the fit is in).
- `render(viewModel)` (index nodes + buildings, join + draw) is **unchanged**. `setSelection` / `applyDimming` / `renderMarker` / `pointOf` / `selectBuilding` / `clearBuilding` / the hover label are **unchanged**.
- Remove `load` and the two `fetch` calls; keep the `centroidsFailed` semantics (a failed centroid fetch now arrives as `setCentroids(null)` → the "Building positions are unavailable." line).

Net: the map is a pure renderer (data in, pixels out); the view is the only place that issues the area view's requests.

### 3. Markup — `Views/Typology/View.cshtml`

Inside the left panel's tree card (`#typology-tree-card`), alongside the existing `#typology-tree-status`:

- A `gis-loader` surface: `<div id="typology-tree-loader" class="typology-tree-loader" hidden><div class="gis-loader"></div><p class="gis-loader-text">Solving the typology for the area&hellip;</p></div>` — shown while the solve is in flight, hidden on settle. (Reuses the existing `.gis-loader` / `.gis-loader-text`.)
- Keep `#typology-tree-status` as the single line for the three empty states (no definition / no buildings / null solve) — the copy is set by the view per state. The Administrative Area card (with its existing "Back to the definition page" link) is the no-definition "area context" the issue requires; no new card.
- The error modal is created on demand by JS (the `typology.js` pattern) — not static markup — so it is absent from the DOM until a 400/413/503 happens.
- No change to the `data-area-*` attributes, the `ViewData` (Title/Description/`noindex, follow`), or the script order (`typology-panel.js`, `typology-map.js`, `typology-inspector.js`, then `typology-view.js`).

### 4. CSS — `wwwroot/css/gis-theme.css`

In the existing `typology-*` block (before the 1100 px media query, the #24/#26 convention):

- `.typology-tree-loader` placement (centered in the tree card, same box as the status line) so the loader and the status line never show at once.
- The empty-state lines reuse `#typology-tree-status` (`.typology-muted`) as-is — plain copy, no inline links (the actionable "Back to the definition page" link is the one in the Administrative Area card).
- The error modal reuses the existing `.gis-modal-overlay` / `.gis-card.gis-modal` / `.gis-modal-buttons` rules (no new modal CSS); add only a `.typology-error-list` for the message list (`ul` reset, one line per server message).
- Inside the 1100 px media query, confirm the loader + empty states stack with the rest of the left panel (no new overflow).

### 5. Guideline compliance (this issue)

| Concern | Guideline | Where |
|---|---|---|
| Reuse existing `gis-loader` / `gis-modal` / `typology-*` surfaces; no new libraries, no new NuGet | #21 architecture, `Coding - General.md` | all files |
| Enums as integers on the wire; the solve body unchanged | `Coding - WebAPI Contracts.md` | `typology-view.js` |
| Treat `api.digiproject.uk` as read-only; the page's parallel fan-out is at most **3 concurrent upstream reads** at t=0 (outline, centroids, the solve's catalog read) — within the "handful of workers" guardrail; a 500 on a cold partition is retried in isolation before believing it | `Coding - Deployed WebAPI.md` §3-4 | orchestration + verification |
| Interactive DoD in a real browser (Playwright from the scratchpad, host python, server killed in `finally`, rebuild before any `.cshtml` re-check) | `Coding - Browser Testing.md` §2-6 | verification |
| Stored blobs are **LF** (verified via `git cat-file -p` on all five target files) while the working tree is CRLF (`core.autocrlf=true`, no `.gitattributes`): edit with the workspace editor tools (which normalise) or byte-level scripts that assert match counts; commit with the **default** `autocrlf` so LF blobs stay LF; `git diff --stat --ignore-cr-at-eol` before commit | `Coding - General.md` §1.14 | all files |
| Premise correction + close-out as comments via `--body-file` (never body edits), integrity assertions, structured resolution comment | `GitHub - Issues.md` §1-3 | close-out |
| Plan file uses relative paths | repo `README.md` portability rule | this plan |

## Verification

1. `dotnet build` (solution, `-m:1`) → 0 warnings (no C# change, but confirm the tree still compiles); no `DiGi.Test` run needed (no C# change).
2. Live run: `dotnet bin/DiGi.GIS.WebAPI.UI.dll` (`ASPNETCORE_ENVIRONMENT=Development`, `ASPNETCORE_URLS=http://localhost:<port>`). Playwright (Python, Edge headless) script in the scratchpad, against **county 0201 (area id 5, 33 687 rows)** and a **municipality** of the same county, seeding `sessionStorage['digiTypology.definition']` and visiting `/typology/view?id=5&code=0201&administrativearealtype=2`. Kill the server in `finally`. Assert (the #26 assertion style):
   - **County happy path:** loader visible during the solve, then hidden; outline drawn; dots coloured (dot-bucket counts match the tree counts); no console errors other than the pre-existing `favicon.ico` 404.
   - **Municipality clipping:** a municipality solve returns fewer buildings than the county (server-side clip); the tree counts reflect the clipped area, not the whole county.
   - **Dim / restore:** select a level-1 node → non-member dots dimmed, member dots at full opacity; clear (button and Escape) → all dots restore.
   - **Grid → map highlight:** open the right panel, click a grid row → the map marker ring appears on that dot; the building card fills.
   - **Panel persistence + defaults:** fresh context → left open / right closed (class + `aria-expanded`); drag both resizers, reload → widths restored from `localStorage`.
   - **Header/footer collapse (roll-up):** the shared layout toggle flips the `body` class and the viewport height, exactly as in the 3D viewer.
   - **States — the real ones:** (a) no definition (fresh context, no seed) → no `POST /typology/buildings` fired, outline + neutral dots, no-definition line, back link in the area card; (b) a definition the catalog rejects (bad column name) → **error modal listing the server's message array**; Escape closes the modal without clearing an existing selection.
   - **States — forced via Playwright route interception** (deterministic, no data hunting): `POST /typology/buildings` → 404 → no-buildings line over the outline; 413 with the ceiling message array → modal with the cap copy; 503 with no body → modal with the "service unavailable" line; 200 `{root: null, buildings: []}` → "The solve answered no typology." line.
   - **Escape chain:** modal open → Escape closes the modal; grid focused with a building selected → Escape clears the building only; grid focused with no building → Escape clears the typology selection; body focused with a selection → Escape clears the selection; nothing active → Escape is a no-op.
   - **No new console errors** in any of the above (the pre-existing `favicon.ico` 404 is expected and ignored).
   - Screenshots of the county happy path, the 413 modal, and the no-definition state for the closing comments.
3. `git diff --stat --ignore-cr-at-eol` shows no whole-file rewrites; commit `feat(typology): end-to-end orchestration, loading/error/empty states and live verification (#27)` on `0.8.9`.

## Close-out (after the code lands and is verified)

1. Post the **premise-correction comment** on #27 (the null-solve wire mapping, with the `TypologyController.cs` / `Create/TypologyBuildingsViewModel.cs` evidence) — `GitHub - Issues.md` §2, via `--body-file`.
2. Post a resolution comment on #27 (SHA, files, verification, the premise correction) and `gh issue close 27` — **before** touching the umbrella, so #21's "all sub-issues closed" criterion is true when it is checked.
3. Verify each of #21's roll-up criteria against the verification above (all sub-issues closed; county happy path; tree select/dim/clear; grid → map highlight + links; panel resize/persist + defaults + header/footer collapse; county-scale solve + cap message; zero warnings / no new packages / `AreaView` contract unchanged).
4. Check off #21's roll-up checkboxes with a standalone Python script (`subprocess` + `encoding="utf-8"`, the §1 integrity assertions: no `\ufffd`, no `ÔÇ`, tables contiguous), then `gh issue edit 21 --body-file`.
5. Post a **structured resolution comment** on #21 (the umbrella) per `GitHub - Issues.md` §3 — Resolution & Commits (SHA, branch `0.8.9`, repo), Summary of Changes (files), Tests (Playwright assertion count + the live-endpoint reads), Live Deployed Verification (county 0201 + municipality figures, screenshots) — then `gh issue close 21`.

## Out of scope / hand-offs

- Any backend change to the solve contract (400/404/413/503) — #22 is closed and the contract is as designed; the null-solve clarification is client-handling only.
- Map zoom/pan and hover-based spatial selection (fit-to-outline only, per #21).
- A `favicon.ico` (the pre-existing 404 is out of scope for this issue).
- Moving the centroid↔typology join out of the map's renderer into the view's code (a possible follow-up extract; functionally satisfied by the view owning the orchestration).
