#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.Constants Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData'></a>

## BuildingData Class

Provides the canonical names and unique identifiers of the building data table's built\-in columns, as the deployed GIS Web API carries them on the wire\.

The names are the `Name` of the columns the GIS Web API's own `DiGi.GIS.IO.Constants.Column` defines (reference, county, database identifier, internal point), and are the keys a fetched table is addressed by. The unique identifiers are the catalog's `UniqueId` slugs of the same columns - the keys a projection is requested by, and what a Typology definition level names. Both are spelled here, once, because this application reaches the GIS Web API over the wire only and cannot reference `DiGi.GIS.IO` for them.

```csharp
public static class BuildingData
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → BuildingData
### Fields

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.CountyIdName'></a>

## BuildingData\.CountyIdName Field

The name of the building data table's county part column: the partition the row is filed under, always projected by the upstream paging endpoint whether asked for or not\.

```csharp
public const string CountyIdName = "County Id";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.DatabaseIdName'></a>

## BuildingData\.DatabaseIdName Field

The name of the building data table's database identifier column: the `Building2DReference.Id` the 2D details and the 3D viewer routes address a building by\.

```csharp
public const string DatabaseIdName = "Database Id";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.DatabaseIdUniqueId'></a>

## BuildingData\.DatabaseIdUniqueId Field

The unique identifier \(projection slug\) of the [DatabaseIdName](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.BuildingData.DatabaseIdName 'DiGi\.GIS\.WebAPI\.UI\.Constants\.BuildingData\.DatabaseIdName') column\.

```csharp
public const string DatabaseIdUniqueId = "database_id";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.InternalPointXName'></a>

## BuildingData\.InternalPointXName Field

The name of the building data table's internal point X column, read by the clip when the area is below county level\.

```csharp
public const string InternalPointXName = "Internal Point X";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.InternalPointXUniqueId'></a>

## BuildingData\.InternalPointXUniqueId Field

The unique identifier \(projection slug\) of the [InternalPointXName](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.BuildingData.InternalPointXName 'DiGi\.GIS\.WebAPI\.UI\.Constants\.BuildingData\.InternalPointXName') column\.

```csharp
public const string InternalPointXUniqueId = "internal_point_x";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.InternalPointYName'></a>

## BuildingData\.InternalPointYName Field

The name of the building data table's internal point Y column, read by the clip when the area is below county level\.

```csharp
public const string InternalPointYName = "Internal Point Y";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.InternalPointYUniqueId'></a>

## BuildingData\.InternalPointYUniqueId Field

The unique identifier \(projection slug\) of the [InternalPointYName](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.BuildingData.InternalPointYName 'DiGi\.GIS\.WebAPI\.UI\.Constants\.BuildingData\.InternalPointYName') column\.

```csharp
public const string InternalPointYUniqueId = "internal_point_y";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.ReferenceName'></a>

## BuildingData\.ReferenceName Field

The name of the building data table's reference column: it names the buildings the solve files into buckets, and it is the keyset cursor that pages a part\. Always projected by the upstream paging endpoint whether asked for or not\.

```csharp
public const string ReferenceName = "Reference";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.BuildingData.ReferenceUniqueId'></a>

## BuildingData\.ReferenceUniqueId Field

The unique identifier \(projection slug\) of the [ReferenceName](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.BuildingData.ReferenceName 'DiGi\.GIS\.WebAPI\.UI\.Constants\.BuildingData\.ReferenceName') column\.

```csharp
public const string ReferenceUniqueId = "reference";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default'></a>

## Default Class

Provides default values used during conversion of GIS domain objects to glTF\.

```csharp
public static class Default
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Default
### Fields

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.Building2DCentroidsUri'></a>

## Default\.Building2DCentroidsUri Field

URI of the GIS Web API endpoint answering the building centroids of an administrative area as one compact, columnar document \- `{"References":[…],"CountyIds":[…],"X":[…],"Y":[…]}` \- the 2D dot layer of the Typology area view is drawn from \(DiGi\.GIS\.WebAPI\#40\)\.

```csharp
public const string Building2DCentroidsUri = "https://api.digiproject.uk/gis/building2D/centroidsbyadministrativeareal2Did";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingDataColumnsUri'></a>

