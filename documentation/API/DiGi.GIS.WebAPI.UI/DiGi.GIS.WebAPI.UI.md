#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Convert'></a>

## Convert Class

```csharp
public static class Convert
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Convert
### Methods

<a name='DiGi.GIS.WebAPI.UI.Convert.ToCommunication(thisDiGi.Analytical.Building.Classes.BuildingModel,double)'></a>

## Convert\.ToCommunication\(this BuildingModel, double\) Method

Converts the specified [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') into a list of [DiGi\.Communication\.Classes\.ScatteringObject](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringobject 'DiGi\.Communication\.Classes\.ScatteringObject') instances \(one per building component\) by gathering and triangulating the surface of each component\.

```csharp
public static System.Collections.Generic.List<DiGi.Communication.Classes.ScatteringObject>? ToCommunication(this DiGi.Analytical.Building.Classes.BuildingModel? buidlingModel, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Convert.ToCommunication(thisDiGi.Analytical.Building.Classes.BuildingModel,double).buidlingModel'></a>

`buidlingModel` [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')

The [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') to be converted\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Convert.ToCommunication(thisDiGi.Analytical.Building.Classes.BuildingModel,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during triangulation\.

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.Communication\.Classes\.ScatteringObject](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringobject 'DiGi\.Communication\.Classes\.ScatteringObject')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [DiGi\.Communication\.Classes\.ScatteringObject](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringobject 'DiGi\.Communication\.Classes\.ScatteringObject') instances \(one per component\), or null if the building model or its components are null\.

<a name='DiGi.GIS.WebAPI.UI.Create'></a>

## Create Class

```csharp
public static class Create
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Create
### Methods

<a name='DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double)'></a>

## Create\.BuildingModelAsync\(this HttpClient, long, Nullable\<int\>, double, double\) Method

Asynchronously creates a [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') for the building with the specified unique identifier by fetching the [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D') from the GIS Web API and extruding its 2D footprint storey by storey into individual components \(walls, floors and roofs\)\.

```csharp
public static System.Threading.Tasks.Task<DiGi.Analytical.Building.Classes.BuildingModel?> BuildingModelAsync(this System.Net.Http.HttpClient? httpClient, long id, System.Nullable<int> countyId=null, double storeyHeight=3.0, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

The [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') used to call the GIS Web API\.

<a name='DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the building\.

<a name='DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double).storeyHeight'></a>

`storeyHeight` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The height of a single storey in meters used for the extrusion\.

<a name='DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance used during component creation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the created [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel'), or null if the building could not be fetched or converted\.

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

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,string,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## Create\.TerrainGLTFNodeAsync\(this HttpClient, Circle2D, string, Nullable\<double\>, CancellationToken\) Method

Asynchronously creates the [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') holding the ground surface of a circular area, read from the GIS Web API terrain service\.

```csharp
public static System.Threading.Tasks.Task<DiGi.GLTF.Classes.GLTFNode?> TerrainGLTFNodeAsync(this System.Net.Http.HttpClient? httpClient, DiGi.Geometry.Planar.Classes.Circle2D? circle2D, string? name=null, System.Nullable<double> tolerance=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,string,System.Nullable_double_,System.Threading.CancellationToken).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

The HTTP client used for the request\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,string,System.Nullable_double_,System.Threading.CancellationToken).circle2D'></a>

`circle2D` [DiGi\.Geometry\.Planar\.Classes\.Circle2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.circle2d 'DiGi\.Geometry\.Planar\.Classes\.Circle2D')

The area to show the terrain surface of, in PL\-1992 \(EPSG:2180\) metres\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,string,System.Nullable_double_,System.Threading.CancellationToken).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name given to the node\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,string,System.Nullable_double_,System.Threading.CancellationToken).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\. When omitted the terrain service applies its own default\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,string,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The terrain node, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when the area has no surface to show\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,string,string,System.Threading.CancellationToken)'></a>

## Create\.TerrainGLTFNodeAsync\(this HttpClient, string, string, CancellationToken\) Method

Asynchronously creates the [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') holding the ground surface of an area, read from the GIS Web API terrain service\.

The surface keeps the elevations it is stored with. Nothing is shifted onto another datum here, so a scene mixing terrain with other geometry only lines up when that geometry carries real elevations too - see the TERRAIN note on [TerrainEnabled](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.TerrainEnabled 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.TerrainEnabled').

[null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') is returned whenever there is no surface to show, for any reason (see [TerrainJsonAsync\(this HttpClient, string, CancellationToken\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainJsonAsync\(this System\.Net\.Http\.HttpClient, string, System\.Threading\.CancellationToken\)')). A caller adds the node when it gets one and carries on unchanged when it does not.

```csharp
public static System.Threading.Tasks.Task<DiGi.GLTF.Classes.GLTFNode?> TerrainGLTFNodeAsync(this System.Net.Http.HttpClient? httpClient, string? requestUri, string? name=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,string,string,System.Threading.CancellationToken).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

The HTTP client used for the request\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,string,string,System.Threading.CancellationToken).requestUri'></a>

`requestUri` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The terrain service URL to read the surface from, as built by [TerrainRequestUri\(this Circle2D, Nullable&lt;double&gt;\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.Circle2D,System.Nullable_double_) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainRequestUri\(this DiGi\.Geometry\.Planar\.Classes\.Circle2D, System\.Nullable\<double\>\)')\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,string,string,System.Threading.CancellationToken).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name given to the node\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNodeAsync(thisSystem.Net.Http.HttpClient,string,string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The terrain node, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when the area has no surface to show\.

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

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken)'></a>

## Query\.TerrainJsonAsync\(this HttpClient, string, CancellationToken\) Method

Asynchronously reads a GIS Web API terrain service response body\.

Terrain is optional by contract, so every way of not getting a body collapses into [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null'): the area holds no stored elevation points (404), the terrain store cannot be queried (500 - the elevation table does not exist in every environment yet), the service is unreachable, or the request timed out. A caller that only wants terrain when it exists cannot be broken by any of them, and no scene loading terrain alongside other objects loses those objects because of it. The price is that "no terrain here" and "terrain service down" are indistinguishable to the caller; the terrain service logs which one it was.

The wait is bounded by [TerrainRequestTimeout](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.TerrainRequestTimeout 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.TerrainRequestTimeout') rather than by the 100 second [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') default, so a stalled terrain query cannot hold a page request open.

```csharp
public static System.Threading.Tasks.Task<string?> TerrainJsonAsync(this System.Net.Http.HttpClient? httpClient, string? requestUri, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

The HTTP client used for the request\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken).requestUri'></a>

`requestUri` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The terrain service URL to read, as built by [TerrainRequestUri\(this Circle2D, Nullable&lt;double&gt;\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.Circle2D,System.Nullable_double_) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainRequestUri\(this DiGi\.Geometry\.Planar\.Classes\.Circle2D, System\.Nullable\<double\>\)')\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The response body, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when there is none\.

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.BoundingBox2D,System.Nullable_double_)'></a>

## Query\.TerrainRequestUri\(this BoundingBox2D, Nullable\<double\>\) Method

Builds the GIS Web API terrain service URL for an axis aligned rectangular area\.

The corners are checked only for being usable numbers: the ceiling on how large an area may be asked for belongs to the terrain service, which rejects an oversized request itself.

An omitted tolerance is left out of the URL entirely rather than sent as a value of this application's choosing, so the terrain service applies its own default.

```csharp
public static string? TerrainRequestUri(this DiGi.Geometry.Planar.Classes.BoundingBox2D? boundingBox2D, System.Nullable<double> tolerance=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.BoundingBox2D,System.Nullable_double_).boundingBox2D'></a>

`boundingBox2D` [DiGi\.Geometry\.Planar\.Classes\.BoundingBox2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.boundingbox2d 'DiGi\.Geometry\.Planar\.Classes\.BoundingBox2D')

The area to request the terrain surface for, in PL\-1992 \(EPSG:2180\) metres\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.BoundingBox2D,System.Nullable_double_).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The terrain service URL, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when the area cannot be requested\.

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.Circle2D,System.Nullable_double_)'></a>

## Query\.TerrainRequestUri\(this Circle2D, Nullable\<double\>\) Method

Builds the GIS Web API terrain service URL for a circular area\.

The radius is checked only for being a usable number: the ceiling on how large an area may be asked for belongs to the terrain service, which rejects an oversized request itself.

An omitted tolerance is left out of the URL entirely rather than sent as a value of this application's choosing, so the terrain service applies its own default.

```csharp
public static string? TerrainRequestUri(this DiGi.Geometry.Planar.Classes.Circle2D? circle2D, System.Nullable<double> tolerance=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.Circle2D,System.Nullable_double_).circle2D'></a>

`circle2D` [DiGi\.Geometry\.Planar\.Classes\.Circle2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.circle2d 'DiGi\.Geometry\.Planar\.Classes\.Circle2D')

The area to request the terrain surface for, in PL\-1992 \(EPSG:2180\) metres\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.Circle2D,System.Nullable_double_).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The terrain service URL, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when the area cannot be requested\.