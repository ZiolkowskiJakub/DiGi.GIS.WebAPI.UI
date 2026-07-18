#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.Controllers Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController'></a>

## AdministrativeAreal2DController Class

Provides API endpoints for managing and retrieving 2D administrative areal data\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencePathsByNameAsync(string)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencePathsByNameAsync\(string\) Method

Searches for administrative area reference paths by name\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencePathsByNameAsync(string text);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencePathsByNameAsync(string).text'></a>

`text` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text to search for within the name column\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation, containing the result of the search\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(string,System.Nullable_int_,System.Nullable_bool_)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync\(string, Nullable\<int\>, Nullable\<bool\>\) Method

Retrieves administrative 2D area references based on the specified administrative areal type, parent identifier, and unique code filter\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(string administrativeArealType, System.Nullable<int> parentId, System.Nullable<bool> uniqueCode);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(string,System.Nullable_int_,System.Nullable_bool_).administrativeArealType'></a>

`administrativeArealType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The type of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(string,System.Nullable_int_,System.Nullable_bool_).parentId'></a>

`parentId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier of the parent administrative area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync(string,System.Nullable_int_,System.Nullable_bool_).uniqueCode'></a>

`uniqueCode` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional flag indicating whether to filter by unique code\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByCodeAsync(string)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencesByCodeAsync\(string\) Method

Retrieves administrative areal 2D references by the specified code asynchronously\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencesByCodeAsync(string code);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByCodeAsync(string).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The code used to retrieve the administrative areal 2D references\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByIdAsync(int)'></a>

## AdministrativeAreal2DController\.GetAdministrativeAreal2DReferencesByIdAsync\(int\) Method

Retrieves the administrative areal 2D references by their identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAdministrativeAreal2DReferencesByIdAsync(int id);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetAdministrativeAreal2DReferencesByIdAsync(int).id'></a>

`id` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative areal 2D reference\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemByCodeAsync(string)'></a>

## AdministrativeAreal2DController\.GetItemByCodeAsync\(string\) Method

Retrieves an administrative areal 2D item by its specified code\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByCodeAsync(string code);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemByCodeAsync(string).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The code of the item to retrieve\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemsByAdministrativeArealTypeAsync(string)'></a>

## AdministrativeAreal2DController\.GetItemsByAdministrativeArealTypeAsync\(string\) Method

Retrieves items filtered by the specified administrative area type\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemsByAdministrativeArealTypeAsync(string administrativeArealType);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetItemsByAdministrativeArealTypeAsync(string).administrativeArealType'></a>

