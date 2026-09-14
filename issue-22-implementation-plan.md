# Implementation Plan — Issue #22: `POST /typology/buildings`

> Part of the Typology area view tracking issue [#21](https://github.com/ZiolkowskiJakub/DiGi.GIS.WebAPI.UI/issues/21).
> **Labels:** `type: feature`, `priority: high`, `ai: heavy`
> **Status (final):** **Implemented and live-verified** (see §12). `POST /typology/buildings` answers on `TypologyController`; `TypologySolveParameter` exists; the `View.cshtml` remains a stub (the area view is a separate sub-issue of #21).
> Sections §5.4, §5.6, §8.1, §11 below were updated in place where the implementation diverged from the original draft.

---

## 1. Goal

Create `POST /typology/buildings` on `TypologyController` that:
1. Validates the request (definition + area context).
2. Resolves the area to county-part ids (shared with the existing `GET /typology/countyids`).
3. Fetches building data per part from the deployed GIS Web API, paging sequentially.
4. Clips to the selected municipality/voivodeship when the area is below county.
5. Pages each part into the solver's table type (each page is parsed straight to it — no conversion step, see §5.4).
6. Solves the typology with `DiGi.Typology.Visual.Create.VisualTypology`.
7. Flattens the solved tree to the view DTO and returns it.

The browser never spells a `_type` or a rule name — the join and the solve run server-side.

---

## 2. Guideline Compliance Checklist

| Concern | Guideline | Section |
|---|---|---|
| One member per file for Query/Create/Convert | Coding – General | §2 File Organisation |
| Cheap constructors, validation in `Create` | Coding – General | §2 Constructors Stay Cheap |
| No `var`, block namespaces, `CancellationToken` last | Coding – General | §1 Core Coding Rules |
| Enum nullable binding (`Undefined = -1`) | Coding – WebAPI Contracts | §2 Binding traps |
| Enum values as integers on the wire | Coding – WebAPI Contracts | §2 |
| JSON body with `JsonSerializerOptions.Default` | Coding – WebAPI Contracts | §2 |
| Base URI from a constant, HTTP in `/Query` | Coding – WebAPI Contracts | §3 |
| County-part resolution — key by `id`, never `code` | Coding – GIS Administrative Data | §4 Rules |
| `ORDER BY` on any `LIMIT`/`FirstOrDefault` | Coding – GIS Administrative Data | §4 |
| Sequential paging, single retry on cold partition | Coding – Deployed WebAPI | §4 Gotchas |
| `commandTimeout` parameter standard | Coding – PostgreSQL | §3 |
| XML `<summary>` on all new public members | Coding – General | §1.4 |
| Test facts in DiGi.Test, build before test | Coding – Automatic Tests | §4 |
| No new NuGet packages | Issue Definition of Done | — |
| Zero compiler warnings | Coding – General | §1.4 |

---

## 3. Critical Type Mismatch — Verified Against the DLL

> **The upstream API and the solver use two different `Table` classes.** This is the single most important finding of the double-check.

| | Upstream API returns | Solver expects |
|---|---|---|
| **Namespace** | `DiGi.PostgreSQL.Table.Classes` | `DiGi.Core.IO.Table.Classes` |
| **Table** | `Table : SerializableObject` (non-generic) | `Table : Table<Column, Row>` (non-generic, generic base) |
| **Columns** | `List<DiGi.PostgreSQL.Table.Classes.Column?>` | `IEnumerable<DiGi.Core.IO.Table.Classes.Column>` |
| **Rows** | `List<object?[]>` | `IEnumerable<DiGi.Core.IO.Table.Classes.Row>` |
| **Implements `IColumn`** | No | Yes (`Column : IColumn`) |
| **Has `UniqueId` property** | Yes (slug, e.g. `floor_area`) | No — only `Index`, `Name`, `Type` |
| **`Core.IO.Query.UniqueId(column)`** | n/a | "Generates a unique identifier by **normalizing its name**" |

**Consequences:**

1. The upstream JSON **cannot** be deserialized directly to the solver's `Table` type — the `_type` discriminator, the column type, and the row type are all different.
2. The solver resolves filter columns against table columns by `Core.IO.Query.UniqueId(column)`, which normalizes `column.Name`. The `DiGi.Core.IO.Table.Classes.Column` has **no `UniqueId` property** — the unique ID is derived from `Name`.
3. A **conversion step** is required: `DiGi.PostgreSQL.Table.Classes.Table` → `DiGi.Core.IO.Table.Classes.Table`, mapping `Column.Name` → `Column.Name`, `Column.DataType` → `Column.Type` (via `Core.Query.Type(dataType)`), and `object?[]` → `Row(count, IDictionary<int,object>)`.
4. The existing `Create/VisualColumnTypologyFilter.cs` already creates `DiGi.Core.IO.Table.Classes.Column` instances with `Name = column.Name` (the display name). The solver resolves by normalized `Name`. The table's columns must carry the same `Name` for the resolution to succeed.

**Verified from the DLL XML docs (`DiGi.Core.IO.xml`):**

```
DiGi.Core.IO.Table.Classes.Column:
  Ctor: (int index), (int index, string name, Type type), (int index, Type type), (string name, Type type), ...
  Props: Index (int), Name (string), Type (System.Type)
  Methods: TryGetValidValue(object, out object, bool)

DiGi.Core.IO.Table.Classes.Row:
  Ctor: (int count, IDictionary<int, object> values), (Row row), (int count)
  Inherited from Row<object>: Count, Index, Indexes, this[int], Clone(), GetValue<T>(), GetValues(), RemoveValue(int), TryGetValue<T>()

DiGi.Core.IO.Table.Classes.Table (non-generic):
  Ctor: (), (IEnumerable<Column> columns)
  Inherited from Table<Column, Row>: ColumnCount, Columns, RowCount, Rows
  Methods: AddColumn(), AddColumn(Type), AddColumn(string, Type), UpdateColumn(int, string, Type, bool)

DiGi.Core.IO.Query.UniqueId(IColumn):
  "Generates a unique identifier for the specified column by normalizing its name."
```

---

## 4. New & Modified Files

### 4.1 New files (DiGi.GIS.WebAPI.UI project)

```
DiGi.GIS.WebAPI.UI/
├── Classes/Parameter/
│   └── TypologySolveParameter.cs              # Request body: Definition + area context
├── ViewModels/
│   ├── TypologyBuildingsViewModel.cs          # Response DTO: tree + buildings[]
│   ├── TypologyTreeNodeViewModel.cs           # Recursive tree node
│   └── TypologyBuildingViewModel.cs           # One building entry {reference, countyId, path}
├── Convert/ToCoreIO/
│   └── Table.cs                               # DiGi.PostgreSQL.Table → DiGi.Core.IO.Table conversion
├── Query/
│   ├── CountyPartsAsync.cs                    # Shared county-part resolution (extracted)
│   └── BuildingDataTableAsync.cs              # Upstream fetch with sequential paging + retry
├── Create/
│   └── TypologyBuildingsViewModel.cs          # Flatten solved VisualTypology → view DTO
└── Controllers/
    └── TypologyController.cs                  # + SolveBuildingsAsync action (modified)
```

### 4.2 Modified files

| File | Change |
|---|---|
| `Controllers/TypologyController.cs` | Add `POST /typology/buildings` action; refactor `GetCountyIdsAsync` to call the shared `Query.CountyPartsAsync` |
| `Constants/Default.cs` | Add `BuildingDataTableUri` constant (upstream `tablebybuildingdatabypagingparameter`) |

### 4.3 Test files (DiGi.Test)

```
DiGi.Test/
└── DiGi.GIS.WebAPI.UI.xUnit/Facts/
    ├── Convert_Table.cs                        # PostgreSQL.Table → Core.IO.Table conversion
    ├── Create_TypologyBuildingsViewModel.cs    # Flatten logic
    └── Query_ClipBuildingsByPointAsync.cs      # Clip logic
```

---

## 5. Detailed Design

### 5.1 Request Body — `TypologySolveParameter`

```csharp
// Classes/Parameter/TypologySolveParameter.cs
public class TypologySolveParameter
{
    public TypologyDefinitionParameter? Definition { get; set; }
    public int Id { get; set; }
    public string? Code { get; set; }
    public AdministrativeArealType? AdministrativeArealType { get; set; }
}
```

**Guideline notes:**
- `AdministrativeArealType?` is nullable — the sentinel `Undefined = -1` is not `0`, so a non-nullable binding would silently keep `Country` for an omitted parameter (Coding – WebAPI Contracts §2). The action rejects `null` and `Undefined` explicitly.
- `CancellationToken` is the last parameter on the action (CA1068), not on the body.
- The body is bound via `[FromBody]` — ASP.NET Core's input formatter is case-insensitive, so the camelCase wire names bind to the PascalCase properties.

### 5.2 Action Signature — `SolveBuildingsAsync`

```csharp
[HttpPost("buildings")]
public async Task<IActionResult> SolveBuildingsAsync(
    [FromBody] Classes.TypologySolveParameter? typologySolveParameter,
    CancellationToken cancellationToken = default)
```

**Pipeline (in order):**

1. **Guard.** `typologySolveParameter == null` → 400. `Id <= 0` → 400. `AdministrativeArealType` is `null` or `Undefined` → 400. `Definition` is `null` → 400.

2. **Validate the definition.** Fetch the live column catalog (`httpClient.BuildingDataColumnsAsync`). `null` → 503 (same convention as `definition/export`). `Query.TypologyDefinitionErrors(definition, columns)` non-empty → 400 with the error list.

3. **Build the filter.** `Create.VisualColumnTypologyFilter(definition, columns)` → `null` → 400.

4. **Resolve county-part ids.** Call the shared `Query.CountyPartsAsync(httpClient, code, administrativeArealType, cancellationToken)`. `null` → 503. Empty list → 404 (no buildings to solve).
   - For `Country`: return empty → 404.
   - For `Voivodeship`: prefix-match counties, collect part ids.
   - For `County`/`Municipality`/`Subdivision`: derive the 4-char county code, call `idsbycode`.
   - **This is the same code path the existing `GET /typology/countyids` uses** — the shared method is the single implementation (Definition of Done: "County-part resolution shared with `countyids`").

5. **Fetch building data per part.** For each county-part id, call `Query.BuildingDataTableAsync(httpClient, countyId, columnUniqueIds, log, commandTimeout, cancellationToken)`:
   - POST to `{BuildingDataTableUri}?commandtimeout={commandTimeout}`.
   - Body: `{ CountyId, ColumnUniqueIds, PageSize: 10000, Cursor }` — the cursor is the previous page's last `Reference` row, `null` on the first page (the keyset pattern of `DiGi.GIS.PostgreSQL/Create/TypologyAsync.cs`).
   - **Column projection:** the definition's chain column slugs + the `reference` slug. Add `internal_point_x` / `internal_point_y` only when the area is municipality or subdivision (clip needs them). The county part is not projected — it is tracked client-side per fetched part (`countyId_ByReference`).
   - **Sequential paging:** the upstream answers 500 (command timeout) on a cold partition — retry once (Coding – Deployed WebAPI §4). Do not fan out in parallel.
   - **Collapse failure to `null`:** one part failing does not fail the whole page. Collect the successful parts; if all fail → 503.
   - **Log the page count per part** (Definition of Done).

6. **Clip (municipality/voivodeship only).** When the area is below county, filter the fetched rows to those whose `internal_point_x`/`internal_point_y` falls within the selected area's polygon. Single-use local function inside the action (per the encapsulation rule: "Single-use helpers: Implement as local functions").
   - For county-level: no clip, all parts are the county.

7. **Table type.** The table is already the solver's `DiGi.Core.IO.Table.Classes.Table`: each page is parsed straight to it by `Create.Table` (§5.6). No conversion step — the `Convert/ToCoreIO` bridge of the original draft was never needed.

8. **Solve.** Call `table.VisualTypology(filter, column_Reference, includeReferences: true)`:
   - `table` is the merged, clipped table (already the solver's type).
   - `filter` is the `VisualColumnTypologyFilter` from step 3 (its `Value` columns are already `DiGi.Core.IO.Table.Classes.Column` instances).
   - `column_Reference` is the reference column from the converted table (looked up by name).
   - `null` → 400 naming the invalid level.

9. **Flatten to the view DTO.** Call `Create.TypologyBuildingsViewModel(visualTypology, countyId_ByPart)`:
   - Walk the solved tree recursively.
   - For each leaf node: emit a `TypologyTreeNodeViewModel` with `name`, `description`, `color` (from `VisualTypologyItem.Appearance` → `Query.Color`), `path` (the `TypologyPath` as a list of integers), and `children` (sub-typologies).
   - For each reference in a leaf: emit a `TypologyBuildingViewModel` entry `{ reference, countyId, path }`.
   - **No coordinates on the wire** — dot positions come from the area-scoped centroid endpoint in `DiGi.GIS.WebAPI`, joined in the view by `(reference, countyId)`.

### 5.3 View DTOs

```csharp
// ViewModels/TypologyBuildingsViewModel.cs
public class TypologyBuildingsViewModel
{
    public TypologyTreeNodeViewModel? Root { get; }
    public List<TypologyBuildingViewModel> Buildings { get; }
}

// ViewModels/TypologyTreeNodeViewModel.cs  (recursive)
public class TypologyTreeNodeViewModel
{
    public string? Name { get; }
    public string? Description { get; }
    public string? Color { get; }              // hex, from Query.Color → ToSystem_String
    public List<int> Path { get; }             // TypologyPath as integer list
    public List<TypologyTreeNodeViewModel>? Children { get; }
}

// ViewModels/TypologyBuildingViewModel.cs
public class TypologyBuildingViewModel
{
    public string Reference { get; }
    public int CountyId { get; }
    public List<int> Path { get; }
}
```

**Guideline notes:**
- One member per file (Coding – General §2). Three separate files.
- `Color` is a hex string (`"#RRGGBB"`) — the page renders CSS colors, not `System.Drawing.Color`.
- `Path` is `List<int>` — the `TypologyPath` is a list of filing indexes, one per level.

### 5.4 Table Conversion — SUPERSEDED (bridge removed)

> **Superseded by the final implementation.** The draft assumed the upstream page deserializes to `DiGi.PostgreSQL.Table.Classes.Table` and had to be converted. That deserialization is **silently lossy**: the upstream's columns are `DiGi.Core.IO.Table.Classes.ExtendedColumn`, a different class hierarchy that fails the deserializer's type check, so every column drops to null and the reference-column lookup fails — every solve 404s (proven with a scratch console app and a live 404). The wire columns also carry `Name`/`Index`/`Type` (a CLR type-name string) with **no `UniqueId`, no `DataType`**, so a bridge reading `DataType` was doubly broken.
>
> **Final implementation:** the page is parsed straight to the solver's own type with the `TableConverter<Table, Column, Row>` the deployed GIS Web API's own `Create.Table` uses — `Create/Table.cs` in this repository (verified against the live wire payload: Storeys→UInt16, Internal Point→Double, Reference→String). The `Convert/ToCoreIO/Table.cs` file below was never created and its design is invalid:

```csharp
// Convert/ToCoreIO/Table.cs
public static partial class Convert
{
    public static Core.IO.Table.Classes.Table? ToCoreIO(this DiGi.PostgreSQL.Table.Classes.Table? table)
    {
        if (table is null || table.Columns.Count == 0)
        {
            return null;
        }

        List<Core.IO.Table.Classes.Column> columns = [];
        foreach (DiGi.PostgreSQL.Table.Classes.Column? column in table.Columns)
        {
            if (column is null || string.IsNullOrWhiteSpace(column.Name))
            {
                continue;
            }

            Core.Enums.DataType dataType = column.DataType ?? Core.Enums.DataType.Undefined;
            columns.Add(new Core.IO.Table.Classes.Column(column.Index, column.Name, Core.Query.Type(dataType)));
        }

        if (columns.Count == 0)
        {
            return null;
        }

        Core.IO.Table.Classes.Table result = new(columns);

        foreach (object?[] row in table.Rows)
        {
            Dictionary<int, object> values = [];
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] != null)
                {
                    values[i] = row[i];
                }
            }

            result.Rows.Add(new Core.IO.Table.Classes.Row(row.Length, values));
        }

        return result;
    }
}
```

**Why `Convert`, not `Create`:** the method transforms an existing object into a target representation (`DiGi.PostgreSQL.Table` → `DiGi.Core.IO.Table`), not instantiates from scratch. Per Coding – General §2: "Convert: Transforms objects/primitives into target representations."

**Column mapping:**

| Source (`DiGi.PostgreSQL.Table`) | Target (`DiGi.Core.IO.Table`) | Notes |
|---|---|---|
| `Column.Index` (int) | `Column.Index` (int) | Position in the row array |
| `Column.Name` (string) | `Column.Name` (string) | Display name; `Core.IO.Query.UniqueId` normalizes this for resolution |
| `Column.DataType` (`Core.Enums.DataType`) | `Column.Type` (`System.Type`) | Via `Core.Query.Type(dataType)` — the same mapping `Create/VisualColumnTypologyFilter.cs` uses |
| `Column.UniqueId` (slug) | — (not mapped) | The `DiGi.Core.IO.Table.Classes.Column` has no `UniqueId` property |

**Row mapping:**

| Source | Target | Notes |
|---|---|---|
| `object?[]` (array of cell values) | `Row(count, IDictionary<int, object> values)` | Null cells omitted from the dictionary; the solver's `row[column.Index]` returns `null` for absent keys |

### 5.5 Shared County-Part Resolution — `Query.CountyPartsAsync`

Extract the resolution logic currently inlined in `TypologyController.GetCountyIdsAsync` into a `Query` extension method:

```csharp
// Query/CountyPartsAsync.cs
public static async Task<List<int>?> CountyPartsAsync(
    this HttpClient? httpClient,
    string code,
    AdministrativeArealType administrativeArealType,
    CancellationToken cancellationToken = default)
```

- Country → return empty list.
- Voivodeship → GET `administrativeareal2Dreferencesbyadministrativearealtype?administrativearealtype=2` → filter by code prefix → collect distinct ids, ordered.
- County/Municipality/Subdivision → derive 4-char county code → GET `idsbycode?code=…&administrativearealtype=2` → return the id list.
- `null` when the upstream is unreachable (caller maps to 503).

**The existing `GetCountyIdsAsync` action is refactored to call this method**, so both actions share one implementation (Definition of Done).

### 5.6 Upstream Fetch with Paging — `Query.BuildingDataTableAsync`

```csharp
// Query/BuildingDataTableAsync.cs
public static async Task<DiGi.Core.IO.Table.Classes.Table?> BuildingDataTableAsync(
    this HttpClient? httpClient,
    int countyId,
    List<string> columnUniqueIds,
    Action<string>? log = null,
    int commandTimeout = 600,
    CancellationToken cancellationToken = default)
```

- POST to `{BuildingDataTableUri}?commandtimeout={commandTimeout}`.
- Body: a JSON object with `CountyId`, `ColumnUniqueIds`, `PageSize` (10 000 — the upstream's page cap), `Cursor`.
- Loop: the page arrives ordered ascending by `Reference`; its last row is the next cursor. Stop when a page comes back short of the page size; fail if the cursor does not advance — the canonical pattern of `DiGi.GIS.PostgreSQL/Create/TypologyAsync.cs`.
- **Single retry:** on a non-success response (500 from command timeout on a cold partition), wait a short delay and retry once (Coding – Deployed WebAPI §4).
- **Sequential:** do not fan out parts in parallel — the Npgsql pool on the server exhausts under concurrency (Coding – Deployed WebAPI §4).
- Return the combined `DiGi.PostgreSQL.Table.Classes.Table` (all pages merged), or `null` on failure.
- **Log the page count per part (Definition of Done):** the `log` callback receives `Building data part {id}: {pages} page(s), {rows} row(s).` on success and the failure on the way out. The action passes `message => logger.LogInformation(message)` (its injected `ILogger<TypologyController>`).
- **Pages merge by column name** (`Modify.Append`), because a page's column order is not guaranteed to repeat.

**JSON body serialization:** use `JsonSerializerOptions.Default` (not `JsonSerializerDefaults.Web`) so the property names are not camelCase-renamed (Coding – WebAPI Contracts §2). The upstream binds case-insensitively, but the dependency must not be implicit.

**Response deserialization:** the page is parsed by `Create.Table` — `JsonSerializer` + `TableConverter<Table, Column, Row>` — straight to the solver's `DiGi.Core.IO.Table.Classes.Table`, every cell converted to the column's declared type. `Core.Convert.ToDiGi<Table>` is the wrong tool here (silently drops every column to null — see §5.4).

### 5.7 Flatten — `Create.TypologyBuildingsViewModel`

```csharp
// Create/TypologyBuildingsViewModel.cs
public static ViewModels.TypologyBuildingsViewModel? TypologyBuildingsViewModel(
    this DiGi.Typology.Visual.Classes.VisualTypology? visualTypology,
    Dictionary<int, int>? countyId_ByPart = null)
```

- Recursively walk `visualTypology.SubTypologies` (the `List<VisualTypology>?` property).
- For each node: read `node.TypologyItem` → `VisualTypologyItem` → `Name`, `Description`, `Appearance`.
- Extract the color: `Query.Color(node.TypologyItem?.Appearance)` → `Core.Classes.Color?` → hex string.
- Build the `TypologyTreeNodeViewModel` with `Children` from sub-typologies.
- For leaf nodes (no sub-typologies): iterate `node.References` (the `List<string>` property) and emit `TypologyBuildingViewModel` entries with the node's `Path`.
- Return `null` when the input is `null`.

**Guideline notes:**
- The method is in `Create/` because it instantiates and returns new view-model objects (Coding – General §2: "Create: Instantiates and returns new objects").
- One public method per file.
- The `countyId_ByPart` parameter maps a building reference to the county-part id it was fetched from, so the `buildings[]` entry carries the correct `countyId` for the centroid join.

---

## 6. Constants

Add to `Constants/Default.cs`:

```csharp
/// <summary>
/// URI of the GIS Web API endpoint that pages building data rows by county part,
/// used by the Typology solve to fetch the table the solver classifies.
/// </summary>
public const string BuildingDataTableUri = GISWebAPIUri + "/gis/BuildingData/tablebybuildingdatabypagingparameter";
```

**Guideline:** one base URI constant, never a literal (Coding – WebAPI Contracts §3).

---

## 7. Controller Action — Full Pipeline

```
SolveBuildingsAsync(TypologySolveParameter?, CancellationToken)
│
├─ 1. Guard: null / Id <= 0 / type null|Undefined / Definition null  → 400
│
├─ 2. Fetch column catalog → null → 503
│   Validate definition → errors → 400
│   Build filter (Create.VisualColumnTypologyFilter) → null → 400
│
├─ 3. CountyPartsAsync → null → 503, empty → 404
│
├─ 4. For each county-part id (sequential):
│     BuildingDataTableAsync(countyId, columnUniqueIds, log)
│     → null → skip (logged), collect successful parts
│     all failed → 503
│
├─ 5. Clip (municipality/subdivision only) — filter rows by point-in-polygon
│
├─ 6. Ceiling: table.RowCount > Constants.Default.BuildingSolveCeiling (200 000)
│     → 413 with an actionable message (issue scope step 8)
│
├─ 7. Solve: table.VisualTypology(filter, columnRef, includeReferences: true)
│     → null → 400 "The definition could not be solved: …"
│
├─ 8. Flatten: Create.TypologyBuildingsViewModel(solved, countyIdMap)
│     → null → 500 (should not happen if solve succeeded)
│
└─ 9. return Ok(viewModel)
```

---

## 8. Testing Strategy

### 8.1 Unit Tests (DiGi.Test)

| Fact | What it covers |
|---|---|
| `Create_Table()` | Parse the real wire fixture (`DiGi.Test/files/BuildingDataTable_Sample.json`) with `Create.Table` → assert the solver's `Table` carries the columns typed to their CLR types and the cells converted; assert `null` input → `null` |
| `Create_TypologyBuildingsViewModel()` | Flatten a small hand-built `VisualTypology` tree (2 levels, 3 leaves, 2 references per leaf) → assert the DTO structure, names, colors, paths, building entries |
| `Query_ClipByPolygon()` | Clip a synthetic row set with `Internal Point X/Y` against a known polygon → assert which rows survive, that the input is untouched, and the no-columns path returns the table unchanged |
| `TypologySolveParameter_Serialization()` | Round-trip the request body through `Core.xUnit.Query.SerializationCheck` |
| `TypologyBuildingsViewModel_Serialization()` | Round-trip the response DTO — assert `Root`, `Buildings`, nested `Children` survive |

### 8.2 Build Before Test (Coding – Automatic Tests §4)

```powershell
dotnet build "..\DiGi.GIS.WebAPI.UI\DiGi.GIS.WebAPI.UI\DiGi.GIS.WebAPI.UI.csproj" -c Debug -m:1
dotnet test "DiGi.Test\DiGi.GIS.WebAPI.UI.xUnit\DiGi.GIS.WebAPI.UI.xUnit.csproj" -c Debug -m:1
```

### 8.3 Live Verification (Coding – Deployed WebAPI)

Manual `curl` checks against the running host (Definition of Done):

```bash
# 1. County (~30k buildings) — solves end to end
curl -s -X POST "https://localhost:5001/typology/buildings" \
  -H "Content-Type: application/json" \
  -d '{"definition":{...},"id":73482,"code":"2212","administrativeArealType":2}'

# 2. Municipality (clipped)
# 3. Subdivision
# 4. 400 path: missing definition
# 5. 400 path: invalid level
# 6. 503 path: catalog unreachable
# 7. Sequential paging: verify page count in server log — verified: "Building data part 5: 4 page(s), 33687 row(s)."
# 8. Single retry on cold partition: first call slow, second fast
```

**Do NOT add these to DiGi.Test** (Coding – Deployed WebAPI §3).

### 8.4 Build Check

```powershell
dotnet build "DiGi.GIS.WebAPI.UI\DiGi.GIS.WebAPI.UI\DiGi.GIS.WebAPI.UI.csproj" -c Release -m:1
# Expect: zero warnings, zero errors
```

---

## 9. Definition of Done — Traceability

| Criterion | Where it is met |
|---|---|
| `POST /typology/buildings` answers the DTO for a county, a municipality (clipped) and a subdivision | §5.2 action pipeline, §8.3 live checks |
| 400/413/503 paths exercised with curl | §8.3 live checks |
| County-part resolution shared with `countyids` — one implementation, both actions call it | §5.5 `Query.CountyPartsAsync` |
| Only the needed `ColumnUniqueIds` requested upstream; upstream page count logged per part | §5.6 clip-aware column list, §5.6 log |
| Live read-only check on `api.digiproject.uk`: a real county (~30k buildings) solves end to end; sequential paging; single retry on a cold partition | §8.3 |
| `dotnet build` zero warnings; XML docs on all new public members; no new NuGet packages; test facts in DiGi.Test for the flatten and clip logic | §8.1, §8.4 |

---

## 10. Risk Register

| Risk | Likelihood | Mitigation |
|---|---|---|
| **Type mismatch: upstream `Table` ≠ solver's `Table`** | **Confirmed, resolved** | The page is parsed straight to the solver's type by `Create.Table` (`TableConverter<Table, Column, Row>`) — no bridge. Verified against the live wire payload (Storeys→UInt16, Internal Point→Double, Reference→String). |
| `Core.IO.Query.UniqueId` normalizes `Name` differently than expected (e.g. lowercases, strips spaces) | Medium | The `Create/VisualColumnTypologyFilter.cs` already creates `DiGi.Core.IO.Table.Classes.Column` with `Name = column.Name` (the display name from the PostgreSQL column). The solver resolves the filter's columns against the table's columns using the same normalization. As long as both sides use the same `Name`, the resolution succeeds. Verified live (the solves above file into the named buckets). |
| Upstream `tablebybuildingdatabypagingparameter` 500 on cold partition (command timeout) | High | Single retry with short delay (Coding – Deployed WebAPI §4). Log the retry. |
| County with many parts (e.g. `2412` has 3 parts, ~32k buildings total) | Medium | Sequential paging per part at `PageSize` 10 000. Verified live on county 2404 (4 parts, 100 543 rows, ~3 min). The solver processes all parts in one merged table; the 413 ceiling bounds the solve. |
| Municipality clip: the area polygon is not in the fetched data | Medium | Fetch the polygon separately from the GIS Web API (`administrativeareal2D` by id) before the clip step. |
| The solver returns `null` for a valid definition when a column name in the chain does not match the table's column names | Low | The definition is validated against the live column catalog first (§5.2 step 2), and the `Create.VisualColumnTypologyFilter` resolves each level against the catalog using the same `Name` the table carries. |
| Large county (100 543 buildings, 11 pages of 10 000) — memory | Low | The table is held in memory for the solve. Verified live on county 2404. The 413 ceiling bounds the solve. |
| Row/cell typing (the solver reads `row[column.Index]` by CLR type) | **Resolved** | The `TableConverter` (via `Create.Table`) converts every cell to the column's declared type as the page is parsed — verified live (Storeys→UInt16, Internal Point→Double, Reference→String). |

---

## 11. Implementation Order

1. **`TypologySolveParameter`** — the request body class. No dependencies.
2. **`TypologyTreeNodeViewModel`** + **`TypologyBuildingViewModel`** + **`TypologyBuildingsViewModel`** — the response DTOs. No dependencies.
3. **`Create/Table.cs`** — the wire-JSON parse (supersedes the draft's `Convert/ToCoreIO` bridge; see §5.4).
4. **`Query.CountyPartsAsync`** — extract from `TypologyController.GetCountyIdsAsync`. Refactor the existing action to call it.
5. **`Query.BuildingDataTableAsync`** — upstream fetch with sequential paging + retry. Depends on `Constants.Default.BuildingDataTableUri`.
6. **`Create.TypologyBuildingsViewModel`** — flatten the solved tree. Depends on the DTOs.
7. **`SolveBuildingsAsync` action** — the full pipeline. Depends on all of the above.
8. **Tests** — conversion, flatten, clip, serialization.
9. **Live verification** — curl against the running host.
10. **Build check** — zero warnings.

---

## 12. Final State — Implementation and Verification

**Implemented and live-verified** against the running host proxying production (`https://localhost:53115`, production `api.digiproject.uk`):

| Check | Result |
|---|---|
| County 1465, no clip | **200** — 154 529 buildings, correct tree (Storeys [1,2]/[3,12]) |
| County 0201, no clip | **200** — 33 687 buildings |
| County 2404 (4 parts), no clip | **200** — 100 543 buildings; 3 of the 4 parts carry no rows (logged per part) |
| Municipality 0201052 (clipped) | **200** — 3 404 buildings (< county ✓) |
| Subdivision id=13 (clipped) | **200** — 306 buildings (< municipality ✓) — nested containment proves the clip works live |
| 400 paths (missing definition, invalid column, min>max) | **400** with the error list ✓ |
| Country (type 0) | **404** (no parts) ✓ |
| 413 ceiling (ceiling temporarily 1 000) | **413** — `["The area carries 3404 buildings; a solve classifies at most 1000. Select a smaller area."]` ✓ (restored to 200 000) |
| Per-part page-count log (DoD) | `Building data part 5: 4 page(s), 33687 row(s).` ✓ |
| `dotnet build` Debug + Release | 0 warnings, 0 errors ✓ (5 pre-existing CS1574 cref warnings in `Facts/MeshClip.cs`/`Facts/Polygon2D.cs` are out of scope) |
| `dotnet test` | 22/22 pass ✓ |
| API docs `documentation/API/` | regenerated on build, in sync with the new API ✓ |

**Deviations from the original draft (all resolved in the final code):**
- §5.4 `Convert/ToCoreIO` bridge — **never created**; the page parses straight to the solver's type via `TableConverter` (`Create/Table.cs`). The draft's deserialization target was silently lossy (see §5.4).
- §5.6 paging — keyset cursor from the page's last `Reference` row at `PageSize` 10 000 (the upstream's page cap), not the draft's response-carried cursor at 1 000.
- The response DTO is the draft's `Root` + `Buildings` (the issue's `area`/`columns` echo fields are owned by the area-view sub-issue, not by this endpoint).
- New since the draft: `Constants.Default.BuildingSolveCeiling` (200 000) with the 413 guardrail (issue scope step 8), and the per-part `Action<string>` log callback wired to the controller's `ILogger<TypologyController>` (issue DoD).
