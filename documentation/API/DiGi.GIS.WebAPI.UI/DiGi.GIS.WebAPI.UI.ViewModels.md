#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.ViewModels Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel'></a>

## AdministrativeAreal2DViewModel Class

Represents a view model for an administrative areal in 2D, providing access to its references and associated data\.

```csharp
public class AdministrativeAreal2DViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → AdministrativeAreal2DViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DViewModel()'></a>

## AdministrativeAreal2DViewModel\(\) Constructor

Initializes a new instance of the [AdministrativeAreal2DViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.AdministrativeAreal2DViewModel') class\.

```csharp
public AdministrativeAreal2DViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DViewModel(DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference,DiGi.GIS.Classes.AdministrativeAreal2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath,System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference_)'></a>

## AdministrativeAreal2DViewModel\(AdministrativeAreal2DReference, AdministrativeAreal2D, AdministrativeAreal2DReferencePath, IEnumerable\<AdministrativeAreal2DReference\>\) Constructor

Initializes a new instance of the [AdministrativeAreal2DViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.AdministrativeAreal2DViewModel') class with specified administrative areal data and references\.

```csharp
public AdministrativeAreal2DViewModel(DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference, DiGi.GIS.Classes.AdministrativeAreal2D? administrativeAreal2D, DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath, System.Collections.Generic.IEnumerable<DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DViewModel(DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference,DiGi.GIS.Classes.AdministrativeAreal2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath,System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference_).administrativeAreal2DReference'></a>

`administrativeAreal2DReference` [DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference')

The reference to the administrative areal 2D\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DViewModel(DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference,DiGi.GIS.Classes.AdministrativeAreal2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath,System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference_).administrativeAreal2D'></a>

`administrativeAreal2D` [DiGi\.GIS\.Classes\.AdministrativeAreal2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.administrativeareal2d 'DiGi\.GIS\.Classes\.AdministrativeAreal2D')

The administrative areal 2D object\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DViewModel(DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference,DiGi.GIS.Classes.AdministrativeAreal2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath,System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference_).administrativeAreal2DReferencePath'></a>

`administrativeAreal2DReferencePath` [DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreferencepath 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath')

The path reference for the administrative areal 2D\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DViewModel(DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference,DiGi.GIS.Classes.AdministrativeAreal2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath,System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference_).administrativeAreal2DReferences'></a>

`administrativeAreal2DReferences` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

A collection of references to administrative areals 2D\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2D'></a>

## AdministrativeAreal2DViewModel\.AdministrativeAreal2D Property

Gets the administrative areal 2D object\.

```csharp
public DiGi.GIS.Classes.AdministrativeAreal2D? AdministrativeAreal2D { get; }
```

#### Property Value
[DiGi\.GIS\.Classes\.AdministrativeAreal2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.administrativeareal2d 'DiGi\.GIS\.Classes\.AdministrativeAreal2D')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DReference'></a>

## AdministrativeAreal2DViewModel\.AdministrativeAreal2DReference Property

Gets the reference to the administrative areal 2D\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference? AdministrativeAreal2DReference { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DReferencePath'></a>

## AdministrativeAreal2DViewModel\.AdministrativeAreal2DReferencePath Property

Gets the path reference for the administrative areal 2D\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath? AdministrativeAreal2DReferencePath { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreferencepath 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.AdministrativeAreal2DViewModel.AdministrativeAreal2DReferences'></a>

## AdministrativeAreal2DViewModel\.AdministrativeAreal2DReferences Property

Gets a list of references to administrative areals 2D\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReference>? AdministrativeAreal2DReferences { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReference')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel'></a>

## Building2DOccupancyDataViewModel Class

Represents a view model that provides combined access to [Building2DReference](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.Building2DReference 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DOccupancyDataViewModel\.Building2DReference') and [DiGi\.GIS\.Interfaces\.IOccupancyData](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.interfaces.ioccupancydata 'DiGi\.GIS\.Interfaces\.IOccupancyData')\.

```csharp
public class Building2DOccupancyDataViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Building2DOccupancyDataViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.Building2DOccupancyDataViewModel()'></a>

## Building2DOccupancyDataViewModel\(\) Constructor

Initializes a new instance of the [Building2DOccupancyDataViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DOccupancyDataViewModel') class\.

```csharp
public Building2DOccupancyDataViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.Building2DOccupancyDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Interfaces.IOccupancyData)'></a>

## Building2DOccupancyDataViewModel\(Building2DReference, IOccupancyData\) Constructor

Initializes a new instance of the [Building2DOccupancyDataViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DOccupancyDataViewModel') class\.

```csharp
public Building2DOccupancyDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference? building2DReference, DiGi.GIS.Interfaces.IOccupancyData? occupancyData);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.Building2DOccupancyDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Interfaces.IOccupancyData).building2DReference'></a>

