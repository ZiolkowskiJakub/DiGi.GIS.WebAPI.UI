#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.Constants Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Constants.Default'></a>

## Default Class

Provides default values used during conversion of GIS domain objects to glTF\.

```csharp
public static class Default
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Default
### Fields

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

The buffer distance in meters added to spatial terrain queries to guarantee the retrieved elevation lattice spans the target geometric boundary before regular clipping\.

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