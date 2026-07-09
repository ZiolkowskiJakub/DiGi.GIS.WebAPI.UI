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

```csharp
public const string CommunicationWebAPIUri_Development = "http://localhost:5010";
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