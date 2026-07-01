#### [DiGi\.GIS\.WebAPI\.UI](index.md 'index')

## DiGi\.GIS\.WebAPI\.UI Namespace
### Classes

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