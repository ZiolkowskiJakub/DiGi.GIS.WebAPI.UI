#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.Classes Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Classes.AntennaParameter'></a>

## AntennaParameter Class

Represents a single antenna sent by the communication 3D view: its world location and the communication functions selected by the user \(mapped to [DiGi\.Communication\.Enums\.Function](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.enums.function 'DiGi\.Communication\.Enums\.Function')\)\.

```csharp
public class AntennaParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → AntennaParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.AntennaParameter.Functions'></a>

## AntennaParameter\.Functions Property

Gets or sets the names of the selected [DiGi\.Communication\.Enums\.Function](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.enums.function 'DiGi\.Communication\.Enums\.Function') values \(e\.g\. Transmitter, Receiver\)\.

```csharp
public System.Collections.Generic.List<string>? Functions { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.AntennaParameter.X'></a>

## AntennaParameter\.X Property

Gets or sets the X coordinate of the antenna in world coordinates\.

```csharp
public double X { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.AntennaParameter.Y'></a>

## AntennaParameter\.Y Property

Gets or sets the Y coordinate of the antenna in world coordinates\.

```csharp
public double Y { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.AntennaParameter.Z'></a>

## AntennaParameter\.Z Property

Gets or sets the height of the antenna top above the ground plane \(Z = 0\) in meters\.

```csharp
public double Z { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter'></a>

## CommunicationCalculationParameter Class

Represents the payload of a communication calculation request sent by the communication 3D view: the analyzed circular area \(used to fetch the buildings on the fly\) and the antennas placed by the user\.

```csharp
public class CommunicationCalculationParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → CommunicationCalculationParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Antennas'></a>

## CommunicationCalculationParameter\.Antennas Property

Gets or sets the antennas placed by the user in the 3D view\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.Classes.AntennaParameter>? Antennas { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[AntennaParameter](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.AntennaParameter 'DiGi\.GIS\.WebAPI\.UI\.Classes\.AntennaParameter')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.CenterX'></a>

## CommunicationCalculationParameter\.CenterX Property

Gets or sets the X coordinate of the center of the analyzed circular area\.

```csharp
public double CenterX { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.CenterY'></a>

## CommunicationCalculationParameter\.CenterY Property

Gets or sets the Y coordinate of the center of the analyzed circular area\.

```csharp
public double CenterY { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Conductivity'></a>

## CommunicationCalculationParameter\.Conductivity Property

Gets or sets the default electrical conductivity applied to the scattering object mesh cells \[S/m\]\.

```csharp
public System.Nullable<double> Conductivity { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.DefaultSimpleMultipathPowerDelayProfile'></a>

## CommunicationCalculationParameter\.DefaultSimpleMultipathPowerDelayProfile Property

Gets or sets the default simple multipath power delay profile name\.

```csharp
public string? DefaultSimpleMultipathPowerDelayProfile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Frequencies'></a>

## CommunicationCalculationParameter\.Frequencies Property

Gets or sets the frequencies of the propagating electromagnetic wave \[MHz\]\.

AI-NOTE (multi-frequency extensibility): the payload is a list so the calculation can be executed for multiple frequencies in one request. The 3D view currently sends a single frequency; once the per frequency toggling UI is implemented, the additional values flow through this property without any backend change.

```csharp
public System.Collections.Generic.List<double>? Frequencies { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Polarization'></a>

## CommunicationCalculationParameter\.Polarization Property

Gets or sets the polarization type of the propagating electromagnetic wave \(Vertical or Horizontal\)\.

```csharp
public string? Polarization { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Radius'></a>

## CommunicationCalculationParameter\.Radius Property

Gets or sets the radius of the analyzed circular area in meters\.

```csharp
public double Radius { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.RelativePermittivity'></a>

## CommunicationCalculationParameter\.RelativePermittivity Property

Gets or sets the default relative electrical permittivity applied to the scattering object mesh cells \[\-\]\.

```csharp
public System.Nullable<double> RelativePermittivity { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')