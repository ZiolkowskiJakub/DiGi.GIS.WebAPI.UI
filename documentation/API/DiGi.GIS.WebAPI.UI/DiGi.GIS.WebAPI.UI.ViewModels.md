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

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel'></a>

## Building2DCentroidViewModel Class

One building dot of a 2D area view: the bounding\-box centre of a `building_2d` row, keyed by the reference and the county part the row is filed under\.

A building reference is unique only per county partition - a reference is stored once per county part it was imported under - so the county identifier travels with the reference for any caller-side join, such as the Typology area view's join with the solved typology assignment. [X](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.X 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DCentroidViewModel\.X') and [Y](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Y 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DCentroidViewModel\.Y') are the centre of the row's bounding-box columns in the same coordinate reference system the `point2dsbyreferences` endpoint answers in (PL-1992, EPSG:2180, metres). Row order is not contractual.

A relay of `DiGi.GIS.PostgreSQL.Classes.Building2DCentroid` without its `_type` discriminator: the browser never spells a type name, and the discriminator alone is most of the upstream payload for a county-sized area.

```csharp
public class Building2DCentroidViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Building2DCentroidViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Building2DCentroidViewModel()'></a>

## Building2DCentroidViewModel\(\) Constructor

Initializes a new instance of the [Building2DCentroidViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DCentroidViewModel') class\.

```csharp
public Building2DCentroidViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Building2DCentroidViewModel(string,int,double,double)'></a>

## Building2DCentroidViewModel\(string, int, double, double\) Constructor

Initializes a new instance of the [Building2DCentroidViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.Building2DCentroidViewModel') class\.

```csharp
public Building2DCentroidViewModel(string reference, int countyId, double x, double y);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Building2DCentroidViewModel(string,int,double,double).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The building reference key\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Building2DCentroidViewModel(string,int,double,double).countyId'></a>

`countyId` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The county part identifier the building row is filed under\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Building2DCentroidViewModel(string,int,double,double).x'></a>

`x` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the bounding\-box centre\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Building2DCentroidViewModel(string,int,double,double).y'></a>

`y` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the bounding\-box centre\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.CountyId'></a>

## Building2DCentroidViewModel\.CountyId Property

Gets the county part identifier the building row is filed under\.

```csharp
public int CountyId { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Reference'></a>

## Building2DCentroidViewModel\.Reference Property

Gets the building reference key, as addressed by the GIS Web API\.

```csharp
public string Reference { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.X'></a>

## Building2DCentroidViewModel\.X Property

Gets the X coordinate of the bounding\-box centre, in PL\-1992 \(EPSG:2180\) metres\.

```csharp
public double X { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.Building2DCentroidViewModel.Y'></a>

## Building2DCentroidViewModel\.Y Property

Gets the Y coordinate of the bounding\-box centre, in PL\-1992 \(EPSG:2180\) metres\.

```csharp
public double Y { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

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

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double)'></a>

## CommunicationSceneViewModel\(string, string, double, double, double\) Constructor

Initializes a new instance of the [CommunicationSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.CommunicationSceneViewModel') class\.

```csharp
public CommunicationSceneViewModel(string? title, string? gLBUrl, double centerX, double centerY, double radius);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double).gLBUrl'></a>

`gLBUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the binary glTF \(\.glb\) endpoint\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double).centerX'></a>

`centerX` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The X coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double).centerY'></a>

`centerY` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The Y coordinate of the center of the analyzed circular area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.CommunicationSceneViewModel.CommunicationSceneViewModel(string,string,double,double,double).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The radius of the analyzed circular area in meters\.
### Properties

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

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string,string)'></a>

## GLTFSceneViewModel\(string, string, string\) Constructor

Initializes a new instance of the [GLTFSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel') class for streamed delivery: the viewer fetches the binary glTF payload from [gLBUrl](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string,string).gLBUrl 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.GLTFSceneViewModel\.GLTFSceneViewModel\(string, string, string\)\.gLBUrl') and reads the scene configuration from its extras\.

```csharp
public GLTFSceneViewModel(string? title, string? gLBUrl, string? scopeBoxSize=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string,string).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string,string).gLBUrl'></a>

`gLBUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the binary glTF \(\.glb\) endpoint\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.GLTFSceneViewModel(string,string,string).scopeBoxSize'></a>

`scopeBoxSize` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The default scope box preset in the form "halfX;halfY;zMin;zMax" \(DiGi coordinates\) passed to the viewer, or null for the bounds\-fit default\. This value can be null\.
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

<a name='DiGi.GIS.WebAPI.UI.ViewModels.GLTFSceneViewModel.ScopeBoxSize'></a>

## GLTFSceneViewModel\.ScopeBoxSize Property

Gets the default scope box preset in the form "halfX;halfY;zMin;zMax" \(DiGi coordinates\) passed to the viewer, or null for the bounds\-fit default\.

```csharp
public string? ScopeBoxSize { get; }
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

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel'></a>

## SolarJobViewModel Class

Represents the state of a background solar radiation job as the `solar/jobs` routes answer it and the solar radiation viewer polls it\.

```csharp
public class SolarJobViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → SolarJobViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string)'></a>

## SolarJobViewModel\(string, string, Nullable\<int\>, double, int, string\) Constructor

Initializes a new instance of the [SolarJobViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.SolarJobViewModel') class\.

```csharp
public SolarJobViewModel(string jobId, string status, System.Nullable<int> queuePosition, double elapsedSeconds, int receiverCount, string? error);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string).jobId'></a>

`jobId` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The unique identifier of the job\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string).status'></a>

