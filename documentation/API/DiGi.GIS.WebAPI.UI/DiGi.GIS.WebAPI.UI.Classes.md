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

<a name='DiGi.GIS.WebAPI.UI.Classes.BuildingModelGLTFNodeConverter'></a>

## BuildingModelGLTFNodeConverter Class

Converts a [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances by converting all of its components \(see [ToGLTF\_GLTFNodes\(this BuildingModel, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Classes.BuildingModel,double) 'DiGi\.GIS\.WebAPI\.UI\.Convert\.ToGLTF\_GLTFNodes\(this DiGi\.Analytical\.Building\.Classes\.BuildingModel, double\)')\)\.

Registered automatically at startup by assembly scanning (see Program.cs).

```csharp
public class BuildingModelGLTFNodeConverter : DiGi.GLTF.Classes.GLTFNodeConverter<DiGi.Analytical.Building.Classes.BuildingModel>
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [DiGi\.GLTF\.Classes\.GLTFNodeConverter&lt;](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnodeconverter-1 'DiGi\.GLTF\.Classes\.GLTFNodeConverter\`1')[DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnodeconverter-1 'DiGi\.GLTF\.Classes\.GLTFNodeConverter\`1') → BuildingModelGLTFNodeConverter
### Methods

<a name='DiGi.GIS.WebAPI.UI.Classes.BuildingModelGLTFNodeConverter.Convert(DiGi.Analytical.Building.Classes.BuildingModel,double)'></a>

## BuildingModelGLTFNodeConverter\.Convert\(BuildingModel, double\) Method

Converts the specified [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances holding geometry in world coordinates\.

```csharp
public override System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? Convert(DiGi.Analytical.Building.Classes.BuildingModel serializableObject, double tolerance);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Classes.BuildingModelGLTFNodeConverter.Convert(DiGi.Analytical.Building.Classes.BuildingModel,double).serializableObject'></a>

`serializableObject` [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')

The [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') to be converted\.

<a name='DiGi.GIS.WebAPI.UI.Classes.BuildingModelGLTFNodeConverter.Convert(DiGi.Analytical.Building.Classes.BuildingModel,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances, or null if the model has no convertible components\.

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

<a name='DiGi.GIS.WebAPI.UI.Classes.ComponentGLTFNodeConverter'></a>

## ComponentGLTFNodeConverter Class

Converts a standalone building [DiGi\.Analytical\.Building\.Interfaces\.IComponent](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.interfaces.icomponent 'DiGi\.Analytical\.Building\.Interfaces\.IComponent') \(for example a wall or a roof\) into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances using its surface geometry \(see [ToGLTF\_GLTFNodes\(this IComponent, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Interfaces.IComponent,double) 'DiGi\.GIS\.WebAPI\.UI\.Convert\.ToGLTF\_GLTFNodes\(this DiGi\.Analytical\.Building\.Interfaces\.IComponent, double\)')\)\.

Implemented against [DiGi\.GLTF\.Interfaces\.IGLTFNodeConverter](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.interfaces.igltfnodeconverter 'DiGi\.GLTF\.Interfaces\.IGLTFNodeConverter') directly because [DiGi\.Analytical\.Building\.Interfaces\.IComponent](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.interfaces.icomponent 'DiGi\.Analytical\.Building\.Interfaces\.IComponent') is an interface matched by type test rather than a concrete serializable class. Registered automatically at startup by assembly scanning (see Program.cs).

```csharp
public class ComponentGLTFNodeConverter : DiGi.GLTF.Interfaces.IGLTFNodeConverter, DiGi.GLTF.Interfaces.IGLTFObject, DiGi.Core.Interfaces.IObject
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → ComponentGLTFNodeConverter

Implements [DiGi\.GLTF\.Interfaces\.IGLTFNodeConverter](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.interfaces.igltfnodeconverter 'DiGi\.GLTF\.Interfaces\.IGLTFNodeConverter'), [DiGi\.GLTF\.Interfaces\.IGLTFObject](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.interfaces.igltfobject 'DiGi\.GLTF\.Interfaces\.IGLTFObject'), [DiGi\.Core\.Interfaces\.IObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iobject 'DiGi\.Core\.Interfaces\.IObject')
### Methods

<a name='DiGi.GIS.WebAPI.UI.Classes.ComponentGLTFNodeConverter.CanConvert(DiGi.Core.Interfaces.ISerializableObject)'></a>

## ComponentGLTFNodeConverter\.CanConvert\(ISerializableObject\) Method

Determines whether this converter can convert the specified object\.

```csharp
public bool CanConvert(DiGi.Core.Interfaces.ISerializableObject serializableObject);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Classes.ComponentGLTFNodeConverter.CanConvert(DiGi.Core.Interfaces.ISerializableObject).serializableObject'></a>

`serializableObject` [DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')

The domain object to be checked\.

Implements [CanConvert\(ISerializableObject\)](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.interfaces.igltfnodeconverter.canconvert#digi-gltf-interfaces-igltfnodeconverter-canconvert(digi-core-interfaces-iserializableobject) 'DiGi\.GLTF\.Interfaces\.IGLTFNodeConverter\.CanConvert\(DiGi\.Core\.Interfaces\.ISerializableObject\)')

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if the object is an [DiGi\.Analytical\.Building\.Interfaces\.IComponent](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.interfaces.icomponent 'DiGi\.Analytical\.Building\.Interfaces\.IComponent'); otherwise, false\.

<a name='DiGi.GIS.WebAPI.UI.Classes.ComponentGLTFNodeConverter.Convert(DiGi.Core.Interfaces.ISerializableObject,double)'></a>

## ComponentGLTFNodeConverter\.Convert\(ISerializableObject, double\) Method

Converts the specified building component into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances holding geometry in world coordinates\.

```csharp
public System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? Convert(DiGi.Core.Interfaces.ISerializableObject serializableObject, double tolerance);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Classes.ComponentGLTFNodeConverter.Convert(DiGi.Core.Interfaces.ISerializableObject,double).serializableObject'></a>

`serializableObject` [DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')

The [DiGi\.Analytical\.Building\.Interfaces\.IComponent](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.interfaces.icomponent 'DiGi\.Analytical\.Building\.Interfaces\.IComponent') to be converted\.

<a name='DiGi.GIS.WebAPI.UI.Classes.ComponentGLTFNodeConverter.Convert(DiGi.Core.Interfaces.ISerializableObject,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

Implements [Convert\(ISerializableObject, double\)](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.interfaces.igltfnodeconverter.convert#digi-gltf-interfaces-igltfnodeconverter-convert(digi-core-interfaces-iserializableobject-system-double) 'DiGi\.GLTF\.Interfaces\.IGLTFNodeConverter\.Convert\(DiGi\.Core\.Interfaces\.ISerializableObject,System\.Double\)')

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances, or null if the component has no supported surface geometry\.

<a name='DiGi.GIS.WebAPI.UI.Classes.UrbanModelGLTFNodeConverter'></a>

## UrbanModelGLTFNodeConverter Class

Converts an [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances by converting all contained building models \(see [ToGLTF\_GLTFNodes\(this UrbanModel, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Urban.Classes.UrbanModel,double) 'DiGi\.GIS\.WebAPI\.UI\.Convert\.ToGLTF\_GLTFNodes\(this DiGi\.Analytical\.Urban\.Classes\.UrbanModel, double\)')\)\.

Registered automatically at startup by assembly scanning (see Program.cs).

```csharp
public class UrbanModelGLTFNodeConverter : DiGi.GLTF.Classes.GLTFNodeConverter<DiGi.Analytical.Urban.Classes.UrbanModel>
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [DiGi\.GLTF\.Classes\.GLTFNodeConverter&lt;](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnodeconverter-1 'DiGi\.GLTF\.Classes\.GLTFNodeConverter\`1')[DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnodeconverter-1 'DiGi\.GLTF\.Classes\.GLTFNodeConverter\`1') → UrbanModelGLTFNodeConverter
### Methods

<a name='DiGi.GIS.WebAPI.UI.Classes.UrbanModelGLTFNodeConverter.Convert(DiGi.Analytical.Urban.Classes.UrbanModel,double)'></a>

## UrbanModelGLTFNodeConverter\.Convert\(UrbanModel, double\) Method

Converts the specified [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances holding geometry in world coordinates\.

```csharp
public override System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? Convert(DiGi.Analytical.Urban.Classes.UrbanModel serializableObject, double tolerance);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Classes.UrbanModelGLTFNodeConverter.Convert(DiGi.Analytical.Urban.Classes.UrbanModel,double).serializableObject'></a>

`serializableObject` [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel')

The [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel') to be converted\.

<a name='DiGi.GIS.WebAPI.UI.Classes.UrbanModelGLTFNodeConverter.Convert(DiGi.Analytical.Urban.Classes.UrbanModel,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances, or null if the model is empty\.