## Default\.BuildingDataColumnsUri Field

URI of the GIS Web API endpoint listing the columns of the building data table, which the Typology definition page offers for grouping and the definition import resolves its columns against\.

```csharp
public const string BuildingDataColumnsUri = "https://api.digiproject.uk/gis/BuildingData/columns";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingDataHistogramUri'></a>

## Default\.BuildingDataHistogramUri Field

URI of the GIS Web API endpoint answering the value distribution histogram \(bucket, actual bucket min/max, building count\) of one building data column inside a county part, which the Typology definition Load reads to split an area's buildings into equal\-count ranges \(issue \#30\)\.

```csharp
public const string BuildingDataHistogramUri = "https://api.digiproject.uk/gis/BuildingData/histogramsummary";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingDataPageSize'></a>

## Default\.BuildingDataPageSize Field

The number of rows the Typology solve asks [BuildingDataTableUri](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.BuildingDataTableUri 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.BuildingDataTableUri') for per page \- the upstream cap\.

Page size does not decide the cost of the read: a warm 10 000-row page answers in about 0.1 s and a cold one is bound by the database reading its heap (DiGi.GIS.WebAPI.UI#29). The cap only keeps the number of round trips down.

```csharp
public const int BuildingDataPageSize = 10000;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingDataTableUri'></a>

## Default\.BuildingDataTableUri Field

URI of the GIS Web API endpoint that pages building data rows by county part, used by the Typology solve to fetch the table the solver classifies\.

```csharp
public const string BuildingDataTableUri = "https://api.digiproject.uk/gis/BuildingData/tablebybuildingdatabypagingparameter";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingModelItemsByCircleUri'></a>

## Default\.BuildingModelItemsByCircleUri Field

URI of the GIS Web API endpoint answering every stored building model whose footprint lies within a circle \(`x`, `y`, `radius`\), from which the solar radiation calculation takes the neighbours casting shade on the analysed building\.

```csharp
public const string BuildingModelItemsByCircleUri = "https://api.digiproject.uk/gis/buildingmodel/itemsbycircle";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingSearchRadius'></a>

## Default\.BuildingSearchRadius Field

The radius, in metres, searched around a plan position to find the building standing there\.

The 3D viewer knows a picked building by its centroid rather than by its identifier, so the building is recovered by asking for everything within this distance of that point. Small on purpose: it has to be forgiving of the difference between a footprint centroid and a model centroid without reaching a neighbouring building.

```csharp
public const double BuildingSearchRadius = 5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingSearchTolerance'></a>

## Default\.BuildingSearchTolerance Field

The tolerance, in metres, applied to the spatial query behind [BuildingSearchRadius](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.BuildingSearchRadius 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.BuildingSearchRadius')\.

```csharp
public const double BuildingSearchTolerance = 5;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.BuildingSolveCeiling'></a>

## Default\.BuildingSolveCeiling Field

The ceiling on the total number of buildings one Typology solve classifies; an area above it is refused with a 413 and an actionable message instead of timing out the fetch and the solve\.

Chosen above the largest county verified live (code 1465 with 154 529 buildings) and below any voivodeship, so a county still solves while a voivodeship or country scope degrades gracefully (issue #22 guardrail).

```csharp
public const int BuildingSolveCeiling = 200000;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.CommunicationWebAPIUri'></a>

## Default\.CommunicationWebAPIUri Field

Base URI of the DiGi\.Communication\.WebAPI extension \(hosted by the generic DiGi\.WebAPI\.WindowsService\) used in production\.

```csharp
public const string CommunicationWebAPIUri = "https://api.digiproject.uk";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.CommunicationWebAPIUri_Development'></a>

## Default\.CommunicationWebAPIUri\_Development Field

Base URI of the DiGi\.Communication\.WebAPI extension \(hosted by the generic DiGi\.WebAPI\.WindowsService\) used during local development\.

Points at the production service: no DiGi.WebAPI.WindowsService host runs locally by default, and a dead localhost URI made every V2 calculation fail with HTTP 500 (connection refused). Restore a localhost URI (matching the local host port) only when debugging the Communication extension locally.

```csharp
public const string CommunicationWebAPIUri_Development = "https://api.digiproject.uk";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.DisplayRadiusFastMax'></a>

## Default\.DisplayRadiusFastMax Field

The maximum radius in meters for fast loading 3D display areas without user warnings\.

```csharp
public const double DisplayRadiusFastMax = 1000;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.DisplayRadiusMax'></a>

## Default\.DisplayRadiusMax Field

The maximum allowable radius in meters for 3D display areas\.

```csharp
public const double DisplayRadiusMax = 1500;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.EPWFileItemUri'></a>

## Default\.EPWFileItemUri Field

URI of the GIS Web API endpoint answering the EPW weather file of the station serving a location \(`x`, `y`\), the weather the solar radiation calculation integrates over one year\.

The station differs by location, and so does the file's quality: for Warsaw Ursynów it serves IWEC `WARSAW`, whose snow depth is a filler value, and elsewhere IMGW `Warszawa Okecie`, whose albedo is all missing (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).

```csharp
public const string EPWFileItemUri = "https://api.digiproject.uk/gis/epwfile/item";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.GISWebAPIUri'></a>

## Default\.GISWebAPIUri Field

Base URI of the GIS Web API \(DiGi\.GIS\.WebAPI, hosted by the generic DiGi\.WebAPI\.WindowsService\) this application proxies\.

Every outbound request this application makes is built on this value, so the whole application can be pointed at another host by changing it here. The service is deployed on a separate machine and is versioned independently of this application - query `GET /information/controllers` on it to learn which build is actually answering before relying on a recently added endpoint.

```csharp
public const string GISWebAPIUri = "https://api.digiproject.uk";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.GISWebAPIUri_Development'></a>

## Default\.GISWebAPIUri\_Development Field

Base URI of the GIS Web API used during local development\.

Points at the production service for the same reason [CommunicationWebAPIUri\_Development](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.CommunicationWebAPIUri_Development 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.CommunicationWebAPIUri\_Development') does: no host runs locally by default, and a dead localhost URI turns every page of this application into an error. Restore a localhost URI (matching the local host port) only when debugging the GIS Web API locally.

```csharp
public const string GISWebAPIUri_Development = "https://api.digiproject.uk";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.HistogramBucketCount'></a>

## Default\.HistogramBucketCount Field

The number of buckets the histogram relay asks for; 1000 is the upstream cap and, with the equal\-count bucketing the relay asks for \(issue \#37\), gives the Load's quantile boundaries a resolution of one part in a thousand of every county part's buildings whatever the value distribution\.

The cost is one sort of the part's values and at most 1000 rows back — the same order as the 0.38 s partition scan measured for the equal-width request (issue #30).

```csharp
public const int HistogramBucketCount = 1000;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.NextCursorHeaderName'></a>

## Default\.NextCursorHeaderName Field

The response header in which [BuildingDataTableUri](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.BuildingDataTableUri 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.BuildingDataTableUri') returns the cursor of the next page of a physical\-order read \(DiGi\.GIS\.WebAPI\#40\)\. Absent when the county part is exhausted, and absent when the endpoint answered in reference order instead\.

Must equal the GIS Web API's `Constants.Header.NextCursor`: the client has no compile-time link to the API (Coding - WebAPI Contracts, section 1).

```csharp
public const string NextCursorHeaderName = "DiGi-Next-Cursor";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.OrtoDataImageByReferenceUri'></a>

## Default\.OrtoDataImageByReferenceUri Field

URI of the GIS Web API endpoint serving the orthophoto image of a building for one year, as JPEG bytes\.

The `fallbackbyreference` parameter the relay sends is ignored by builds that predate it.

```csharp
public const string OrtoDataImageByReferenceUri = "https://api.digiproject.uk/gis/ortodatas/imagebyreference";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.OrtoDataRandomBuilding2DReferenceUri'></a>

## Default\.OrtoDataRandomBuilding2DReferenceUri Field

URI of the GIS Web API endpoint drawing the next building to verify: one with orthophoto coverage and no user\-provided year built yet\.

Requires a signed-in session.

```csharp
public const string OrtoDataRandomBuilding2DReferenceUri = "https://api.digiproject.uk/gis/ortodatas/randombuilding2dreference";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.OrtoDataYearsByReferenceUri'></a>

## Default\.OrtoDataYearsByReferenceUri Field

URI of the GIS Web API endpoint listing the photo years held for a building \- the only years the Orto Data page renders a card for\.

Requires a signed-in session.

```csharp
public const string OrtoDataYearsByReferenceUri = "https://api.digiproject.uk/gis/ortodatas/yearsbyreference";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonMinimumPointCount'></a>

## Default\.PolygonMinimumPointCount Field

The fewest points a reduced outline is allowed to keep, for an administrative area with no rule of its own and for a building footprint\.

```csharp
public const int PolygonMinimumPointCount = 100;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonMinimumPointCount_Country'></a>

## Default\.PolygonMinimumPointCount\_Country Field

The fewest points a reduced country outline is allowed to keep\.

```csharp
public const int PolygonMinimumPointCount_Country = 30;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonMinimumPointCount_Voivodeship'></a>

## Default\.PolygonMinimumPointCount\_Voivodeship Field

The fewest points a reduced voivodeship outline is allowed to keep\.

```csharp
public const int PolygonMinimumPointCount_Voivodeship = 50;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor'></a>

## Default\.PolygonReductionFactor Field

The reduction factor applied to an outline of an administrative area that has no rule of its own\.

These outlines are drawn as an overview map a few hundred pixels across, so they are simplified before they are sent rather than after. The factor falls as the area grows: a country outline carries far more points than the map can show, a subdivision barely more.

```csharp
public const double PolygonReductionFactor = 0.01;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor_Country'></a>

## Default\.PolygonReductionFactor\_Country Field

The reduction factor applied to a country outline\. See [PolygonReductionFactor](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.PolygonReductionFactor')\.

```csharp
public const double PolygonReductionFactor_Country = 1E-05;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor_County'></a>

## Default\.PolygonReductionFactor\_County Field

The reduction factor applied to a county outline\. See [PolygonReductionFactor](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.PolygonReductionFactor')\.

```csharp
public const double PolygonReductionFactor_County = 0.001;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor_Voivodeship'></a>

## Default\.PolygonReductionFactor\_Voivodeship Field

The reduction factor applied to a voivodeship outline\. See [PolygonReductionFactor](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.PolygonReductionFactor 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.PolygonReductionFactor')\.

```csharp
public const double PolygonReductionFactor_Voivodeship = 0.001;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarAngleTolerance'></a>

## Default\.SolarAngleTolerance Field

The angular tolerance, in radians, below which the solar radiation calculation groups sun directions into one shading solve \(`ShadingSolverOptions.AngleTolerance`\); 2°, twice the solver default\.

Measured over a full EPW year (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537): it halves the direction groups (615 → 295) and the solve time, and changes the annual per-surface irradiation by at most 2.06 %. #7 measured on a 16-thread machine, not on this host; the relative saving holds on any machine.

```csharp
public const double SolarAngleTolerance = 0.0349066;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarCasterTriangleCountMax'></a>

## Default\.SolarCasterTriangleCountMax Field

The ceiling on the number of shading\-only triangles \(neighbours and the analysed building's own non\-receiving components\) one synchronous solar radiation request solves against; above it the request is refused with a 413\.

A 50 m radius in central Warsaw gave 8 600–10 700 caster triangles (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537), which the ceiling still admits at the default radius. On this host, a 4-core Intel N150 (DiGi.GIS.WebAPI.UI#59), casters cost more than on the 16-thread machine #7 measured: 25 receivers among 8 600 caster triangles took 42 s against 23 s for 28 receivers among a few. Larger surroundings are calculated as background jobs, up to [SolarJobCasterTriangleCountMax](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobCasterTriangleCountMax 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarJobCasterTriangleCountMax') (DiGi.GIS.WebAPI.UI#60).

```csharp
public const int SolarCasterTriangleCountMax = 12000;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarConcurrentSolveCount'></a>

## Default\.SolarConcurrentSolveCount Field

The number of solar radiation solves that may run at the same time on this host; further requests and background jobs wait for the gate registered under [SolarSolveGateKey](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarSolveGateKey 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarSolveGateKey'), a request for at most [SolarSolveGateWaitSeconds](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarSolveGateWaitSeconds 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarSolveGateWaitSeconds')\.

One solve already uses every core: two parallel solves took 38.9 s each against 23.0 s for one on the 16-thread machine #7 measured, so a second request waits less on average when queued (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537). This host has 4 cores, which makes the case for one slot stronger.

```csharp
public const int SolarConcurrentSolveCount = 1;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarIrradiationScaleMax'></a>

## Default\.SolarIrradiationScaleMax Field

The irradiation, in kWh/m² per year, at the top of the colour ramp of the solar radiation viewer; higher values take the top colour\.

Fixed rather than fitted to each building so that two buildings read the same colour for the same irradiation. The best roofs of the measured Warsaw buildings received 997–1 041 kWh/m² against an annual global horizontal irradiation of 978–999 kWh/m² (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).

```csharp
public const double SolarIrradiationScaleMax = 1200;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobCasterTriangleCountMax'></a>

## Default\.SolarJobCasterTriangleCountMax Field

The ceiling on the number of shading\-only triangles one background solar radiation job solves against; above it the job is refused with a 413\.

Provisional (DiGi.GIS.WebAPI.UI#60): it covers the largest surroundings measured, B3 at a 100 m radius with 30 519 caster triangles, which peaked at 6.9 GB on the 16-thread machine of ZiolkowskiJakub/DiGi.Solar#7 - below this host's 16 GB. The peak on this host is not measured yet.

```csharp
public const int SolarJobCasterTriangleCountMax = 32000;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobPollSeconds'></a>

## Default\.SolarJobPollSeconds Field

The number of seconds between two status requests of the solar radiation viewer while it waits for a background job\.

```csharp
public const int SolarJobPollSeconds = 5;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobQueueLengthMax'></a>

## Default\.SolarJobQueueLengthMax Field

The number of background solar radiation jobs that may wait in the queue, not counting the one running; a further job is refused with a 503 and a `Retry-After` of [SolarJobRetryAfterSeconds](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobRetryAfterSeconds 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarJobRetryAfterSeconds')\.

At up to about 12 minutes per job ([SolarJobReceiverCountMax](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobReceiverCountMax 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarJobReceiverCountMax')), the last queued job waits about 35 minutes before its own calculation starts (DiGi.GIS.WebAPI.UI#60).

```csharp
public const int SolarJobQueueLengthMax = 3;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobReceiverCountMax'></a>

## Default\.SolarJobReceiverCountMax Field

The ceiling on the number of receiving surfaces \(external walls and roofs\) of the building one background solar radiation job calculates; above it the job is refused with a 413\.

Provisional (DiGi.GIS.WebAPI.UI#60): at the ~1.7 s per receiver measured on this host (see [SolarReceiverCountMax](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarReceiverCountMax 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarReceiverCountMax')), 400 receivers is about 11–12 minutes per job, and it admits the 320-receiver block B3 of ZiolkowskiJakub/DiGi.Solar#7. To be confirmed by a measurement on the web server.

```csharp
public const int SolarJobReceiverCountMax = 400;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobResultRetentionMinutes'></a>

## Default\.SolarJobResultRetentionMinutes Field

The number of minutes a finished, failed or cancelled background solar radiation job, and its results, are kept after it finished; afterwards the job answers 404\. Queued and running jobs never expire\.

Keeping the results until they are fetched is the asynchronous contract, not a cache: nothing is reused across jobs. Jobs live in memory only and are lost when the application restarts.

```csharp
public const int SolarJobResultRetentionMinutes = 60;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobRetryAfterSeconds'></a>

## Default\.SolarJobRetryAfterSeconds Field

The number of seconds in the `Retry-After` header of the 503 a background solar radiation job is refused with when the queue is full \([SolarJobQueueLengthMax](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobQueueLengthMax 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarJobQueueLengthMax')\)\.

```csharp
public const int SolarJobRetryAfterSeconds = 300;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarReceiverCountMax'></a>

## Default\.SolarReceiverCountMax Field

The ceiling on the number of receiving surfaces \(external walls and roofs\) of the building one synchronous solar radiation request calculates; above it the request is refused with a 413\.

Measured on this host, a 4-core Intel N150 with 16 GB, whole requests in central Warsaw at the default radius took about 1.7 s per receiver: 42 s at 25 receivers, 56 s at 34, 65 s at 37 and 51–159 s at 43–48, repeated runs of one building varying up to 2.5 times (DiGi.GIS.WebAPI.UI#59). At 30 receivers a typical request stays near one minute, well under the ~135 s at which the front end answered 503. ZiolkowskiJakub/DiGi.Solar#7 proposed 100, but measured a 16-thread machine, not this host. Refine it from the per-request log of `SolarController` (`logs\log-yyyyMMdd.txt`). Larger buildings are calculated as background jobs, up to [SolarJobReceiverCountMax](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarJobReceiverCountMax 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarJobReceiverCountMax') (DiGi.GIS.WebAPI.UI#60).

```csharp
public const int SolarReceiverCountMax = 30;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarReferenceYear'></a>

## Default\.SolarReferenceYear Field

The year every EPW record is mapped to before the sun position is calculated\. Typical meteorological years mix calendar years month by month, and the last record of the year rolls into the next one, so one fixed year keeps the time series monotonic\.

A non-leap year, so the 8 760 hours of an EPW year map one to one; a 29 February record is skipped.

```csharp
public const int SolarReferenceYear = 2025;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarSolveGateKey'></a>

## Default\.SolarSolveGateKey Field

The dependency injection key of the [System\.Threading\.SemaphoreSlim](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim 'System\.Threading\.SemaphoreSlim') of [SolarConcurrentSolveCount](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarConcurrentSolveCount 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarConcurrentSolveCount') slots that gates every solar radiation solve on this host\.

A keyed singleton rather than a static field so that the background jobs (`SolarJobHostedService`, DiGi.GIS.WebAPI.UI#60) share the same gate with the synchronous requests: a job and a request never solve at the same time.

```csharp
public const string SolarSolveGateKey = "SolarSolveGate";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarSolveGateWaitSeconds'></a>

## Default\.SolarSolveGateWaitSeconds Field

The number of seconds a synchronous solar radiation request waits for the gate registered under [SolarSolveGateKey](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarSolveGateKey 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarSolveGateKey') before it is refused with a 503 and a `Retry-After` of the same number of seconds\.

A background job holds the gate for many minutes (about 1.7 s per receiver on this host), and the front end answers 503 on its own at about 135 s (DiGi.GIS.WebAPI.UI#59). Thirty seconds of waiting plus a synchronous solve of about one minute stays under that, and the refusal tells the viewer why.

```csharp
public const int SolarSolveGateWaitSeconds = 30;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarSurroundingRadius'></a>

## Default\.SolarSurroundingRadius Field

The default neighbour radius, in metres, of a solar radiation request, measured from the edge of the analysed building's footprint: every building within it casts shade\.

Going from 50 to 200 m changed the shading loss of the three measured buildings by at most 0.1 point while the solve time rose by 10–125 % (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).

```csharp
public const double SolarSurroundingRadius = 50;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.SolarSurroundingRadiusMax'></a>

## Default\.SolarSurroundingRadiusMax Field

The upper bound, in metres, of the neighbour radius a solar radiation request may ask for; a larger one is refused with a 400\.

Headroom above [SolarSurroundingRadius](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarSurroundingRadius 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarSurroundingRadius') for tall distant casters in low winter sun, which the measured buildings do not cover; 200 m cost up to twice the solve time for no measured gain (ZiolkowskiJakub/DiGi.Solar#7, comment 5831808537).

```csharp
public const double SolarSurroundingRadiusMax = 100;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.StoreyHeight'></a>

## Default\.StoreyHeight Field

Default storey height in meters used to extrude 2D building footprints\.

```csharp
public const double StoreyHeight = 3;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainBuffer'></a>

## Default\.TerrainBuffer Field

The clip margin in metres added to a terrain query beyond the boundary the surface is clipped to, so that the clipping never runs along the very edge of the surface\.

This covers the clip only. The distance the surface can stop short of the query radius on a coarse lattice is a separate term - see [TerrainLatticeStepMax](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.TerrainLatticeStepMax 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.TerrainLatticeStepMax') - and [TerrainQueryCircle\(this Circle2D, double, double, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainQueryCircle(thisDiGi.Geometry.Planar.Classes.Circle2D,double,double,double) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainQueryCircle\(this DiGi\.Geometry\.Planar\.Classes\.Circle2D, double, double, double\)') adds both.

```csharp
public const double TerrainBuffer = 15;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainCircleSegmentCount'></a>

## Default\.TerrainCircleSegmentCount Field

The number of segments used to discretize a circular boundary into a regular 2D polygon during terrain clipping\.

```csharp
public const int TerrainCircleSegmentCount = 64;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainEnabled'></a>

## Default\.TerrainEnabled Field

Whether the ground surface is added to the scenes that display [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') geometry \(the 3D viewer and the communication view\)\.

The standalone terrain feature (the Terrain controller and its own pages) shows the surface on its own, where the elevation is correct as stored.

Note that an area smaller than the sampling lattice legitimately holds no points: the counties are sampled at 10 m to 100 m, so a request with a radius below the lattice step answers 404 without meaning that nothing was ever stored there.

```csharp
public const bool TerrainEnabled = True;
```

#### Field Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainLatticeStepMax'></a>

## Default\.TerrainLatticeStepMax Field

The coarsest lattice, in metres, the counties' elevation points are sampled on \(10 m to 100 m \- see [TerrainEnabled](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.TerrainEnabled 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.TerrainEnabled')\)\.

The terrain service triangulates only the stored points inside the query circle, so the surface it answers stops short of the query radius by up to one lattice diagonal: a point lies inside the returned surface once all four corners of its lattice cell are inside the query, and the farthest corner is `step * sqrt(2)` away. Growing a query by `TerrainLatticeStepMax * sqrt(2)` beyond the display boundary therefore guarantees the boundary is covered on any lattice up to this step, and it is deliberately the worst case rather than the county's own step so that no scene depends on knowing which county it is in. Measured on the deployed 100 m lattice the shortfall reaches 101 m; the bound is 141.4 m.

```csharp
public const double TerrainLatticeStepMax = 100;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainName'></a>

## Default\.TerrainName Field

The name given to the terrain node of a scene\.

```csharp
public const string TerrainName = "Terrain";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainPadding'></a>

## Default\.TerrainPadding Field

The margin in meters extending beyond building bounding envelopes when calculating dynamic terrain coverage\.

```csharp
public const double TerrainPadding = 50;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainRadius'></a>

## Default\.TerrainRadius Field

The default minimum radius of the ground surface, in metres, shown around a scene that holds a building model\.

A building scene ensures at least this radius of ground is displayed for context even when building footprints are small.

```csharp
public const double TerrainRadius = 100;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainRadiusMax'></a>

## Default\.TerrainRadiusMax Field

The maximum search radius in meters supported by the GIS Web API terrain service\.

```csharp
public const double TerrainRadiusMax = 2000;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainRequestTimeout'></a>

## Default\.TerrainRequestTimeout Field

The longest a terrain request to the GIS Web API may take, in seconds, before it is abandoned\.

Terrain is an optional overlay, so a stalled terrain query must not hold a page request open for the 100 second [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') default. Abandoning it is answered exactly like an area with no stored elevation points.

```csharp
public const double TerrainRequestTimeout = 30;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainUri'></a>

## Default\.TerrainUri Field

Base URI of the GIS Web API terrain endpoints\.

```csharp
public const string TerrainUri = "https://api.digiproject.uk/gis/terrain";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TypologyAppearanceThickness'></a>

## Default\.TypologyAppearanceThickness Field

The curve and edge thickness written into every appearance of a Typology definition document\.

The definition page edits a single color per bucket; the document carries a full [DiGi\.Typology\.Visual\.Classes\.TypologyAppearance](https://learn.microsoft.com/en-us/dotnet/api/digi.typology.visual.classes.typologyappearance 'DiGi\.Typology\.Visual\.Classes\.TypologyAppearance') per bucket, so the thickness the page does not edit is fixed here rather than invented per export.

```csharp
public const double TypologyAppearanceThickness = 1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserLoginUri'></a>

## Default\.UserLoginUri Field

URI of the endpoint that exchanges a set of credentials for a session token\.

```csharp
public const string UserLoginUri = "https://api.digiproject.uk/user/login";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserLogoutUri'></a>

## Default\.UserLogoutUri Field

URI of the endpoint that terminates the presented session, revoking its token until the token's natural expiration\.

```csharp
public const string UserLogoutUri = "https://api.digiproject.uk/user/logout";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserRefreshUri'></a>

## Default\.UserRefreshUri Field

URI of the endpoint that issues a new token for the identity carried by the presented one\.

```csharp
public const string UserRefreshUri = "https://api.digiproject.uk/user/session/refresh";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserSecureDataUri'></a>

## Default\.UserSecureDataUri Field

URI of the endpoint that reads the stored record of the authenticated user\.

```csharp
public const string UserSecureDataUri = "https://api.digiproject.uk/user/secure-data";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserSessionUri'></a>

## Default\.UserSessionUri Field

URI of the endpoint that introspects the presented session\.

```csharp
public const string UserSessionUri = "https://api.digiproject.uk/user/session";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserTokenCookieName'></a>

## Default\.UserTokenCookieName Field

The name of the cookie this application keeps a visitor's session token in\.

The token never reaches the browser as a value: the cookie is written HttpOnly by the server and read back by it on every relayed request, so a script on the page cannot read, copy or leak it. See [UserTokenCookieOptions\(\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Create.UserTokenCookieOptions() 'DiGi\.GIS\.WebAPI\.UI\.Create\.UserTokenCookieOptions\(\)').

```csharp
public const string UserTokenCookieName = "digi_user_token";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserWebAPIUri'></a>

## Default\.UserWebAPIUri Field

Base URI of the user authentication service \(DiGi\.User\.WebAPI, hosted by the generic DiGi\.WebAPI\.WindowsService\) this application signs its visitors in against\.

Kept apart from [GISWebAPIUri](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.GISWebAPIUri 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.GISWebAPIUri') even though both address the same host today: the authentication service is versioned and deployed independently, so pointing sign-in at another host must not move every GIS read with it.

```csharp
public const string UserWebAPIUri = "https://api.digiproject.uk";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.UserWebAPIUri_Development'></a>

## Default\.UserWebAPIUri\_Development Field

Base URI of the user authentication service used during local development\.

Points at the production service for the same reason [GISWebAPIUri\_Development](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.GISWebAPIUri_Development 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.GISWebAPIUri\_Development') does: no host runs locally by default, and a dead localhost URI would turn every sign-in attempt into a failure indistinguishable from a wrong password. Restore a localhost URI (matching the local host port) only when debugging DiGi.User.WebAPI locally.

```csharp
public const string UserWebAPIUri_Development = "https://api.digiproject.uk";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.YearBuiltDataSetUserYearBuiltUri'></a>

## Default\.YearBuiltDataSetUserYearBuiltUri Field

URI of the GIS Web API endpoint recording a reviewer's year built answer for a building\.

Requires a signed-in session.

```csharp
public const string YearBuiltDataSetUserYearBuiltUri = "https://api.digiproject.uk/gis/yearbuiltdata/setuseryearbuilt";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')