`building2DReference` [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

The [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference') reference to the 2D building, or `null`\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.Building2DOccupancyDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Interfaces.IOccupancyData).occupancyData'></a>

`occupancyData` [DiGi\.GIS\.Interfaces\.IOccupancyData](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.interfaces.ioccupancydata 'DiGi\.GIS\.Interfaces\.IOccupancyData')

The [DiGi\.GIS\.Interfaces\.IOccupancyData](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.interfaces.ioccupancydata 'DiGi\.GIS\.Interfaces\.IOccupancyData') containing occupancy data, or `null`\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.Building2DReference'></a>

## Building2DOccupancyDataViewModel\.Building2DReference Property

Gets the reference to the 2D building associated with this occupancy data view\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.Building2DReference? Building2DReference { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DOccupancyDataViewModel.OccupancyData'></a>

## Building2DOccupancyDataViewModel\.OccupancyData Property

Gets the occupancy data associated with this building 2D occupancy data view\.

```csharp
public DiGi.GIS.Interfaces.IOccupancyData? OccupancyData { get; }
```

#### Property Value
[DiGi\.GIS\.Interfaces\.IOccupancyData](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.interfaces.ioccupancydata 'DiGi\.GIS\.Interfaces\.IOccupancyData')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel'></a>

## Building2DReferencesViewModel Class

Represents a view model containing a collection of 2D building references\.

```csharp
public class Building2DReferencesViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Building2DReferencesViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel.Building2DReferencesViewModel()'></a>

## Building2DReferencesViewModel\(\) Constructor

Initializes a new instance of the [Building2DReferencesViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DReferencesViewModel') class\.

```csharp
public Building2DReferencesViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel.Building2DReferencesViewModel(System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.Building2DReference_)'></a>

## Building2DReferencesViewModel\(IEnumerable\<Building2DReference\>\) Constructor

Initializes a new instance of the [Building2DReferencesViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DReferencesViewModel') class with a specified collection of 2D building references\.

```csharp
public Building2DReferencesViewModel(System.Collections.Generic.IEnumerable<DiGi.GIS.PostgreSQL.Classes.Building2DReference>? building2DReferences);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel.Building2DReferencesViewModel(System.Collections.Generic.IEnumerable_DiGi.GIS.PostgreSQL.Classes.Building2DReference_).building2DReferences'></a>

`building2DReferences` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The collection of [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference') objects to initialize the view model with\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DReferencesViewModel.Building2DReferences'></a>

## Building2DReferencesViewModel\.Building2DReferences Property

Gets the list of 2D building references associated with this view\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.PostgreSQL.Classes.Building2DReference>? Building2DReferences { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel'></a>

## Building2DViewModel Class

Represents a 2D view model of a building, providing access to its reference, spatial data, and administrative areal path\.

```csharp
public class Building2DViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Building2DViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2DViewModel()'></a>

## Building2DViewModel\(\) Constructor

Initializes a new instance of the [Building2DViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DViewModel') class\.

```csharp
public Building2DViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2DViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.Building2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath)'></a>

## Building2DViewModel\(Building2DReference, Building2D, AdministrativeAreal2DReferencePath\) Constructor

Initializes a new instance of the [Building2DViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DViewModel') class\.

```csharp
public Building2DViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference? building2DReference, DiGi.GIS.Classes.Building2D? building2D, DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2DViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.Building2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath).building2DReference'></a>

`building2DReference` [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

The [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference') for the building\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2DViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.Building2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath).building2D'></a>

`building2D` [DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')

The [Building2D](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2D 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DViewModel\.Building2D') associated with this view model\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2DViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.Building2D,DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath).administrativeAreal2DReferencePath'></a>

`administrativeAreal2DReferencePath` [DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreferencepath 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath')

The [DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreferencepath 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath') for the building\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.AdministrativeAreal2DReferencePath'></a>

## Building2DViewModel\.AdministrativeAreal2DReferencePath Property

Gets the collection of administrative 2D area references for this building view\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.AdministrativeAreal2DReferencePath? AdministrativeAreal2DReferencePath { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.administrativeareal2dreferencepath 'DiGi\.GIS\.PostgreSQL\.Classes\.AdministrativeAreal2DReferencePath')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2D'></a>

## Building2DViewModel\.Building2D Property

Gets the 2D building associated with this view\.

```csharp
public DiGi.GIS.Classes.Building2D? Building2D { get; }
```

#### Property Value
[DiGi\.GIS\.Classes\.Building2D](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.building2d 'DiGi\.GIS\.Classes\.Building2D')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DViewModel.Building2DReference'></a>

## Building2DViewModel\.Building2DReference Property

Gets the reference to the 2D building associated with this view\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.Building2DReference? Building2DReference { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel'></a>

## CommunicationSceneViewModel Class

Represents a view model for the communication 3D scene view: the buildings of the analyzed circular area \(streamed as a binary glTF payload\) plus the input parameters required to send the communication calculation request back to the server\.

```csharp
public class CommunicationSceneViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → CommunicationSceneViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double)'></a>

## CommunicationSceneViewModel\(string, string, double, double, double, double\) Constructor

Initializes a new instance of the [CommunicationSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.CommunicationSceneViewModel') class\.

```csharp
public CommunicationSceneViewModel(string? title, string? gLBUrl, double centerX, double centerY, double radius, double storeyHeight);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double).gLBUrl'></a>

`gLBUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the binary glTF \(\.glb\) endpoint\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the analyzed circular area in meters\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double,double).storeyHeight'></a>

`storeyHeight` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The storey height in meters used for the building extrusions\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CalculateUrl'></a>

## CommunicationSceneViewModel\.CalculateUrl Property

Gets or sets the application relative URL of the communication calculation endpoint used by the 3D scene\.

```csharp
public string? CalculateUrl { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CenterX'></a>

## CommunicationSceneViewModel\.CenterX Property

Gets the X coordinate of the center of the analyzed circular area\.

```csharp
public double CenterX { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CenterY'></a>

## CommunicationSceneViewModel\.CenterY Property

Gets the Y coordinate of the center of the analyzed circular area\.

```csharp
public double CenterY { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.GLBUrl'></a>

## CommunicationSceneViewModel\.GLBUrl Property

Gets the application relative URL of the binary glTF \(\.glb\) endpoint\.

```csharp
public string? GLBUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.Radius'></a>

## CommunicationSceneViewModel\.Radius Property

Gets the radius of the analyzed circular area in meters\.

```csharp
public double Radius { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.StoreyHeight'></a>

## CommunicationSceneViewModel\.StoreyHeight Property

Gets the storey height in meters used for the building extrusions\.

```csharp
public double StoreyHeight { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.Title'></a>

## CommunicationSceneViewModel\.Title Property

Gets the title displayed above the viewer\.

```csharp
public string? Title { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel'></a>

## EPWFileViewModel Class

Represents a view model for an EPW file, providing access to the underlying weather data and structure\.

```csharp
public class EPWFileViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → EPWFileViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel.EPWFileViewModel()'></a>

## EPWFileViewModel\(\) Constructor

Initializes a new instance of the [EPWFileViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.EPWFileViewModel') class\.

```csharp
public EPWFileViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel.EPWFileViewModel(DiGi.EPW.Classes.EPWFile)'></a>

## EPWFileViewModel\(EPWFile\) Constructor

Initializes a new instance of the [EPWFileViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.EPWFileViewModel') class\.

```csharp
public EPWFileViewModel(DiGi.EPW.Classes.EPWFile? epwFile);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel.EPWFileViewModel(DiGi.EPW.Classes.EPWFile).epwFile'></a>

`epwFile` [DiGi\.EPW\.Classes\.EPWFile](https://learn.microsoft.com/en-us/dotnet/api/digi.epw.classes.epwfile 'DiGi\.EPW\.Classes\.EPWFile')

The EPW file to be associated with this view model\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.EPWFileViewModel.EPWFile'></a>

## EPWFileViewModel\.EPWFile Property

Gets the EPW file associated with the EPW file view\.

```csharp
public DiGi.EPW.Classes.EPWFile? EPWFile { get; }
```

#### Property Value
[DiGi\.EPW\.Classes\.EPWFile](https://learn.microsoft.com/en-us/dotnet/api/digi.epw.classes.epwfile 'DiGi\.EPW\.Classes\.EPWFile')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel'></a>

## GLTFSceneViewModel Class

Represents a view model for rendering a [GLTFScene](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFScene 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel\.GLTFScene') in the 3D glTF viewer\.

Two delivery modes are supported: streamed (the view carries only [GLBUrl](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLBUrl 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel\.GLBUrl') and the viewer fetches the binary glTF payload, whose scene extras are fully self-describing) and embedded (the scene JSON and the base64 encoded payload are inlined in the page).

```csharp
public class GLTFSceneViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → GLTFSceneViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel()'></a>

## GLTFSceneViewModel\(\) Constructor

Initializes a new instance of the [GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel') class\.

```csharp
public GLTFSceneViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(DiGi.GLTF.Classes.GLTFScene,string,string,string)'></a>

## GLTFSceneViewModel\(GLTFScene, string, string, string\) Constructor

Initializes a new instance of the [GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel') class for embedded delivery\.

```csharp
public GLTFSceneViewModel(DiGi.GLTF.Classes.GLTFScene? gLTFScene, string? gLTFSceneJson, string? gLBBase64, string? title);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(DiGi.GLTF.Classes.GLTFScene,string,string,string).gLTFScene'></a>

`gLTFScene` [DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene')

The [GLTFScene](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFScene 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel\.GLTFScene') to be rendered\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(DiGi.GLTF.Classes.GLTFScene,string,string,string).gLTFSceneJson'></a>

`gLTFSceneJson` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The JSON representation of the scene used by the viewer for lights, camera and reference point configuration\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(DiGi.GLTF.Classes.GLTFScene,string,string,string).gLBBase64'></a>

`gLBBase64` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The base64 encoded binary glTF \(\.glb\) payload rendered by the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(DiGi.GLTF.Classes.GLTFScene,string,string,string).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string)'></a>

## GLTFSceneViewModel\(string, string\) Constructor

Initializes a new instance of the [GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel') class for streamed delivery: the viewer fetches the binary glTF payload from [gLBUrl](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string).gLBUrl 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel\.GLTFSceneViewModel\(string, string\)\.gLBUrl') and reads the scene configuration from its extras\.

```csharp
public GLTFSceneViewModel(string? title, string? gLBUrl);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string).gLBUrl'></a>

`gLBUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the binary glTF \(\.glb\) endpoint\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLBBase64'></a>

## GLTFSceneViewModel\.GLBBase64 Property

Gets the base64 encoded binary glTF \(\.glb\) payload rendered by the viewer \(embedded delivery only\)\.

```csharp
public string? GLBBase64 { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLBUrl'></a>

## GLTFSceneViewModel\.GLBUrl Property

Gets the application relative URL of the binary glTF \(\.glb\) endpoint \(streamed delivery only\)\.

```csharp
public string? GLBUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFScene'></a>

## GLTFSceneViewModel\.GLTFScene Property

Gets the [GLTFScene](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFScene 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel\.GLTFScene') to be rendered \(embedded delivery only\)\.

```csharp
public DiGi.GLTF.Classes.GLTFScene? GLTFScene { get; }
```

#### Property Value
[DiGi\.GLTF\.Classes\.GLTFScene](https://learn.microsoft.com/en-us/dotnet/api/digi.gltf.classes.gltfscene 'DiGi\.GLTF\.Classes\.GLTFScene')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneJson'></a>

## GLTFSceneViewModel\.GLTFSceneJson Property

Gets the JSON representation of the scene used by the viewer for lights, camera and reference point configuration \(embedded delivery only\)\.

```csharp
public string? GLTFSceneJson { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.Title'></a>

## GLTFSceneViewModel\.Title Property

Gets the title displayed above the viewer\.

```csharp
public string? Title { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel'></a>

## OrtoDatasViewModel Class

Represents a view model that combines orthographic data and a 2D building reference\.

```csharp
public class OrtoDatasViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → OrtoDatasViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel.OrtoDatasViewModel()'></a>

## OrtoDatasViewModel\(\) Constructor

Initializes a new instance of the [OrtoDatasViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.OrtoDatasViewModel') class\.

```csharp
public OrtoDatasViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel.OrtoDatasViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.OrtoDatas)'></a>

## OrtoDatasViewModel\(Building2DReference, OrtoDatas\) Constructor

Initializes a new instance of the [OrtoDatasViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.OrtoDatasViewModel') class with the specified building reference and orthographic data\.

```csharp
public OrtoDatasViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference? building2DReference, DiGi.GIS.Classes.OrtoDatas? ortoDatas);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel.OrtoDatasViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.OrtoDatas).building2DReference'></a>

`building2DReference` [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

The reference to the 2D building\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel.OrtoDatasViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Classes.OrtoDatas).ortoDatas'></a>

`ortoDatas` [DiGi\.GIS\.Classes\.OrtoDatas](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.ortodatas 'DiGi\.GIS\.Classes\.OrtoDatas')

The orthographic data associated with the view model\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel.Building2DReference'></a>

## OrtoDatasViewModel\.Building2DReference Property

Gets the reference to the 2D building associated with this view\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.Building2DReference? Building2DReference { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.OrtoDatasViewModel.OrtoDatas'></a>

## OrtoDatasViewModel\.OrtoDatas Property

Gets the orthographic data associated with this view\.

```csharp
public DiGi.GIS.Classes.OrtoDatas? OrtoDatas { get; }
```

#### Property Value
[DiGi\.GIS\.Classes\.OrtoDatas](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.classes.ortodatas 'DiGi\.GIS\.Classes\.OrtoDatas')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel'></a>

## RegulatedHeatTransferCoefficientsViewModel Class

Represents a view model of the regulated heat transfer coefficients\.

```csharp
public class RegulatedHeatTransferCoefficientsViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → RegulatedHeatTransferCoefficientsViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.RegulatedHeatTransferCoefficientsViewModel()'></a>

## RegulatedHeatTransferCoefficientsViewModel\(\) Constructor

Initializes a new instance of the [RegulatedHeatTransferCoefficientsViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.RegulatedHeatTransferCoefficientsViewModel') class\.

```csharp
public RegulatedHeatTransferCoefficientsViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.RegulatedHeatTransferCoefficientsViewModel(short,DiGi.Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients,System.Nullable_bool_)'></a>

## RegulatedHeatTransferCoefficientsViewModel\(short, IRegulatedHeatTransferCoefficients, Nullable\<bool\>\) Constructor

Initializes a new instance of the [RegulatedHeatTransferCoefficientsViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.RegulatedHeatTransferCoefficientsViewModel') class\.

```csharp
public RegulatedHeatTransferCoefficientsViewModel(short year, DiGi.Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients? regulatedHeatTransferCoefficients, System.Nullable<bool> isResidential);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.RegulatedHeatTransferCoefficientsViewModel(short,DiGi.Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients,System.Nullable_bool_).year'></a>

`year` [System\.Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16 'System\.Int16')

The year associated with the regulated heat transfer coefficients\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.RegulatedHeatTransferCoefficientsViewModel(short,DiGi.Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients,System.Nullable_bool_).regulatedHeatTransferCoefficients'></a>

`regulatedHeatTransferCoefficients` [DiGi\.Analytical\.Building\.HVAC\.Interfaces\.IRegulatedHeatTransferCoefficients](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.hvac.interfaces.iregulatedheattransfercoefficients 'DiGi\.Analytical\.Building\.HVAC\.Interfaces\.IRegulatedHeatTransferCoefficients')

The regulated heat transfer coefficients\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.RegulatedHeatTransferCoefficientsViewModel(short,DiGi.Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients,System.Nullable_bool_).isResidential'></a>

`isResidential` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

A value indicating whether the building is residential\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.IsResidential'></a>

## RegulatedHeatTransferCoefficientsViewModel\.IsResidential Property

Gets a value indicating whether the building is residential\.

```csharp
public System.Nullable<bool> IsResidential { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.RegulatedHeatTransferCoefficients'></a>

## RegulatedHeatTransferCoefficientsViewModel\.RegulatedHeatTransferCoefficients Property

Gets the regulated heat transfer coefficients\.

```csharp
public DiGi.Analytical.Building.HVAC.Interfaces.IRegulatedHeatTransferCoefficients? RegulatedHeatTransferCoefficients { get; }
```

#### Property Value
[DiGi\.Analytical\.Building\.HVAC\.Interfaces\.IRegulatedHeatTransferCoefficients](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.hvac.interfaces.iregulatedheattransfercoefficients 'DiGi\.Analytical\.Building\.HVAC\.Interfaces\.IRegulatedHeatTransferCoefficients')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.RegulatedHeatTransferCoefficientsViewModel.Year'></a>

## RegulatedHeatTransferCoefficientsViewModel\.Year Property

Gets the year associated with the regulated heat transfer coefficients\.

```csharp
public short Year { get; }
```

#### Property Value
[System\.Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16 'System\.Int16')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel'></a>

## TableViewModel Class

Represents a view model for a PostgreSQL table, providing access to the underlying table data and structure\.

```csharp
public class TableViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TableViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel.TableViewModel()'></a>

## TableViewModel\(\) Constructor

Initializes a new instance of the [TableViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TableViewModel') class\.

```csharp
public TableViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel.TableViewModel(DiGi.PostgreSQL.Table.Classes.Table)'></a>

## TableViewModel\(Table\) Constructor

Initializes a new instance of the [TableViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TableViewModel') class\.

```csharp
public TableViewModel(DiGi.PostgreSQL.Table.Classes.Table? table);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel.TableViewModel(DiGi.PostgreSQL.Table.Classes.Table).table'></a>

`table` [DiGi\.PostgreSQL\.Table\.Classes\.Table](https://learn.microsoft.com/en-us/dotnet/api/digi.postgresql.table.classes.table 'DiGi\.PostgreSQL\.Table\.Classes\.Table')

The PostgreSQL table to be associated with this view model\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TableViewModel.Table'></a>

## TableViewModel\.Table Property

Gets the table associated with the table view\.

```csharp
public DiGi.PostgreSQL.Table.Classes.Table? Table { get; }
```

#### Property Value
[DiGi\.PostgreSQL\.Table\.Classes\.Table](https://learn.microsoft.com/en-us/dotnet/api/digi.postgresql.table.classes.table 'DiGi\.PostgreSQL\.Table\.Classes\.Table')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel'></a>

## YearBuiltDataViewModel Class

Represents a view model that associates a 2D building reference with its corresponding year built data\.

```csharp
public class YearBuiltDataViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → YearBuiltDataViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel.YearBuiltDataViewModel()'></a>

## YearBuiltDataViewModel\(\) Constructor

Initializes a new instance of the [YearBuiltDataViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.YearBuiltDataViewModel') class\.

```csharp
public YearBuiltDataViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel.YearBuiltDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Interfaces.IYearBuiltData)'></a>

## YearBuiltDataViewModel\(Building2DReference, IYearBuiltData\) Constructor

Initializes a new instance of the [YearBuiltDataViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.YearBuiltDataViewModel') class with specified building reference and year built data\.

```csharp
public YearBuiltDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference? building2DReference, DiGi.GIS.Interfaces.IYearBuiltData? yearBuiltData);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel.YearBuiltDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Interfaces.IYearBuiltData).building2DReference'></a>

`building2DReference` [DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

The reference to the 2D building\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel.YearBuiltDataViewModel(DiGi.GIS.PostgreSQL.Classes.Building2DReference,DiGi.GIS.Interfaces.IYearBuiltData).yearBuiltData'></a>

`yearBuiltData` [DiGi\.GIS\.Interfaces\.IYearBuiltData](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.interfaces.iyearbuiltdata 'DiGi\.GIS\.Interfaces\.IYearBuiltData')

The year built data associated with the building\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel.Building2DReference'></a>

## YearBuiltDataViewModel\.Building2DReference Property

Gets the reference to the 2D building\.

```csharp
public DiGi.GIS.PostgreSQL.Classes.Building2DReference? Building2DReference { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.classes.building2dreference 'DiGi\.GIS\.PostgreSQL\.Classes\.Building2DReference')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.YearBuiltDataViewModel.YearBuiltData'></a>

## YearBuiltDataViewModel\.YearBuiltData Property

Gets the year built data associated with the building\.

```csharp
public DiGi.GIS.Interfaces.IYearBuiltData? YearBuiltData { get; }
```

#### Property Value
[DiGi\.GIS\.Interfaces\.IYearBuiltData](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.interfaces.iyearbuiltdata 'DiGi\.GIS\.Interfaces\.IYearBuiltData')