#### [DiGi\.GIS\.WebAPI\.UI](index.md 'index')

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

<a name='DiGi.GIS.WebAPI.UI.Classes.Building2DGLTFNodeConverter'></a>

## Building2DGLTFNodeConverter Class

Converts a [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances by extruding its 2D footprint by the number of storeys \(see [ToGLTF\_GLTFNodes\(this Building2D, double, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.GIS.Classes.Building2D,double,double) 'DiGi\.GIS\.WebAPI\.UI\.Convert\.ToGLTF\_GLTFNodes\(this DiGi\.GIS\.Classes\.Building2D, double, double\)')\)\.

Registered automatically at startup by assembly scanning (see Program.cs); the generic DiGi.GLTF engine consults it when converting [DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject') instances.

```csharp
public class Building2DGLTFNodeConverter : DiGi.GLTF.Classes.GLTFNodeConverter<DiGi.GIS.Classes.Building2D>
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [DiGi\.GLTF\.Classes\.GLTFNodeConverter&lt;](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnodeconverter-1 'DiGi\.GLTF\.Classes\.GLTFNodeConverter\`1')[DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnodeconverter-1 'DiGi\.GLTF\.Classes\.GLTFNodeConverter\`1') → Building2DGLTFNodeConverter
### Methods

<a name='DiGi.GIS.WebAPI.UI.Classes.Building2DGLTFNodeConverter.Convert(DiGi.GIS.Classes.Building2D,double)'></a>

## Building2DGLTFNodeConverter\.Convert\(Building2D, double\) Method

Converts the specified [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances holding geometry in world coordinates\.

```csharp
public override System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? Convert(DiGi.GIS.Classes.Building2D serializableObject, double tolerance);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Classes.Building2DGLTFNodeConverter.Convert(DiGi.GIS.Classes.Building2D,double).serializableObject'></a>

`serializableObject` [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')

The [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') to be converted\.

<a name='DiGi.GIS.WebAPI.UI.Classes.Building2DGLTFNodeConverter.Convert(DiGi.GIS.Classes.Building2D,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances, or null if the building has no geometry\.

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

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Radius'></a>

## CommunicationCalculationParameter\.Radius Property

Gets or sets the radius of the analyzed circular area in meters\.

```csharp
public double Radius { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.StoreyHeight'></a>

## CommunicationCalculationParameter\.StoreyHeight Property

Gets or sets the storey height in meters used for the building extrusions\.

```csharp
public System.Nullable<double> StoreyHeight { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')