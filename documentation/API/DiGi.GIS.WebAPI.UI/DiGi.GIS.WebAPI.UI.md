#### [DiGi\.GIS\.WebAPI\.UI](index.md 'index')

## DiGi\.GIS\.WebAPI\.UI Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Convert'></a>

## Convert Class

```csharp
public static class Convert
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Convert
### Methods

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Classes.BuildingModel,double)'></a>

## Convert\.ToGLTF\_GLTFNodes\(this BuildingModel, double\) Method

Converts all components of the specified [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') \(walls, roofs, floors and other components with surface geometry\) into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances\.

```csharp
public static System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? ToGLTF_GLTFNodes(this DiGi.Analytical.Building.Classes.BuildingModel? buildingModel, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Classes.BuildingModel,double).buildingModel'></a>

`buildingModel` [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')

The [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') to be converted\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Classes.BuildingModel,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances for all convertible components, or null if the building model is null or has no components\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Interfaces.IComponent,double)'></a>

## Convert\.ToGLTF\_GLTFNodes\(this IComponent, double\) Method

Converts the specified building [DiGi\.Analytical\.Building\.Interfaces\.IComponent](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.interfaces.icomponent 'DiGi\.Analytical\.Building\.Interfaces\.IComponent') \(for example a wall or a roof\) into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances using its surface geometry and default component styling\.

```csharp
public static System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? ToGLTF_GLTFNodes(this DiGi.Analytical.Building.Interfaces.IComponent? component, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Interfaces.IComponent,double).component'></a>

`component` [DiGi\.Analytical\.Building\.Interfaces\.IComponent](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.interfaces.icomponent 'DiGi\.Analytical\.Building\.Interfaces\.IComponent')

The building component to be converted\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Building.Interfaces.IComponent,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list with a single [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') representing the component, or null if the component has no supported surface geometry\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Urban.Classes.UrbanModel,double)'></a>

## Convert\.ToGLTF\_GLTFNodes\(this UrbanModel, double\) Method

Converts the specified [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances by converting all contained [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') instances\.

```csharp
public static System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? ToGLTF_GLTFNodes(this DiGi.Analytical.Urban.Classes.UrbanModel? urbanModel, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Urban.Classes.UrbanModel,double).urbanModel'></a>

`urbanModel` [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel')

The [DiGi\.Analytical\.Urban\.Classes\.UrbanModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.urban.classes.urbanmodel 'DiGi\.Analytical\.Urban\.Classes\.UrbanModel') to be converted\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.Analytical.Urban.Classes.UrbanModel,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances for all contained building models, or null if the urban model is null or empty\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.GIS.Classes.Building2D,double,double)'></a>

## Convert\.ToGLTF\_GLTFNodes\(this Building2D, double, double\) Method

Converts the specified [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances by extruding its 2D polygonal footprint by the number of storeys multiplied by the storey height\.

```csharp
public static System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? ToGLTF_GLTFNodes(this DiGi.GIS.Classes.Building2D? building2D, double storeyHeight=3.0, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.GIS.Classes.Building2D,double,double).building2D'></a>

`building2D` [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')

The [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') to be converted\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.GIS.Classes.Building2D,double,double).storeyHeight'></a>

`storeyHeight` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The height of a single storey in meters used for the extrusion\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisDiGi.GIS.Classes.Building2D,double,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list with a single [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') representing the extruded building, or null if the building or its geometry is null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,double,double)'></a>

## Convert\.ToGLTF\_GLTFNodes\(this IEnumerable\<Building2D\>, double, double\) Method

Converts the specified [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') collection into [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances by extruding each 2D polygonal footprint by its number of storeys multiplied by the storey height\.

```csharp
public static System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode>? ToGLTF_GLTFNodes(this System.Collections.Generic.IEnumerable<DiGi.GIS.Classes.Building2D>? building2Ds, double storeyHeight=3.0, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,double,double).building2Ds'></a>

`building2Ds` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') collection to be converted\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,double,double).storeyHeight'></a>

`storeyHeight` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The height of a single storey in meters used for the extrusions\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,double,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances for all convertible buildings, or null if [building2Ds](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Convert.ToGLTF_GLTFNodes(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,double,double).building2Ds 'DiGi\.GIS\.WebAPI\.UI\.Convert\.ToGLTF\_GLTFNodes\(this System\.Collections\.Generic\.IEnumerable\<DiGi\.GIS\.Classes\.Building2D\>, double, double\)\.building2Ds') is null\.

<a name='DiGi.GIS.WebAPI.UI.Create'></a>

## Create Class

```csharp
public static class Create
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Create
### Methods

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFScene(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,string,double,double)'></a>

## Create\.GLTFScene\(this IEnumerable\<Building2D\>, string, double, double\) Method

Creates a [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') from the specified [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') collection by extruding each footprint by its storeys and translating all geometry to a local origin \(0, 0, 0\)\.

The world offset removed from the geometry is stored in [DiGi\.GLTF\.Classes\.GLTFScene\.ReferencePoint](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene.referencepoint 'DiGi\.GLTF\.Classes\.GLTFScene\.ReferencePoint') so the camera can automatically frame the whole area. Default lighting and an automatically framing camera are added.

```csharp
public static DiGi.GLTF.Classes.GLTFScene? GLTFScene(this System.Collections.Generic.IEnumerable<DiGi.GIS.Classes.Building2D>? building2Ds, string? name=null, double storeyHeight=3.0, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFScene(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,string,double,double).building2Ds'></a>

`building2Ds` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') collection to be displayed\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFScene(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,string,double,double).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The display name of the scene\.

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFScene(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,string,double,double).storeyHeight'></a>

`storeyHeight` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The height of a single storey in meters used for the extrusions\.

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFScene(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,string,double,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene')  
A [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') holding the converted buildings, or null if [building2Ds](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Create.GLTFScene(thisSystem.Collections.Generic.IEnumerable_DiGi.GIS.Classes.Building2D_,string,double,double).building2Ds 'DiGi\.GIS\.WebAPI\.UI\.Create\.GLTFScene\(this System\.Collections\.Generic\.IEnumerable\<DiGi\.GIS\.Classes\.Building2D\>, string, double, double\)\.building2Ds') is null or contains no convertible buildings\.

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFSceneViewModel(thisDiGi.GLTF.Classes.GLTFScene,string)'></a>

## Create\.GLTFSceneViewModel\(this GLTFScene, string\) Method

Creates a [GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel') for the 3D viewer from the specified [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') by serializing the scene to JSON and exporting it as a base64 encoded binary glTF \(\.glb\) payload\.

```csharp
public static DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel? GLTFSceneViewModel(this DiGi.GLTF.Classes.GLTFScene? gLTFScene, string? title=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFSceneViewModel(thisDiGi.GLTF.Classes.GLTFScene,string).gLTFScene'></a>

`gLTFScene` [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene')

The [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') to be rendered\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.GLTFSceneViewModel(thisDiGi.GLTF.Classes.GLTFScene,string).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\. If this value is null, the scene name is used\.

#### Returns
[GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel')  
A [GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel') ready to be passed to the glTF scene view, or null if the scene is null or could not be exported\.

<a name='DiGi.GIS.WebAPI.UI.Modify'></a>

## Modify Class

```csharp
public static class Modify
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Modify
### Methods

<a name='DiGi.GIS.WebAPI.UI.Modify.Reduce(thisSystem.Collections.Generic.List_DiGi.Geometry.Planar.Classes.Point2D_,System.Nullable_double_,int)'></a>

## Modify\.Reduce\(this List\<Point2D\>, Nullable\<double\>, int\) Method

Reduces the number of points in a list using a sampling factor, ensuring a minimum count is maintained\.

```csharp
public static void Reduce(this System.Collections.Generic.List<DiGi.Geometry.Planar.Classes.Point2D>? point2Ds, System.Nullable<double> factor, int minCount=100);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Modify.Reduce(thisSystem.Collections.Generic.List_DiGi.Geometry.Planar.Classes.Point2D_,System.Nullable_double_,int).point2Ds'></a>

`point2Ds` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.Geometry\.Planar\.Classes\.Point2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.point2d 'DiGi\.Geometry\.Planar\.Classes\.Point2D')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

The list of [DiGi\.Geometry\.Planar\.Classes\.Point2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.point2d 'DiGi\.Geometry\.Planar\.Classes\.Point2D') objects to be reduced\.

<a name='DiGi.GIS.WebAPI.UI.Modify.Reduce(thisSystem.Collections.Generic.List_DiGi.Geometry.Planar.Classes.Point2D_,System.Nullable_double_,int).factor'></a>

`factor` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The reduction factor used to determine the target number of points\. A value between 0 and 1\. If null or NaN, no reduction is performed\.

<a name='DiGi.GIS.WebAPI.UI.Modify.Reduce(thisSystem.Collections.Generic.List_DiGi.Geometry.Planar.Classes.Point2D_,System.Nullable_double_,int).minCount'></a>

`minCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The minimum number of points that should remain in the list after reduction\. Defaults to 100\.

<a name='DiGi.GIS.WebAPI.UI.Query'></a>

## Query Class

```csharp
public static class Query
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Query
### Methods

<a name='DiGi.GIS.WebAPI.UI.Query.Color(thisDiGi.Core.Interfaces.ISerializableObject)'></a>

## Query\.Color\(this ISerializableObject\) Method

Gets the default display [DiGi\.Core\.Classes\.Color](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.color 'DiGi\.Core\.Classes\.Color') for the specified domain object\.

```csharp
public static DiGi.Core.Classes.Color? Color(this DiGi.Core.Interfaces.ISerializableObject? serializableObject);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Query.Color(thisDiGi.Core.Interfaces.ISerializableObject).serializableObject'></a>

`serializableObject` [DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')

The domain object to be styled\. This value can be null\.

#### Returns
[DiGi\.Core\.Classes\.Color](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.color 'DiGi\.Core\.Classes\.Color')  
A [DiGi\.Core\.Classes\.Color](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.color 'DiGi\.Core\.Classes\.Color') representing the default styling of the object, or null if no default styling is defined\.