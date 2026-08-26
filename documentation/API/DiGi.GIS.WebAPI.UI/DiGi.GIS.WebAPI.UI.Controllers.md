#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.Controllers Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController'></a>

## AdministrativeAreal2DController Class

Provides the pages and partial views of the administrative area feature\.

The data itself is owned by the GIS Web API (gis/administrativeareal2D); this controller only reads it and renders it, so the query rules stay owned by that service and cannot drift here.

```csharp
public class AdministrativeAreal2DController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → AdministrativeAreal2DController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.AdministrativeAreal2DController(System.Net.Http.IHttpClientFactory)'></a>

## AdministrativeAreal2DController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [AdministrativeAreal2DController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.AdministrativeAreal2DController') class\.

```csharp
public AdministrativeAreal2DController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.AdministrativeAreal2DController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create HTTP clients\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencePathsByNameAsync(string,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencePathsByNameAsync\(string, CancellationToken\) Method

Searches for administrative area reference paths by name\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencePathsByNameAsync(string text, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencePathsByNameAsync(string,System.Threading.CancellationToken).text'></a>

`text` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text to search for within the name column\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencePathsByNameAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation, containing the result of the search\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Nullable_int_,System.Nullable_bool_,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync\(Nullable\<AdministrativeArealType\>, Nullable\<int\>, Nullable\<bool\>, CancellationToken\) Method

Retrieves administrative 2D area references based on the specified administrative areal type, parent identifier, and unique code filter\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(System.Nullable<DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType> administrativeArealType, System.Nullable<int> parentId, System.Nullable<bool> uniqueCode, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Nullable_int_,System.Nullable_bool_,System.Threading.CancellationToken).administrativeArealType'></a>

`administrativeArealType` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The type of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Nullable_int_,System.Nullable_bool_,System.Threading.CancellationToken).parentId'></a>

`parentId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier of the parent administrative area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Nullable_int_,System.Nullable_bool_,System.Threading.CancellationToken).uniqueCode'></a>

`uniqueCode` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional flag indicating whether to filter by unique code\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Nullable_int_,System.Nullable_bool_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByCodeAsync(string,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencesByCodeAsync\(string, CancellationToken\) Method

Retrieves administrative areal 2D references by the specified code asynchronously\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencesByCodeAsync(string code, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByCodeAsync(string,System.Threading.CancellationToken).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The code used to retrieve the administrative areal 2D references\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByCodeAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByIdAsync(int,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencesByIdAsync\(int, CancellationToken\) Method

Retrieves the administrative areal 2D references by their identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencesByIdAsync(int id, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByIdAsync(int,System.Threading.CancellationToken).id'></a>

`id` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative areal 2D reference\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByIdAsync(int,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemByCodeAsync(string,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetItemByCodeAsync\(string, CancellationToken\) Method

Retrieves an administrative areal 2D item by its specified code\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByCodeAsync(string code, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemByCodeAsync(string,System.Threading.CancellationToken).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The code of the item to retrieve\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemByCodeAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemsByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetItemsByAdministrativeArealTypeAsync\(Nullable\<AdministrativeArealType\>, CancellationToken\) Method

Retrieves items filtered by the specified administrative area type\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemsByAdministrativeArealTypeAsync(System.Nullable<DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType> administrativeArealType, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemsByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Threading.CancellationToken).administrativeArealType'></a>

`administrativeArealType` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The administrative area type \(e\.g\., country, voivodeship, county, municipality\) to filter by\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemsByAdministrativeArealTypeAsync(System.Nullable_DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result containing the items or an error response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## AdministrativeAreal2DController\.GetPolygonsByIdAsync\(int, Nullable\<double\>, Nullable\<int\>, CancellationToken\) Method

Retrieves the outlines of an administrative area, reduced for drawing as an overview map\.

An area whose territory is disconnected is stored as one row per polygon part, so an area identified by a code is drawn from every row sharing that code. An area addressed directly (a municipality or a subdivision) is drawn from its own row alone.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetPolygonsByIdAsync(int id, System.Nullable<double> reductionFactor=null, System.Nullable<int> minCount=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).id'></a>

`id` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).reductionFactor'></a>

`reductionFactor` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional reduction factor used to simplify the geometry of the retrieved polygons\. When omitted, a factor matching the size of the area is applied\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).minCount'></a>

`minCount` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional fewest points a reduced outline may keep\. When omitted, a count matching the size of the area is applied\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') carrying one flat coordinate list per outline\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.MinimumPointCount(DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType)'></a>

## AdministrativeAreal2DController\.MinimumPointCount\(AdministrativeArealType\) Method

Gives the fewest points an outline of the given kind of area is allowed to keep\.

```csharp
private static int MinimumPointCount(DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType administrativeArealType);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.MinimumPointCount(DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType).administrativeArealType'></a>

`administrativeArealType` [DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')

The kind of administrative area\.

#### Returns
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The fewest points to keep\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.ReductionFactor(DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType)'></a>

## AdministrativeAreal2DController\.ReductionFactor\(AdministrativeArealType\) Method

Gives the reduction factor an outline of the given kind of area is simplified with\.

```csharp
private static double ReductionFactor(DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType administrativeArealType);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.ReductionFactor(DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType).administrativeArealType'></a>

`administrativeArealType` [DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')

The kind of administrative area\.

#### Returns
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
The reduction factor\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.Start()'></a>

## AdministrativeAreal2DController\.Start\(\) Method

Starts the Administrative Areal 2D view\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result that renders the start view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController'></a>

## Building2DController Class

Provides the pages and partial views of the building feature\.

The data itself is owned by the GIS Web API (gis/building2D); this controller only reads it and renders it.

```csharp
public class Building2DController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → Building2DController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.Building2DController(System.Net.Http.IHttpClientFactory)'></a>

## Building2DController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [Building2DController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController') class\.

```csharp
public Building2DController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.Building2DController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') instances\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.CountyIdAsync(System.Net.Http.HttpClient,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## Building2DController\.CountyIdAsync\(HttpClient, Nullable\<double\>, Nullable\<double\>, CancellationToken\) Method

Asynchronously resolves which county a plan position falls in\.

Used only as a fallback: the 3D viewer knows a building by its reference and its centroid, and the reference alone does not say which county partition holds it.

```csharp
private static System.Threading.Tasks.Task<System.Nullable<int>> CountyIdAsync(System.Net.Http.HttpClient httpClient, System.Nullable<double> x, System.Nullable<double> y, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.CountyIdAsync(System.Net.Http.HttpClient,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

The HTTP client used for the requests\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.CountyIdAsync(System.Net.Http.HttpClient,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).x'></a>

`x` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The X coordinate, in PL\-1992 \(EPSG:2180\) metres\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.CountyIdAsync(System.Net.Http.HttpClient,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).y'></a>

`y` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The Y coordinate, in PL\-1992 \(EPSG:2180\) metres\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.CountyIdAsync(System.Net.Http.HttpClient,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The identifier of the county, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when it cannot be resolved\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int,System.Threading.CancellationToken)'></a>

## Building2DController\.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync\(int, CancellationToken\) Method

Asynchronously retrieves building 2D references associated with the specified administrative areal 2D identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int administrativeAreal2DId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int,System.Threading.CancellationToken).administrativeAreal2DId'></a>

`administrativeAreal2DId` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The administrative areal 2D identifier to filter by\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## Building2DController\.GetDetailsByReferenceAsync\(string, Nullable\<int\>, Nullable\<double\>, Nullable\<double\>, CancellationToken\) Method

Renders the standalone building details page for the specified building reference \(used e\.g\. by the "Show" button of the 3D viewer "Details" panel\)\.

The partial view _Building2DView cannot be rendered on its own: it depends on the scripts, styles and AJAX loading logic of the references master-detail layout. The building context is therefore injected into `Building2DDetailsView`, which renders only the details side and loads the partial through the same AJAX pipeline.

The building data is partitioned per county, so the by-reference lookup requires a county identifier. When it is not provided, it is resolved from the optional [x](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).x 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.x')/[y](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).y 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.y') point (e.g. the building centroid known to the 3D viewer).

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetDetailsByReferenceAsync(string? reference, System.Nullable<int> countyId=null, System.Nullable<double> x=null, System.Nullable<double> y=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).x'></a>

`x` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional X coordinate of a point inside the building used to resolve the county when [countyId](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).countyId 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.countyId') is not provided\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).y'></a>

`y` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional Y coordinate of a point inside the building used to resolve the county when [countyId](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).countyId 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.countyId') is not provided\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') rendering the building details page\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## Building2DController\.GetItemByIdAsync\(long, Nullable\<int\>, CancellationToken\) Method

Asynchronously retrieves a building 2D item by its unique identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByIdAsync(long id, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the item to retrieve\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the item\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation, containing the [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## Building2DController\.GetPolygonByIdAsync\(long, Nullable\<int\>, Nullable\<double\>, Nullable\<int\>, CancellationToken\) Method

Retrieves the outline of a building footprint, reduced for drawing\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetPolygonByIdAsync(long id, System.Nullable<int> countyId, System.Nullable<double> reductionFactor=null, System.Nullable<int> minCount=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional county identifier used to filter the request\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).reductionFactor'></a>

`reductionFactor` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional reduction factor for simplifying the polygon geometry\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).minCount'></a>

`minCount` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional fewest points the reduced outline may keep\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') carrying the outline as a space separated coordinate list\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.Start()'></a>

## Building2DController\.Start\(\) Method

Initializes and returns the start view for the 2D building interface\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result that renders the starting view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController'></a>

## BuildingDataController Class

Provides controller endpoints for accessing and managing building data, acting as an interface between the client and the underlying GIS building data services\.

```csharp
public class BuildingDataController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → BuildingDataController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.BuildingDataController(System.Net.Http.IHttpClientFactory)'></a>

## BuildingDataController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [BuildingDataController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.BuildingDataController') class\.

```csharp
public BuildingDataController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.BuildingDataController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create and manage [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') instances for making API requests\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## BuildingDataController\.GetTableByReferenceAsync\(string, Nullable\<int\>, CancellationToken\) Method

Asynchronously retrieves a building data table based on the specified reference and an optional county identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetTableByReferenceAsync(string reference, System.Nullable<int> countyId=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string used to look up the table\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional ID of the county associated with the request\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the HTTP response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController'></a>

## BuildingModelController Class

Provides controller endpoints for accessing analytical [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') data, acting as an interface between the client and the underlying GIS building data services\.

```csharp
public class BuildingModelController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → BuildingModelController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.BuildingModelController(System.Net.Http.IHttpClientFactory)'></a>

## BuildingModelController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [BuildingModelController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.BuildingModelController') class\.

```csharp
public BuildingModelController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.BuildingModelController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create and manage [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') instances for making API requests\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.AddTerrainAsync(System.Collections.Generic.List_DiGi.GLTF.Classes.GLTFNode_,System.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,System.Threading.CancellationToken)'></a>

## BuildingModelController\.AddTerrainAsync\(List\<GLTFNode\>, HttpClient, Circle2D, IEnumerable\<BuildingModel\>, CancellationToken\) Method

Adds the ground surface around the given circular area to the nodes of a scene, with the outlines of the buildings of the scene cut out of it\.

The surface is optional: no stored elevation points, an undeployed or unreachable terrain service and a timeout all leave the scene exactly as it was, so a building scene never depends on terrain being there.

```csharp
private static System.Threading.Tasks.Task AddTerrainAsync(System.Collections.Generic.List<DiGi.GLTF.Classes.GLTFNode> gLTFNodes, System.Net.Http.HttpClient httpClient, DiGi.Geometry.Planar.Classes.Circle2D? circle2D, System.Collections.Generic.IEnumerable<DiGi.Analytical.Building.Classes.BuildingModel>? buildingModels, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.AddTerrainAsync(System.Collections.Generic.List_DiGi.GLTF.Classes.GLTFNode_,System.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,System.Threading.CancellationToken).gLTFNodes'></a>

`gLTFNodes` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

The nodes of the scene being built\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.AddTerrainAsync(System.Collections.Generic.List_DiGi.GLTF.Classes.GLTFNode_,System.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,System.Threading.CancellationToken).httpClient'></a>

`httpClient` [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient')

The HTTP client used for the request\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.AddTerrainAsync(System.Collections.Generic.List_DiGi.GLTF.Classes.GLTFNode_,System.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,System.Threading.CancellationToken).circle2D'></a>

`circle2D` [DiGi\.Geometry\.Planar\.Classes\.Circle2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.circle2d 'DiGi\.Geometry\.Planar\.Classes\.Circle2D')

The circular area of ground to show, in PL\-1992 \(EPSG:2180\) metres\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.AddTerrainAsync(System.Collections.Generic.List_DiGi.GLTF.Classes.GLTFNode_,System.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,System.Threading.CancellationToken).buildingModels'></a>

`buildingModels` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The buildings of the scene, whose outlines are cut out of the ground so it does not run through their interiors \(see [TerrainGLTFNode\(this GLTFNode, IEnumerable&lt;BuildingModel&gt;, Circle2D, double, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Create.TerrainGLTFNode(thisDiGi.GLTF.Classes.GLTFNode,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,DiGi.Geometry.Planar.Classes.Circle2D,double,double) 'DiGi\.GIS\.WebAPI\.UI\.Create\.TerrainGLTFNode\(this DiGi\.GLTF\.Classes\.GLTFNode, System\.Collections\.Generic\.IEnumerable\<DiGi\.Analytical\.Building\.Classes\.BuildingModel\>, DiGi\.Geometry\.Planar\.Classes\.Circle2D, double, double\)')\)\. This value can be null\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.AddTerrainAsync(System.Collections.Generic.List_DiGi.GLTF.Classes.GLTFNode_,System.Net.Http.HttpClient,DiGi.Geometry.Planar.Classes.Circle2D,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')  
A [System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task') representing the asynchronous operation\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## BuildingModelController\.GetBuildingModelByIdAsync\(long, Nullable\<int\>, CancellationToken\) Method

Renders the 3D viewer page for a single building\. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuildingModelByIdAsync(long id, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') rendering the glTF scene view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Threading.CancellationToken)'></a>

## BuildingModelController\.GetBuildingsGLBByRadiusAsync\(double, double, double, CancellationToken\) Method

Asynchronously retrieves all building models within the specified circular area from the PostgreSQL database via the GIS Web API, converts each [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') into a batched [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') with buildings selectable as whole envelopes and streams it as a binary glTF \(\.glb\) payload\.

The search is purely spatial: the area may span multiple counties, so no county identifier is required.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuildingsGLBByRadiusAsync(double centerX, double centerY, double radius, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Threading.CancellationToken).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Threading.CancellationToken).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Threading.CancellationToken).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the search circle in meters\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## BuildingModelController\.GetGLBBuildingModelByIdAsync\(long, Nullable\<int\>, CancellationToken\) Method

Asynchronously retrieves the 3D [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') for the building with the specified unique identifier from the database \(see [BuildingModelAsync\(this HttpClient, long, Nullable&lt;int&gt;, CancellationToken\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,System.Threading.CancellationToken) 'DiGi\.GIS\.WebAPI\.UI\.Query\.BuildingModelAsync\(this System\.Net\.Http\.HttpClient, long, System\.Nullable\<int\>, System\.Threading\.CancellationToken\)')\), converts each of its components \(walls, floors and roofs\) into a separate node of a batched [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') \(translated to a local origin\) and streams it as a binary glTF \(\.glb\) payload\.

Each component carries its own identity in the scene object map, so the 3D viewer can hit-test and select individual components instead of the building as a whole.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetGLBBuildingModelByIdAsync(long id, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double,System.Threading.CancellationToken)'></a>

## BuildingModelController\.GetItemByReferenceAsync\(string, double, double, CancellationToken\) Method

Asynchronously loads a [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') from the GIS Web API by searching for the building at the specified coordinates, converts its components into separate selectable [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances and renders the 3D viewer page\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByReferenceAsync(string reference, double x, double y, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The reference of the building model\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double,System.Threading.CancellationToken).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the building centroid\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double,System.Threading.CancellationToken).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the building centroid\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') rendering the 3D glTF scene view or a not found response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double)'></a>

## BuildingModelController\.GetItemsByRadius\(double, double, double\) Method

Renders the 3D viewer page for all buildings within the specified circular area\. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint\.

The search is purely spatial: the area may span multiple counties, so no county identifier is required.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetItemsByRadius(double centerX, double centerY, double radius);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the search circle in meters\.

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') rendering the glTF scene view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.Start()'></a>

## BuildingModelController\.Start\(\) Method

Handles the HTTP GET request to the root endpoint and returns the 3D viewer landing page\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the start view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController'></a>

## CommunicationController Class

Provides the communication analysis feature: an input page for the analyzed circular area, the 3D scene view with the antenna toolbar and the calculation endpoint that fetches the analyzed area buildings and solves the radio propagation between the placed antennas in process\.

```csharp
public class CommunicationController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → CommunicationController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.CommunicationController(System.Net.Http.IHttpClientFactory)'></a>

## CommunicationController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [CommunicationController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.CommunicationController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.CommunicationController') class\.

```csharp
public CommunicationController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.CommunicationController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') instances\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.CalculateAsync(DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter,System.Threading.CancellationToken)'></a>

## CommunicationController\.CalculateAsync\(CommunicationCalculationParameter, CancellationToken\) Method

Executes the communication calculation for the antennas placed in the 3D view\.

The buildings of the analyzed area are fetched as [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') instances and converted to [DiGi\.Communication\.Classes\.ScatteringObject](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringobject 'DiGi\.Communication\.Classes\.ScatteringObject') instances, packaged together with the antennas into a [DiGi\.Communication\.Classes\.GeometricalPropagationModel](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.geometricalpropagationmodel 'DiGi\.Communication\.Classes\.GeometricalPropagationModel') and solved in process ([DiGi\.Communication\.Classes\.ScatteringSolver](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringsolver 'DiGi\.Communication\.Classes\.ScatteringSolver') + [DiGi\.Communication\.Classes\.AngularPowerDistributionSolver](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.angularpowerdistributionsolver 'DiGi\.Communication\.Classes\.AngularPowerDistributionSolver')); nothing is persisted.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> CalculateAsync(DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter? communicationCalculationParameter, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.CalculateAsync(DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter,System.Threading.CancellationToken).communicationCalculationParameter'></a>

`communicationCalculationParameter` [CommunicationCalculationParameter](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter 'DiGi\.GIS\.WebAPI\.UI\.Classes\.CommunicationCalculationParameter')

The analyzed circular area and the antennas placed by the user\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.CalculateAsync(DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the [DiGi\.Communication\.WebAPI\.Classes\.GeometricalPropagationResult](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.webapi.classes.geometricalpropagationresult 'DiGi\.Communication\.WebAPI\.Classes\.GeometricalPropagationResult') JSON grouped by delay \(ascending\): the propagation ellipsoids, the scattering polylines \(one per [DiGi\.Communication\.Classes\.ScatteringPointGroup](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringpointgroup 'DiGi\.Communication\.Classes\.ScatteringPointGroup')\) and the angular power distribution vectors, all in world coordinates\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double)'></a>

## CommunicationController\.GetBuildingsByRadius\(double, double, double\) Method

Renders the communication 3D scene view for all buildings within the specified circular area \(streamed as a binary glTF payload from the existing glb endpoint\) together with the antenna toolbar\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetBuildingsByRadius(double centerX, double centerY, double radius);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the analyzed circular area in meters\.

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') rendering the communication scene view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.Start()'></a>

## CommunicationController\.Start\(\) Method

Initializes and returns the start view of the communication analysis feature\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result that renders the starting view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController'></a>

## EPWFileController Class

Provides controller endpoints for accessing and managing EPW file data\.

```csharp
public class EPWFileController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → EPWFileController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.EPWFileController(System.Net.Http.IHttpClientFactory)'></a>

## EPWFileController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [EPWFileController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.EPWFileController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.EPWFileController') class\.

```csharp
public EPWFileController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.EPWFileController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The HTTP client factory\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double,System.Threading.CancellationToken)'></a>

## EPWFileController\.GetEPWFileAsync\(double, double, CancellationToken\) Method

Asynchronously retrieves an EPW file based on the specified coordinates\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetEPWFileAsync(double x, double y, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double,System.Threading.CancellationToken).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double,System.Threading.CancellationToken).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the HTTP response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController'></a>

## HeatTransferCoefficientController Class

Controller responsible for handling requests related to heat transfer coefficients,
providing data based on year or building reference\.

```csharp
public class HeatTransferCoefficientController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → HeatTransferCoefficientController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.HeatTransferCoefficientController(System.Net.Http.IHttpClientFactory)'></a>

## HeatTransferCoefficientController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [HeatTransferCoefficientController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.HeatTransferCoefficientController') class\.

```csharp
public HeatTransferCoefficientController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.HeatTransferCoefficientController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The HTTP client factory used to create clients for external API communication\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## HeatTransferCoefficientController\.GetRegulatedHeatTransferCoefficientsByReferenceAsync\(string, Nullable\<int\>, CancellationToken\) Method

Retrieves regulated heat transfer coefficients for a building identified by its reference,
determining the applicable year based on the building's construction data\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetRegulatedHeatTransferCoefficientsByReferenceAsync(string reference, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier for the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') containing a partial view with the coefficient data, or an error result if the request fails\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.HomeController'></a>

## HomeController Class

Provides the controller logic for handling requests to the home page of the GIS WebAPI user interface\.

```csharp
public class HomeController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → HomeController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.HomeController.HomeController()'></a>

## HomeController\(\) Constructor

Initializes a new instance of the [HomeController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.HomeController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.HomeController') class\.

The landing pages carry no data of their own, so nothing is injected here.

```csharp
public HomeController();
```
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.HomeController.About()'></a>

## HomeController\.About\(\) Method

Returns the view for the About page\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult About();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.HomeController.Start()'></a>

## HomeController\.Start\(\) Method

Returns the view for the Start page\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController'></a>

## OccupancyDataController Class

Controller responsible for handling requests related to occupancy data for 2D buildings\.

```csharp
public class OccupancyDataController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → OccupancyDataController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.OccupancyDataController(System.Net.Http.IHttpClientFactory)'></a>

## OccupancyDataController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [OccupancyDataController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.OccupancyDataController') class\.

```csharp
public OccupancyDataController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.OccupancyDataController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The HTTP client factory used to create clients for external API communication\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## OccupancyDataController\.GetBuilding2DItemByReferenceAsync\(string, Nullable\<int\>, CancellationToken\) Method

Retrieves the occupancy data and building reference for a specific 2D building item by its reference\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuilding2DItemByReferenceAsync(string reference, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional ID of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult'), which returns a partial view containing the occupancy data if successful; otherwise, a bad request or no content response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController'></a>

## OrtoDatasController Class

Controller providing endpoints for managing and retrieving orthodata and coverage factor information\.

```csharp
public class OrtoDatasController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → OrtoDatasController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.OrtoDatasController(System.Net.Http.IHttpClientFactory)'></a>

## OrtoDatasController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [OrtoDatasController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.OrtoDatasController') class\.

```csharp
public OrtoDatasController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.OrtoDatasController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The HTTP client factory used to create [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') instances\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorAsync(int,System.Threading.CancellationToken)'></a>

## OrtoDatasController\.GetEstimatedCoverageFactorAsync\(int, CancellationToken\) Method

Retrieves the estimated orthophoto coverage factor of a single administrative area\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetEstimatedCoverageFactorAsync(int administrativeAreal2DId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorAsync(int,System.Threading.CancellationToken).administrativeAreal2DId'></a>

`administrativeAreal2DId` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorAsync(int,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') with the coverage factor or error status\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable_int_,System.Threading.CancellationToken)'></a>

## OrtoDatasController\.GetEstimatedCoverageFactorsAsync\(IEnumerable\<int\>, CancellationToken\) Method

Retrieves the estimated orthophoto coverage factors of several administrative areas at once\.

The values come back in the order the identifiers were given, which is what lets the page update one progress bar per row without matching anything up.

A value is `null` where the coverage could not be measured, which the API keeps distinct from zero - a county nothing has ever been downloaded for is not a county that is nought per cent covered. Relayed on as null so the page can show it as such rather than drawing an empty bar.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable<int> administrativeAreal2DIds, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable_int_,System.Threading.CancellationToken).administrativeAreal2DIds'></a>

`administrativeAreal2DIds` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The unique identifiers of the administrative areas\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') with the list of coverage factor values or error status\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## OrtoDatasController\.GetItemByReferenceAsync\(string, Nullable\<int\>, CancellationToken\) Method

Retrieves orthodata and building 2D reference information based on a provided reference and optional county ID\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByReferenceAsync(string reference, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string for the item\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier for the county\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the partial view or error response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.SolarController'></a>

## SolarController Class

\[TEMPORARY\] Solar calculations for the 3D viewer Lighting panel, hosted locally until a
DiGi\.Solar backed endpoint is available on the central GIS Web API\. The route contract
\(solar/sundirection\) is final \- when the central endpoint exists this controller becomes a
proxy like the other controllers in this project, and the consuming frontend
\(gltf\-viewer\.js\) stays unchanged\.

```csharp
public class SolarController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → SolarController
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.SolarController.GetSunDirection(double,double,string,double)'></a>

## SolarController\.GetSunDirection\(double, double, string, double\) Method

Calculates the sun position for a world location and a local date and time\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetSunDirection(double x, double y, string? date, double hour);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.SolarController.GetSunDirection(double,double,string,double).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate in the EPSG:2180 coordinate system \[m\]\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.SolarController.GetSunDirection(double,double,string,double).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate in the EPSG:2180 coordinate system \[m\]\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.SolarController.GetSunDirection(double,double,string,double).date'></a>

`date` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The local calendar date in the yyyy\-MM\-dd format\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.SolarController.GetSunDirection(double,double,string,double).hour'></a>

`hour` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The local time of day as a decimal hour in the 0\-24 range\.

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
JSON with the true solar angles: azimuth \[deg\] \(0 = north, clockwise\) and altitude \[deg\] above the horizon \(negative at night\)\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController'></a>

## TerrainController Class

Provides the terrain feature: the ground surface of an area as a [DiGi\.Geometry\.Spatial\.Classes\.Mesh3D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.spatial.classes.mesh3d 'DiGi\.Geometry\.Spatial\.Classes\.Mesh3D'), as a binary glTF payload and as a 3D viewer page\.

The surface itself is reconstructed by the GIS Web API terrain endpoints (gis/terrain) from the elevation points stored per county; this controller only relays them, so the query limits (maximum radius, mesh edge length, tolerance defaults) stay owned by that service and cannot drift here.

Every surface returned here is a two-and-a-half dimensional height field: exactly one elevation per plan position. It models ground, and cannot express a vertical face, an overhang or a canopy.

```csharp
public class TerrainController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → TerrainController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.TerrainController(System.Net.Http.IHttpClientFactory)'></a>

## TerrainController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [TerrainController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController') class\.

```csharp
public TerrainController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.TerrainController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create [System\.Net\.Http\.HttpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient 'System\.Net\.Http\.HttpClient') instances\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.BoundingBoxName(double,double,double,double)'></a>

## TerrainController\.BoundingBoxName\(double, double, double, double\) Method

Builds the name of a rectangular terrain area, used as the scene name and as the viewer page title\.

```csharp
private static string BoundingBoxName(double x_1, double y_1, double x_2, double y_2);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.BoundingBoxName(double,double,double,double).x_1'></a>

`x_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.BoundingBoxName(double,double,double,double).y_1'></a>

`y_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.BoundingBoxName(double,double,double,double).x_2'></a>

`x_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.BoundingBoxName(double,double,double,double).y_2'></a>

`y_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The name of the area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_)'></a>

## TerrainController\.Circle2D\(double, double, Nullable\<double\>, Nullable\<double\>\) Method

Builds the circular area a request asked for, resolving the radius from either the radius or the diameter\.

An area that cannot be served (a coordinate or a radius that is not a usable number) is built all the same and rejected afterwards by [TerrainRequestUri\(this Circle2D, Nullable&lt;double&gt;\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainRequestUri(thisDiGi.Geometry.Planar.Classes.Circle2D,System.Nullable_double_) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainRequestUri\(this DiGi\.Geometry\.Planar\.Classes\.Circle2D, System\.Nullable\<double\>\)'), so what a terrain request may ask for is decided in one place.

```csharp
private static DiGi.Geometry.Planar.Classes.Circle2D Circle2D(double x, double y, System.Nullable<double> radius, System.Nullable<double> diameter);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_).radius'></a>

`radius` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search radius in metres\. Optional when [diameter](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_).diameter 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.Circle2D\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.diameter') is supplied\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_).diameter'></a>

`diameter` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search diameter in metres, used only when [radius](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Circle2D(double,double,System.Nullable_double_,System.Nullable_double_).radius 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.Circle2D\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.radius') is absent\.

#### Returns
[DiGi\.Geometry\.Planar\.Classes\.Circle2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.circle2d 'DiGi\.Geometry\.Planar\.Classes\.Circle2D')  
The requested area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_)'></a>

## TerrainController\.CircleName\(double, double, Nullable\<double\>, Nullable\<double\>\) Method

Builds the name of a circular terrain area, used as the scene name and as the viewer page title\.

```csharp
private static string CircleName(double x, double y, System.Nullable<double> radius, System.Nullable<double> diameter);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_).radius'></a>

`radius` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search radius in metres\. Optional when [diameter](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_).diameter 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.CircleName\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.diameter') is supplied\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_).diameter'></a>

`diameter` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search diameter in metres, used only when [radius](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.CircleName(double,double,System.Nullable_double_,System.Nullable_double_).radius 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.CircleName\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.radius') is absent\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The name of the area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## TerrainController\.GetGLBMesh3DByBoundingBoxAsync\(double, double, double, double, Nullable\<double\>, CancellationToken\) Method

Asynchronously retrieves the terrain surface inside an axis aligned bounding box and streams it as a binary glTF \(\.glb\) payload for the 3D viewer\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetGLBMesh3DByBoundingBoxAsync(double x_1, double y_1, double x_2, double y_2, System.Nullable<double> tolerance, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).x_1'></a>

`x_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).y_1'></a>

`y_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).x_2'></a>

`x_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).y_2'></a>

`y_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\. When omitted the terrain service applies its own default\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## TerrainController\.GetGLBMesh3DByCircleAsync\(double, double, Nullable\<double\>, Nullable\<double\>, Nullable\<double\>, CancellationToken\) Method

Asynchronously retrieves the terrain surface inside a circle and streams it as a binary glTF \(\.glb\) payload for the 3D viewer\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetGLBMesh3DByCircleAsync(double x, double y, System.Nullable<double> radius, System.Nullable<double> diameter, System.Nullable<double> tolerance, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).radius'></a>

`radius` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search radius in metres\. Optional when [diameter](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).diameter 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetGLBMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.diameter') is supplied\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).diameter'></a>

`diameter` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search diameter in metres, used only when [radius](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).radius 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetGLBMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.radius') is absent\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\. When omitted the terrain service applies its own default\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetGLBMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## TerrainController\.GetMesh3DByBoundingBoxAsync\(double, double, double, double, Nullable\<double\>, CancellationToken\) Method

Asynchronously retrieves the terrain surface inside an axis aligned bounding box given by two opposite corners\.

Corner order does not matter.

A terrain payload is optional by contract: when the area holds no stored elevation points, or the terrain service cannot answer, the response is 204 rather than an error - see [TerrainJsonAsync\(this HttpClient, string, CancellationToken\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainJsonAsync\(this System\.Net\.Http\.HttpClient, string, System\.Threading\.CancellationToken\)').

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetMesh3DByBoundingBoxAsync(double x_1, double y_1, double x_2, double y_2, System.Nullable<double> tolerance, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).x_1'></a>

`x_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).y_1'></a>

`y_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).x_2'></a>

`x_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).y_2'></a>

`y_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\. When omitted the terrain service applies its own default\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByBoundingBoxAsync(double,double,double,double,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') carrying the [DiGi\.Geometry\.Spatial\.Classes\.Mesh3D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.spatial.classes.mesh3d 'DiGi\.Geometry\.Spatial\.Classes\.Mesh3D') as JSON\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## TerrainController\.GetMesh3DByCircleAsync\(double, double, Nullable\<double\>, Nullable\<double\>, Nullable\<double\>, CancellationToken\) Method

Asynchronously retrieves the terrain surface inside a circle centred on the given plan coordinate\.

Either [radius](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).radius 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.radius') or [diameter](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).diameter 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.diameter') must be supplied; [radius](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).radius 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.radius') wins when both are.

A terrain payload is optional by contract: when the area holds no stored elevation points, or the terrain service cannot answer, the response is 204 rather than an error - see [TerrainJsonAsync\(this HttpClient, string, CancellationToken\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TerrainJsonAsync(thisSystem.Net.Http.HttpClient,string,System.Threading.CancellationToken) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TerrainJsonAsync\(this System\.Net\.Http\.HttpClient, string, System\.Threading\.CancellationToken\)').

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetMesh3DByCircleAsync(double x, double y, System.Nullable<double> radius, System.Nullable<double> diameter, System.Nullable<double> tolerance, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the centre, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).radius'></a>

`radius` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search radius in metres\. Optional when [diameter](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).diameter 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.diameter') is supplied\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).diameter'></a>

`diameter` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The search diameter in metres, used only when [radius](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).radius 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.TerrainController\.GetMesh3DByCircleAsync\(double, double, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Nullable\<double\>, System\.Threading\.CancellationToken\)\.radius') is absent\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).tolerance'></a>

`tolerance` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional tolerance for the spatial query, in metres\. When omitted the terrain service applies its own default\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetMesh3DByCircleAsync(double,double,System.Nullable_double_,System.Nullable_double_,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') carrying the [DiGi\.Geometry\.Spatial\.Classes\.Mesh3D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.spatial.classes.mesh3d 'DiGi\.Geometry\.Spatial\.Classes\.Mesh3D') as JSON\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByBoundingBox(double,double,double,double)'></a>

## TerrainController\.GetTerrainByBoundingBox\(double, double, double, double\) Method

Renders the 3D viewer page for the terrain inside an axis aligned bounding box\. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetTerrainByBoundingBox(double x_1, double y_1, double x_2, double y_2);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByBoundingBox(double,double,double,double).x_1'></a>

`x_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByBoundingBox(double,double,double,double).y_1'></a>

`y_1` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the first corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByBoundingBox(double,double,double,double).x_2'></a>

`x_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByBoundingBox(double,double,double,double).y_2'></a>

`y_2` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the second corner, in PL\-1992 \(EPSG:2180\) metres\.

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') rendering the glTF scene view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByCircle(double,double,double)'></a>

## TerrainController\.GetTerrainByCircle\(double, double, double\) Method

Renders the 3D viewer page for the terrain inside a circle\. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetTerrainByCircle(double centerX, double centerY, double radius);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByCircle(double,double,double).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the centre of the requested area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByCircle(double,double,double).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the centre of the requested area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GetTerrainByCircle(double,double,double).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the requested area in metres\.

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') rendering the glTF scene view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GLBResultAsync(string,string,DiGi.Geometry.Spatial.Classes.Point3D,System.Threading.CancellationToken)'></a>

## TerrainController\.GLBResultAsync\(string, string, Point3D, CancellationToken\) Method

Converts the terrain surface behind the given request into a batched [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') and renders it as a binary glTF \(\.glb\) response body\.

Missing terrain is answered with 204 at every step. The viewer treats an empty payload as "nothing to draw" and keeps the rest of the page working, so a request for an area the terrain store does not cover yet degrades instead of failing.

```csharp
private System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GLBResultAsync(string requestUri, string name, DiGi.Geometry.Spatial.Classes.Point3D referencePoint, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GLBResultAsync(string,string,DiGi.Geometry.Spatial.Classes.Point3D,System.Threading.CancellationToken).requestUri'></a>

`requestUri` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The terrain service URL to read the surface from\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GLBResultAsync(string,string,DiGi.Geometry.Spatial.Classes.Point3D,System.Threading.CancellationToken).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name given to the scene and to its single node\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GLBResultAsync(string,string,DiGi.Geometry.Spatial.Classes.Point3D,System.Threading.CancellationToken).referencePoint'></a>

`referencePoint` [DiGi\.Geometry\.Spatial\.Classes\.Point3D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.spatial.classes.point3d 'DiGi\.Geometry\.Spatial\.Classes\.Point3D')

The world point the scene is translated to a local origin around\. The centre of the requested area, matching what the building scenes use, so a terrain scene and a building scene of the same area share one local origin\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.GLBResultAsync(string,string,DiGi.Geometry.Spatial.Classes.Point3D,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file, or a no content status\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.TerrainController.Start()'></a>

## TerrainController\.Start\(\) Method

Initializes and returns the start view of the terrain feature\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result that renders the starting view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController'></a>

## YearBuiltDataController Class

Provides API endpoints for retrieving and managing year built data\.

```csharp
public class YearBuiltDataController : Microsoft.AspNetCore.Mvc.Controller
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.AspNetCore\.Mvc\.ControllerBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase 'Microsoft\.AspNetCore\.Mvc\.ControllerBase') → [Microsoft\.AspNetCore\.Mvc\.Controller](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controller 'Microsoft\.AspNetCore\.Mvc\.Controller') → YearBuiltDataController
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.YearBuiltDataController(System.Net.Http.IHttpClientFactory)'></a>

## YearBuiltDataController\(IHttpClientFactory\) Constructor

Initializes a new instance of the [YearBuiltDataController](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.YearBuiltDataController') class\.

```csharp
public YearBuiltDataController(System.Net.Http.IHttpClientFactory httpClientFactory);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.YearBuiltDataController(System.Net.Http.IHttpClientFactory).httpClientFactory'></a>

`httpClientFactory` [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory')

The [System\.Net\.Http\.IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.ihttpclientfactory 'System\.Net\.Http\.IHttpClientFactory') used to create HTTP clients\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken)'></a>

## YearBuiltDataController\.GetItemByReferenceAsync\(string, Nullable\<int\>, CancellationToken\) Method

Retrieves a year built data item based on the specified reference and optional county identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByReferenceAsync(string reference, System.Nullable<int> countyId, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference of the item to retrieve\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier of the county associated with the item\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation, containing an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result\.