`administrativeArealType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The administrative area type \(e\.g\., country, voivodeship, county, municipality\) to filter by\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result containing the items or an error response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_)'></a>

## AdministrativeAreal2DController\.GetPolygonsByIdAsync\(int, Nullable\<double\>, Nullable\<int\>\) Method

Retrieves polygons associated with a specific administrative areal identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetPolygonsByIdAsync(int id, System.Nullable<double> reductionFactor=null, System.Nullable<int> minCount=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_).id'></a>

`id` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative areal\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_).reductionFactor'></a>

`reductionFactor` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional reduction factor used to simplify the geometry of the retrieved polygons\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.AdministrativeAreal2DController.GetPolygonsByIdAsync(int,System.Nullable_double_,System.Nullable_int_).minCount'></a>

`minCount` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional minimum count for filtering the results\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the result of the request\.

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

Provides API endpoints for managing and retrieving building 2D information\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int)'></a>

## Building2DController\.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync\(int\) Method

Asynchronously retrieves building 2D references associated with the specified administrative areal 2D identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int administrativeAreal2DId);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetBuilding2DReferencesByAdministrativeAreal2DIdAsync(int).administrativeAreal2DId'></a>

`administrativeAreal2DId` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The administrative areal 2D identifier to filter by\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_)'></a>

## Building2DController\.GetDetailsByReferenceAsync\(string, Nullable\<int\>, Nullable\<double\>, Nullable\<double\>\) Method

Renders the standalone building details page for the specified building reference \(used e\.g\. by the "Show" button of the 3D viewer "Details" panel\)\.

The partial view _Building2DView cannot be rendered on its own: it depends on the scripts, styles and AJAX loading logic of the references master-detail layout. The building context is therefore injected into `Building2DDetailsView`, which renders only the details side and loads the partial through the same AJAX pipeline.

The building data is partitioned per county, so the by-reference lookup requires a county identifier. When it is not provided, it is resolved from the optional [x](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).x 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.x')/[y](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).y 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.y') point (e.g. the building centroid known to the 3D viewer).

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetDetailsByReferenceAsync(string? reference, System.Nullable<int> countyId=null, System.Nullable<double> x=null, System.Nullable<double> y=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).x'></a>

`x` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional X coordinate of a point inside the building used to resolve the county when [countyId](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).countyId 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.countyId') is not provided\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).y'></a>

`y` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional Y coordinate of a point inside the building used to resolve the county when [countyId](DiGi.GIS.WebAPI.UI.Controllers.md#DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetDetailsByReferenceAsync(string,System.Nullable_int_,System.Nullable_double_,System.Nullable_double_).countyId 'DiGi\.GIS\.WebAPI\.UI\.Controllers\.Building2DController\.GetDetailsByReferenceAsync\(string, System\.Nullable\<int\>, System\.Nullable\<double\>, System\.Nullable\<double\>\)\.countyId') is not provided\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') rendering the building details page\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_)'></a>

## Building2DController\.GetItemByIdAsync\(long, Nullable\<int\>\) Method

Asynchronously retrieves a building 2D item by its unique identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByIdAsync(long id, System.Nullable<int> countyId);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the item to retrieve\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetItemByIdAsync(long,System.Nullable_int_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the item\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation, containing the [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_)'></a>

## Building2DController\.GetPolygonByIdAsync\(long, Nullable\<int\>, Nullable\<double\>, Nullable\<int\>\) Method

Retrieves a polygon by its unique identifier asynchronously\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetPolygonByIdAsync(long id, System.Nullable<int> countyId, System.Nullable<double> reductionFactor=null, System.Nullable<int> minCount=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the polygon\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional county identifier used to filter the request\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_).reductionFactor'></a>

`reductionFactor` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional reduction factor for simplifying the polygon geometry\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.Building2DController.GetPolygonByIdAsync(long,System.Nullable_int_,System.Nullable_double_,System.Nullable_int_).minCount'></a>

`minCount` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional minimum count threshold for the data retrieval\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') representing the asynchronous operation result containing the requested polygon data\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_)'></a>

## BuildingDataController\.GetTableByReferenceAsync\(string, Nullable\<int\>\) Method

Asynchronously retrieves a building data table based on the specified reference and an optional county identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetTableByReferenceAsync(string reference, System.Nullable<int> countyId=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string used to look up the table\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.GetTableByReferenceAsync(string,System.Nullable_int_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional ID of the county associated with the request\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the HTTP response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingDataController.Start()'></a>

## BuildingDataController\.Start\(\) Method

Handles the HTTP GET request to the root endpoint and returns the starting view for building data operations\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the start view\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_)'></a>

## BuildingModelController\.GetBuildingModelByIdAsync\(long, Nullable\<int\>, Nullable\<double\>\) Method

Renders the 3D viewer page for a single building\. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuildingModelByIdAsync(long id, System.Nullable<int> countyId, System.Nullable<double> storeyHeight=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_).storeyHeight'></a>

`storeyHeight` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional storey height in meters used for the extrusion\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') rendering the glTF scene view\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Nullable_double_,System.Threading.CancellationToken)'></a>

## BuildingModelController\.GetBuildingsGLBByRadiusAsync\(double, double, double, Nullable\<double\>, CancellationToken\) Method

Asynchronously retrieves all building models within the specified circular area from the PostgreSQL database via the GIS Web API, converts each [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') into a batched [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') with buildings selectable as whole envelopes and streams it as a binary glTF \(\.glb\) payload\.

The search is purely spatial: the area may span multiple counties, so no county identifier is required.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuildingsGLBByRadiusAsync(double centerX, double centerY, double radius, System.Nullable<double> storeyHeight=null, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Nullable_double_,System.Threading.CancellationToken).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Nullable_double_,System.Threading.CancellationToken).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Nullable_double_,System.Threading.CancellationToken).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the search circle in meters\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Nullable_double_,System.Threading.CancellationToken).storeyHeight'></a>

`storeyHeight` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional storey height in meters used for the extrusions\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetBuildingsGLBByRadiusAsync(double,double,double,System.Nullable_double_,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

A cancellation token that can be used by the caller to cancel the asynchronous operation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_)'></a>

## BuildingModelController\.GetGLBBuildingModelByIdAsync\(long, Nullable\<int\>, Nullable\<double\>\) Method

Asynchronously creates a [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') for the building with the specified unique identifier \(see [BuildingModelAsync\(this HttpClient, long, Nullable&lt;int&gt;, double, double\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Create.BuildingModelAsync(thisSystem.Net.Http.HttpClient,long,System.Nullable_int_,double,double) 'DiGi\.GIS\.WebAPI\.UI\.Create\.BuildingModelAsync\(this System\.Net\.Http\.HttpClient, long, System\.Nullable\<int\>, double, double\)')\), converts each of its components \(walls, floors and roofs\) into a separate node of a batched [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene') \(translated to a local origin\) and streams it as a binary glTF \(\.glb\) payload\.

Each component carries its own identity in the scene object map, so the 3D viewer can hit-test and select individual components instead of the building as a whole.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetGLBBuildingModelByIdAsync(long id, System.Nullable<int> countyId, System.Nullable<double> storeyHeight=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The unique identifier of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional unique identifier of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetGLBBuildingModelByIdAsync(long,System.Nullable_int_,System.Nullable_double_).storeyHeight'></a>

`storeyHeight` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional storey height in meters used for the extrusion\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the \.glb file\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double)'></a>

## BuildingModelController\.GetItemByReferenceAsync\(string, double, double\) Method

Asynchronously loads a [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel') from the GIS Web API by searching for the building at the specified coordinates, converts its components into separate selectable [DiGi\.GLTF\.Classes\.GLTFNode](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfnode 'DiGi\.GLTF\.Classes\.GLTFNode') instances and renders the 3D viewer page\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByReferenceAsync(string reference, double x, double y);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The reference of the building model\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the building centroid\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemByReferenceAsync(string,double,double).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the building centroid\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') rendering the 3D glTF scene view or a not found response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double,System.Nullable_double_)'></a>

## BuildingModelController\.GetItemsByRadius\(double, double, double, Nullable\<double\>\) Method

Renders the 3D viewer page for all buildings within the specified circular area\. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint\.

The search is purely spatial: the area may span multiple counties, so no county identifier is required.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetItemsByRadius(double centerX, double centerY, double radius, System.Nullable<double> storeyHeight=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double,System.Nullable_double_).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double,System.Nullable_double_).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the search circle\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double,System.Nullable_double_).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the search circle in meters\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.BuildingModelController.GetItemsByRadius(double,double,double,System.Nullable_double_).storeyHeight'></a>

`storeyHeight` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional storey height in meters used for the extrusions\.

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
A [System\.Threading\.Tasks\.Task&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1') holding the calculation result JSON grouped by delay \(ascending\): the propagation ellipsoids, the scattering polylines \(one per [DiGi\.Communication\.Classes\.ScatteringPointGroup](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.scatteringpointgroup 'DiGi\.Communication\.Classes\.ScatteringPointGroup')\) and the angular power distribution vectors, all in world coordinates\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double,System.Nullable_double_)'></a>

## CommunicationController\.GetBuildingsByRadius\(double, double, double, Nullable\<double\>\) Method

Renders the communication 3D scene view for all buildings within the specified circular area \(streamed as a binary glTF payload from the existing glb endpoint\) together with the antenna toolbar\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult GetBuildingsByRadius(double centerX, double centerY, double radius, System.Nullable<double> storeyHeight=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double,System.Nullable_double_).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double,System.Nullable_double_).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double,System.Nullable_double_).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the analyzed circular area in meters\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.CommunicationController.GetBuildingsByRadius(double,double,double,System.Nullable_double_).storeyHeight'></a>

`storeyHeight` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional storey height in meters used for the building extrusions\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double)'></a>

## EPWFileController\.GetEPWFileAsync\(double, double\) Method

Asynchronously retrieves an EPW file based on the specified coordinates\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetEPWFileAsync(double x, double y);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate \(longitude\)\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.GetEPWFileAsync(double,double).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate \(latitude\)\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the HTTP response\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.EPWFileController.Start()'></a>

## EPWFileController\.Start\(\) Method

Returns the starting view\.

```csharp
public Microsoft.AspNetCore.Mvc.IActionResult Start();
```

#### Returns
[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')  
An [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') representing the start view\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_)'></a>

## HeatTransferCoefficientController\.GetRegulatedHeatTransferCoefficientsByReferenceAsync\(string, Nullable\<int\>\) Method

Retrieves regulated heat transfer coefficients for a building identified by its reference,
determining the applicable year based on the building's construction data\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetRegulatedHeatTransferCoefficientsByReferenceAsync(string reference, System.Nullable<int> countyId);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.HeatTransferCoefficientController.GetRegulatedHeatTransferCoefficientsByReferenceAsync(string,System.Nullable_int_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier for the county associated with the building\.

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

Handles the request for the start page and returns the corresponding view\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByIdAsync(string,System.Nullable_int_,System.Nullable_bool_)'></a>

## OccupancyDataController\.GetBuilding2DItemByIdAsync\(string, Nullable\<int\>, Nullable\<bool\>\) Method

Retrieves the occupancy data and building reference for a specific 2D building item by its reference identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetBuilding2DItemByIdAsync(string reference, System.Nullable<int> countyId, System.Nullable<bool> isResidential);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByIdAsync(string,System.Nullable_int_,System.Nullable_bool_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string of the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByIdAsync(string,System.Nullable_int_,System.Nullable_bool_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional ID of the county associated with the building\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OccupancyDataController.GetBuilding2DItemByIdAsync(string,System.Nullable_int_,System.Nullable_bool_).isResidential'></a>

`isResidential` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

An optional flag indicating whether the building is residential\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorAsync(int)'></a>

## OrtoDatasController\.GetEstimatedCoverageFactorAsync\(int\) Method

Retrieves the estimated coverage factor for a specific administrative area L2D identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetEstimatedCoverageFactorAsync(int administrativeAreal2DId);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorAsync(int).administrativeAreal2DId'></a>

`administrativeAreal2DId` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative area L2D\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') with the coverage factor or error status\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable_int_)'></a>

## OrtoDatasController\.GetEstimatedCoverageFactorsAsync\(IEnumerable\<int\>\) Method

Retrieves estimated coverage factors for a collection of administrative area L2D identifiers\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable<int> administrativeAreal2DIds);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetEstimatedCoverageFactorsAsync(System.Collections.Generic.IEnumerable_int_).administrativeAreal2DIds'></a>

`administrativeAreal2DIds` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

An enumerable collection of administrative area L2D identifiers\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation\. The task result contains an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') with the list of coverage factor values or error status\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByIdAsync(string,System.Nullable_int_)'></a>

## OrtoDatasController\.GetItemByIdAsync\(string, Nullable\<int\>\) Method

Retrieves orthodata and building 2D reference information based on a provided reference and optional county ID\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByIdAsync(string reference, System.Nullable<int> countyId);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByIdAsync(string,System.Nullable_int_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference string for the item\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.OrtoDatasController.GetItemByIdAsync(string,System.Nullable_int_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier for the county\.

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

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_)'></a>

## YearBuiltDataController\.GetItemByReferenceAsync\(string, Nullable\<int\>\) Method

Retrieves a year built data item based on the specified reference and optional county identifier\.

```csharp
public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetItemByReferenceAsync(string reference, System.Nullable<int> countyId);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique reference of the item to retrieve\.

<a name='DiGi.GIS.WebAPI.UI.Controllers.YearBuiltDataController.GetItemByReferenceAsync(string,System.Nullable_int_).countyId'></a>

`countyId` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The optional identifier of the county associated with the item\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that represents the asynchronous operation, containing an [Microsoft\.AspNetCore\.Mvc\.IActionResult](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult 'Microsoft\.AspNetCore\.Mvc\.IActionResult') result\.