`status` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The state of the job, the name of an [SolarJobStatus](DiGi.GIS.WebAPI.UI.Enums.md#DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus 'DiGi\.GIS\.WebAPI\.UI\.Enums\.SolarJobStatus') member\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string).queuePosition'></a>

`queuePosition` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The place of the job in the queue \(0 while it runs\), or null once it has finished\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string).elapsedSeconds'></a>

`elapsedSeconds` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The seconds spent in the current state: waiting while queued, calculating while running, and the calculation time once finished\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string).receiverCount'></a>

`receiverCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of receiving surfaces \(external walls and roofs\) the job calculates\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.SolarJobViewModel(string,string,System.Nullable_int_,double,int,string).error'></a>

`error` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error text of a failed job\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.ElapsedSeconds'></a>

## SolarJobViewModel\.ElapsedSeconds Property

Gets the seconds spent in the current state: waiting while queued, calculating while running, and the calculation time once finished\.

```csharp
public double ElapsedSeconds { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.Error'></a>

## SolarJobViewModel\.Error Property

Gets the error text of a failed job\.

```csharp
public string? Error { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.JobId'></a>

## SolarJobViewModel\.JobId Property

Gets the unique identifier of the job\.

```csharp
public string JobId { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.QueuePosition'></a>

## SolarJobViewModel\.QueuePosition Property

Gets the place of the job in the queue \(0 while it runs\), or null once it has finished\.

```csharp
public System.Nullable<int> QueuePosition { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.ReceiverCount'></a>

## SolarJobViewModel\.ReceiverCount Property

Gets the number of receiving surfaces \(external walls and roofs\) the job calculates\.

```csharp
public int ReceiverCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarJobViewModel.Status'></a>

## SolarJobViewModel\.Status Property

Gets the state of the job, the name of an [SolarJobStatus](DiGi.GIS.WebAPI.UI.Enums.md#DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus 'DiGi\.GIS\.WebAPI\.UI\.Enums\.SolarJobStatus') member; a name rather than the number the JSON serializer would write for the enum\.

```csharp
public string Status { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel'></a>

## SolarSceneViewModel Class

Represents a view model for the solar radiation 3D scene view: the building and its neighbours, streamed as a binary glTF payload with the receiving surfaces coloured by their annual irradiation, plus what the legend states about the calculation\.

```csharp
public class SolarSceneViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → SolarSceneViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string)'></a>

## SolarSceneViewModel\(string, string, string, string, double, string, string\) Constructor

Initializes a new instance of the [SolarSceneViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.SolarSceneViewModel') class\.

```csharp
public SolarSceneViewModel(string? title, string? gLBUrl, string? jobsUrl, string? jobQuery, double radius, string? stationName, string? stationUrl);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).title'></a>

`title` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The title displayed above the viewer\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).gLBUrl'></a>

`gLBUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the binary glTF \(\.glb\) endpoint\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).jobsUrl'></a>

`jobsUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the background job routes \(`solar/jobs`\), offered when the scene request is refused as too large\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).jobQuery'></a>

`jobQuery` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The query string \(`id`, `countyid`, `radius`\) a background job is posted with\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).radius'></a>

`radius` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The neighbour radius in metres, measured from the edge of the footprint\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).stationName'></a>

`stationName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the EPW weather station, or null when the file does not state it\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.SolarSceneViewModel(string,string,string,string,double,string,string).stationUrl'></a>

`stationUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The application relative URL of the EPW weather file page of the station\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.GLBUrl'></a>

## SolarSceneViewModel\.GLBUrl Property

Gets the application relative URL of the binary glTF \(\.glb\) endpoint\.

```csharp
public string? GLBUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.JobQuery'></a>

## SolarSceneViewModel\.JobQuery Property

Gets the query string \(`id`, `countyid`, `radius`\) a background job is posted with\.

```csharp
public string? JobQuery { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.JobsUrl'></a>

## SolarSceneViewModel\.JobsUrl Property

Gets the application relative URL of the background job routes \(`solar/jobs`\), offered when the scene request is refused as too large\.

```csharp
public string? JobsUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.Radius'></a>

## SolarSceneViewModel\.Radius Property

Gets the neighbour radius in metres, measured from the edge of the footprint\.

```csharp
public double Radius { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.StationName'></a>

## SolarSceneViewModel\.StationName Property

Gets the name of the EPW weather station, or null when the file does not state it\.

```csharp
public string? StationName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.StationUrl'></a>

## SolarSceneViewModel\.StationUrl Property

Gets the application relative URL of the EPW weather file page of the station\.

```csharp
public string? StationUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.SolarSceneViewModel.Title'></a>

## SolarSceneViewModel\.Title Property

Gets the title displayed above the viewer\.

```csharp
public string? Title { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

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

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel'></a>

## TypologyAreaCountViewModel Class

The pre\-flight answer of `GET /typology/buildingcount`: how many buildings a Typology solve of an area reads, known before the solve starts, so the area view can say what it is waiting for \(DiGi\.GIS\.WebAPI\.UI\#29, B1\)\.

```csharp
public class TypologyAreaCountViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyAreaCountViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.TypologyAreaCountViewModel()'></a>

## TypologyAreaCountViewModel\(\) Constructor

Initializes a new instance of the [TypologyAreaCountViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyAreaCountViewModel') class\.

```csharp
public TypologyAreaCountViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.TypologyAreaCountViewModel(System.Nullable_long_,int,int,bool)'></a>

## TypologyAreaCountViewModel\(Nullable\<long\>, int, int, bool\) Constructor

Initializes a new instance of the [TypologyAreaCountViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyAreaCountViewModel') class\.

```csharp
public TypologyAreaCountViewModel(System.Nullable<long> count, int countyPartCount, int ceiling, bool clipped);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.TypologyAreaCountViewModel(System.Nullable_long_,int,int,bool).count'></a>

`count` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The number of buildings the solve reads, or null when it is not counted\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.TypologyAreaCountViewModel(System.Nullable_long_,int,int,bool).countyPartCount'></a>

`countyPartCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of county parts the solve reads\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.TypologyAreaCountViewModel(System.Nullable_long_,int,int,bool).ceiling'></a>

`ceiling` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of buildings above which a solve is refused\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.TypologyAreaCountViewModel(System.Nullable_long_,int,int,bool).clipped'></a>

`clipped` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

A value indicating whether the solve clips its county parts to the area's polygon\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.Ceiling'></a>

## TypologyAreaCountViewModel\.Ceiling Property

Gets the number of buildings above which a solve is refused with a 413 \([BuildingSolveCeiling](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.BuildingSolveCeiling 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.BuildingSolveCeiling')\)\.

```csharp
public int Ceiling { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.Clipped'></a>

## TypologyAreaCountViewModel\.Clipped Property

Gets a value indicating whether the area is a municipality or subdivision, whose county parts are clipped to its polygon after the read \- so [Count](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.Count 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyAreaCountViewModel\.Count') is an upper bound \("up to"\) rather than the area's count\.

```csharp
public bool Clipped { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.Count'></a>

## TypologyAreaCountViewModel\.Count Property

Gets the number of buildings the solve reads: the sum over the area's county parts\. For a clipped area it is the count of the county parts the area lies in, an upper bound of the area's own buildings\. Null for a country, which is not counted \- it is refused as above the ceiling outright\.

```csharp
public System.Nullable<long> Count { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyAreaCountViewModel.CountyPartCount'></a>

## TypologyAreaCountViewModel\.CountyPartCount Property

Gets the number of county parts the solve reads\.

```csharp
public int CountyPartCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel'></a>

## TypologyBuildingsViewModel Class

The response of `POST /typology/buildings`: the solved Typology tree and the flat building list the view renders as dots\.

The [Root](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.Root 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingsViewModel\.Root') is the recursive tree the view renders as the typology panel: each node carries its name, description, color and children. The [Buildings](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.Buildings 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingsViewModel\.Buildings') is the flat list of building entries, one per reference the solve classified, each filed under the deepest node holding it - a leaf bucket, or the bucket whose next level dropped the row; the view joins each entry to its dot position by `(Reference, CountyId)` against the area-scoped centroid endpoint.

```csharp
public class TypologyBuildingsViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyBuildingsViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.TypologyBuildingsViewModel()'></a>

## TypologyBuildingsViewModel\(\) Constructor

Initializes a new instance of the [TypologyBuildingsViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingsViewModel') class\.

```csharp
public TypologyBuildingsViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.TypologyBuildingsViewModel(DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel_)'></a>

## TypologyBuildingsViewModel\(TypologyTreeNodeViewModel, List\<TypologyBuildingViewModel\>\) Constructor

Initializes a new instance of the [TypologyBuildingsViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingsViewModel') class\.

```csharp
public TypologyBuildingsViewModel(DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel? root, System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel> buildings);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.TypologyBuildingsViewModel(DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel_).root'></a>

`root` [TypologyTreeNodeViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel')

The root of the solved typology tree, or null when the solve produced no nodes\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.TypologyBuildingsViewModel(DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel_).buildings'></a>

`buildings` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyBuildingViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingViewModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

The flat list of building entries, one per classified reference, filed under the deepest node holding it\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.Buildings'></a>

## TypologyBuildingsViewModel\.Buildings Property

Gets the flat list of building entries, one per classified reference, filed under the deepest node holding it, emitted in the sorted tree's order\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel> Buildings { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyBuildingViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingViewModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingsViewModel.Root'></a>

## TypologyBuildingsViewModel\.Root Property

Gets the root of the solved typology tree\.

```csharp
public DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel? Root { get; set; }
```

#### Property Value
[TypologyTreeNodeViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel'></a>

## TypologyBuildingViewModel Class

One building entry of the Typology area view: the reference that identifies it in the GIS Web API, the county part it was fetched from, and the path of the typology bucket it was solved into\.

Coordinates are intentionally absent: the dot positions come from the area-scoped centroid endpoint in `DiGi.GIS.WebAPI`, joined in the view by `(Reference, CountyId)`. The [Path](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.Path 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingViewModel\.Path') is the filing index chain of the deepest typology bucket holding the building, one integer per level - a leaf's path, or a bucket's own path for a row its next level dropped - which the view uses to locate the node in the tree for the centroid join, the dimming and the grid.

The [Id](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.Id 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingViewModel\.Id') is the building's database identifier, read from the building data table's `Database Id` column. It serves the links to the 2D details page and the 3D viewer, which address a building by that identifier - it takes no part in the centroid join.

```csharp
public class TypologyBuildingViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyBuildingViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.TypologyBuildingViewModel()'></a>

## TypologyBuildingViewModel\(\) Constructor

Initializes a new instance of the [TypologyBuildingViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingViewModel') class\.

```csharp
public TypologyBuildingViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.TypologyBuildingViewModel(string,long,int,System.Collections.Generic.List_int_)'></a>

## TypologyBuildingViewModel\(string, long, int, List\<int\>\) Constructor

Initializes a new instance of the [TypologyBuildingViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyBuildingViewModel') class\.

```csharp
public TypologyBuildingViewModel(string reference, long id, int countyId, System.Collections.Generic.List<int> path);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.TypologyBuildingViewModel(string,long,int,System.Collections.Generic.List_int_).reference'></a>

`reference` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The building reference key\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.TypologyBuildingViewModel(string,long,int,System.Collections.Generic.List_int_).id'></a>

`id` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

The building's database identifier, or 0 when unknown\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.TypologyBuildingViewModel(string,long,int,System.Collections.Generic.List_int_).countyId'></a>

`countyId` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The county part identifier the building was fetched from\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.TypologyBuildingViewModel(string,long,int,System.Collections.Generic.List_int_).path'></a>

`path` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

The typology path, one filing index per level\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.CountyId'></a>

## TypologyBuildingViewModel\.CountyId Property

Gets the county part identifier the building was fetched from\.

```csharp
public int CountyId { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.Id'></a>

## TypologyBuildingViewModel\.Id Property

Gets the building's database identifier \- the `Building2DReference.Id` the 2D details and the 3D viewer routes address \- or 0 when the column was not projected or the row carried none\.

```csharp
public long Id { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.Path'></a>

## TypologyBuildingViewModel\.Path Property

Gets the typology path, one filing index per level, identifying the deepest bucket the building was solved into\.

```csharp
public System.Collections.Generic.List<int> Path { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyBuildingViewModel.Reference'></a>

## TypologyBuildingViewModel\.Reference Property

Gets the building reference key, as addressed by the GIS Web API\.

```csharp
public string Reference { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel'></a>

## TypologyChildAreaViewModel Class

One area of `GET /typology/childareas`: an area one level below one too large for a Typology solve, which the area view offers to open instead \(DiGi\.GIS\.WebAPI\.UI\#29, B2\)\.

```csharp
public class TypologyChildAreaViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyChildAreaViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel()'></a>

## TypologyChildAreaViewModel\(\) Constructor

Initializes a new instance of the [TypologyChildAreaViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyChildAreaViewModel') class\.

```csharp
public TypologyChildAreaViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel(int,string,string,int,System.Nullable_long_)'></a>

## TypologyChildAreaViewModel\(int, string, string, int, Nullable\<long\>\) Constructor

Initializes a new instance of the [TypologyChildAreaViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyChildAreaViewModel') class\.

```csharp
public TypologyChildAreaViewModel(int id, string? code, string? name, int administrativeArealType, System.Nullable<long> count);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel(int,string,string,int,System.Nullable_long_).id'></a>

`id` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The identifier the area view opens the area with \- the lowest of its polygon parts\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel(int,string,string,int,System.Nullable_long_).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The TERYT code of the area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel(int,string,string,int,System.Nullable_long_).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel(int,string,string,int,System.Nullable_long_).administrativeArealType'></a>

`administrativeArealType` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The administrative level of the area, as the integer the area view carries\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.TypologyChildAreaViewModel(int,string,string,int,System.Nullable_long_).count'></a>

`count` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The number of buildings of the area, or null when it is not counted\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.AdministrativeArealType'></a>

## TypologyChildAreaViewModel\.AdministrativeArealType Property

Gets the administrative level of the area, as the integer of `AdministrativeArealType` the area view carries in its query string\.

```csharp
public int AdministrativeArealType { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.Code'></a>

## TypologyChildAreaViewModel\.Code Property

Gets the TERYT code of the area\.

```csharp
public string? Code { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.Count'></a>

## TypologyChildAreaViewModel\.Count Property

Gets the number of buildings of the area: exact for a county \(the sum over its parts\), null for a voivodeship \(not counted\) or when the count could not be read\.

```csharp
public System.Nullable<long> Count { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.Id'></a>

## TypologyChildAreaViewModel\.Id Property

Gets the identifier the area view opens the area with: the lowest of its polygon parts, since a solve resolves every part from the code\.

```csharp
public int Id { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyChildAreaViewModel.Name'></a>

## TypologyChildAreaViewModel\.Name Property

Gets the name of the area\.

```csharp
public string? Name { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel'></a>

## TypologyColumnViewModel Class

Represents a single building\-data column available for typology grouping, as listed in the Available Columns section\.

```csharp
public class TypologyColumnViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyColumnViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.TypologyColumnViewModel()'></a>

## TypologyColumnViewModel\(\) Constructor

Initializes a new instance of the [TypologyColumnViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyColumnViewModel') class\.

```csharp
public TypologyColumnViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.TypologyColumnViewModel(DiGi.PostgreSQL.Table.Classes.Column)'></a>

## TypologyColumnViewModel\(Column\) Constructor

Initializes a new instance of the [TypologyColumnViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyColumnViewModel') class from a deployed GIS Web API [DiGi\.PostgreSQL\.Table\.Classes\.Column](https://learn.microsoft.com/en-us/dotnet/api/digi.postgresql.table.classes.column 'DiGi\.PostgreSQL\.Table\.Classes\.Column')\.

```csharp
public TypologyColumnViewModel(DiGi.PostgreSQL.Table.Classes.Column column);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.TypologyColumnViewModel(DiGi.PostgreSQL.Table.Classes.Column).column'></a>

`column` [DiGi\.PostgreSQL\.Table\.Classes\.Column](https://learn.microsoft.com/en-us/dotnet/api/digi.postgresql.table.classes.column 'DiGi\.PostgreSQL\.Table\.Classes\.Column')

The source column\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.Category'></a>

## TypologyColumnViewModel\.Category Property

Gets the category grouping of the column\.

```csharp
public string? Category { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.DataType'></a>

## TypologyColumnViewModel\.DataType Property

Gets the integer data type value \(0 Undefined through 14 String\)\.

```csharp
public int DataType { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.Description'></a>

## TypologyColumnViewModel\.Description Property

Gets the description of the column\.

```csharp
public string? Description { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.IsNumeric'></a>

## TypologyColumnViewModel\.IsNumeric Property

Gets a value indicating whether the column is numeric and eligible for a Range rule\.

```csharp
public bool IsNumeric { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.Name'></a>

## TypologyColumnViewModel\.Name Property

Gets the display name of the column\.

```csharp
public string? Name { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyColumnViewModel.UniqueId'></a>

## TypologyColumnViewModel\.UniqueId Property

Gets the unique identifier \(slug\) of the column, as addressed by the deployed GIS Web API\.

```csharp
public string? UniqueId { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel'></a>

## TypologyTreeNodeViewModel Class

One node of the solved Typology tree as rendered by the area view: the bucket name, description, color and children\.

The [Color](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Color 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel\.Color') is a CSS hex string (`#rrggbb`) read from the node's `TypologyAppearance` via [Color\(this TypologyAppearance\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.Color(thisDiGi.Typology.Visual.Classes.TypologyAppearance) 'DiGi\.GIS\.WebAPI\.UI\.Query\.Color\(this DiGi\.Typology\.Visual\.Classes\.TypologyAppearance\)'), not a `System.Drawing.Color` — the view paints CSS, and a serialized color type would add a dependency the page does not need.

The [Path](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Path 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel\.Path') is the filing index chain of this node, one integer per level from the root. The view uses it to locate the node in the tree for the centroid join with the building dot positions.

The [Count](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Count 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel\.Count') is the number of buildings filed under this node (issue #24). A bucket node counts its own reference set - every row that matched it, including rows that resolved to no bucket at a lower level - so a parent's count can exceed the sum of its children's counts; the root, which the solver never files references on, counts the sum of its children. Every building the count includes is listed in the flat building list under this node or one below it, so the count always equals the buildings the view can show for the node.

```csharp
public class TypologyTreeNodeViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyTreeNodeViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel()'></a>

## TypologyTreeNodeViewModel\(\) Constructor

Initializes a new instance of the [TypologyTreeNodeViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel') class\.

```csharp
public TypologyTreeNodeViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_)'></a>

## TypologyTreeNodeViewModel\(string, string, string, List\<int\>, int, List\<TypologyTreeNodeViewModel\>\) Constructor

Initializes a new instance of the [TypologyTreeNodeViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel') class\.

```csharp
public TypologyTreeNodeViewModel(string? name, string? description, string? color, System.Collections.Generic.List<int> path, int count, System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel>? children);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The bucket name, as the solver named it\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_).description'></a>

`description` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The bucket description, or null when the column carries none\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_).color'></a>

`color` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The bucket color as a CSS hex string, or null when the rule maps none for this bucket\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_).path'></a>

`path` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

The typology path, one filing index per level from the root\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of buildings filed under this node\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.TypologyTreeNodeViewModel(string,string,string,System.Collections.Generic.List_int_,int,System.Collections.Generic.List_DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel_).children'></a>

`children` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyTreeNodeViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

The sub\-typology nodes in ascending bucket value, or null when this node is a leaf\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Children'></a>

## TypologyTreeNodeViewModel\.Children Property

Gets the sub\-typology nodes in ascending bucket value \- a range by its Min, a unique value by the value itself \- or null when this node is a leaf\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel>? Children { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyTreeNodeViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyTreeNodeViewModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Color'></a>

## TypologyTreeNodeViewModel\.Color Property

Gets the bucket color as a CSS hex string \(`#rrggbb`\), or null when the rule maps no appearance for this bucket\.

```csharp
public string? Color { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Count'></a>

## TypologyTreeNodeViewModel\.Count Property

Gets the number of buildings filed under this node: a bucket's own reference set \(every row that matched it, including rows dropped at a lower level, so a parent can exceed the sum of its children\), or the sum of the children for the root\.

```csharp
public int Count { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Description'></a>

## TypologyTreeNodeViewModel\.Description Property

Gets the bucket description, the column's description when the column carries one\.

```csharp
public string? Description { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Name'></a>

## TypologyTreeNodeViewModel\.Name Property

Gets the bucket name, as the solver named it: the level's column name and the rule data's text\.

```csharp
public string? Name { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyTreeNodeViewModel.Path'></a>

## TypologyTreeNodeViewModel\.Path Property

Gets the typology path, one filing index per level from the root\.

```csharp
public System.Collections.Generic.List<int> Path { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel'></a>

## TypologyViewViewModel Class

Represents the administrative\-area context the Typology Load modal redirected with: the area the colour\-coded building typology is solved for, shown in the Administrative Area card of the area view\.

```csharp
public class TypologyViewViewModel
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyViewViewModel
### Constructors

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.TypologyViewViewModel()'></a>

## TypologyViewViewModel\(\) Constructor

Initializes a new instance of the [TypologyViewViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyViewViewModel') class\.

```csharp
public TypologyViewViewModel();
```

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.TypologyViewViewModel(int,string,DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType,string)'></a>

## TypologyViewViewModel\(int, string, AdministrativeArealType, string\) Constructor

Initializes a new instance of the [TypologyViewViewModel](DiGi.GIS.WebAPI.UI.ViewModels.md#DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel 'DiGi\.GIS\.WebAPI\.UI\.ViewModels\.TypologyViewViewModel') class from the query context of the Load modal redirect\.

```csharp
public TypologyViewViewModel(int id, string? code, DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType administrativeArealType, string? name);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.TypologyViewViewModel(int,string,DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType,string).id'></a>

`id` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The unique identifier of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.TypologyViewViewModel(int,string,DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType,string).code'></a>

`code` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The code of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.TypologyViewViewModel(int,string,DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType,string).administrativeArealType'></a>

`administrativeArealType` [DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')

The type of the administrative area\.

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.TypologyViewViewModel(int,string,DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType,string).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the administrative area, or null when it could not be read\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.AdministrativeArealType'></a>

## TypologyViewViewModel\.AdministrativeArealType Property

Gets the type of the administrative area\.

```csharp
public DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType AdministrativeArealType { get; }
```

#### Property Value
[DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.AdministrativeArealTypeName'></a>

## TypologyViewViewModel\.AdministrativeArealTypeName Property

Gets the display name of the administrative area type\.

```csharp
public string AdministrativeArealTypeName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.Code'></a>

## TypologyViewViewModel\.Code Property

Gets the code of the administrative area\.

```csharp
public string? Code { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.Id'></a>

## TypologyViewViewModel\.Id Property

Gets the unique identifier of the administrative area\.

```csharp
public int Id { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.ViewModels.TypologyViewViewModel.Name'></a>

## TypologyViewViewModel\.Name Property

Gets the name of the administrative area, read from the GIS Web API by the area identifier, or null when the lookup answered nothing \- the page then shows the context it carries on the query alone\.

```csharp
public string? Name { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

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