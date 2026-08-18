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

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.StoreyHeight'></a>

## Default\.StoreyHeight Field

Default storey height in meters used to extrude 2D building footprints\.

```csharp
public const double StoreyHeight = 3;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainEnabled'></a>

## Default\.TerrainEnabled Field

TERRAIN\. Whether the ground surface is added to the scenes that display [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') geometry \(the 3D viewer and the communication view\)\.

Off, because neither half of the pairing is ready. The GIS Web API terrain endpoints are not deployed and the elevation table they read does not exist yet; and the building models those scenes show are extruded from Building2D footprints at elevation 0, while a terrain surface carries its true height (around 110 m over Warsaw), so the two would not meet. Real stored building models are tracked by DiGi.GIS.PostgreSQL issue 2.

Turn on when both are true. Everything the feature adds to those scenes is behind a TERRAIN note naming this constant, so it can be found in one sweep and promoted or removed. The standalone terrain feature (the Terrain controller and its own pages) is not gated by this - it shows the surface on its own, where the elevation is correct as stored.

```csharp
public const bool TerrainEnabled = False;
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

<a name='DiGi.GIS.WebAPI.UI.Constants.Default.TerrainRadius'></a>

## Default\.TerrainRadius Field

The radius of the ground surface, in metres, shown around a scene that holds a single building\.

A single building has no requested area of its own to borrow, so this is how much ground it is given for context.

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