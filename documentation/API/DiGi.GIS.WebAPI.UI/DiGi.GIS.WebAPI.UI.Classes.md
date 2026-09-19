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

Gets or sets the absolute elevation \(Z coordinate in world coordinates\) of the antenna top in meters\.

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

<a name='DiGi.GIS.WebAPI.UI.Classes.CommunicationCalculationParameter.Frequency'></a>

## CommunicationCalculationParameter\.Frequency Property

Gets or sets the frequency of the propagating electromagnetic wave \[Hz\]\.

The 3D view collects the value in MHz and converts it before sending; when omitted the [DiGi\.Communication\.Classes\.AngularPowerDistributionSolverOptions](https://learn.microsoft.com/en-us/dotnet/api/digi.communication.classes.angularpowerdistributionsolveroptions 'DiGi\.Communication\.Classes\.AngularPowerDistributionSolverOptions') default is used.

```csharp
public System.Nullable<double> Frequency { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

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

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse'></a>

## OrtoDataBuildingResponse Class

The building the Orto Data page is to verify: where to find it, the photo years it holds, and the answer already on record\.

An instance with no [Reference](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Reference 'DiGi\.GIS\.WebAPI\.UI\.Classes\.OrtoDataBuildingResponse\.Reference') is the page's empty state - nothing left to verify - which is also what the relay answers while the upstream endpoints are not deployed. Serialized to the page script with the framework's web defaults, so the property names reach the browser camelCased (`countyId`, `reference`, `years`, `selected`).

```csharp
public class OrtoDataBuildingResponse
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → OrtoDataBuildingResponse
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.CountyId'></a>

## OrtoDataBuildingResponse\.CountyId Property

Gets or sets the identifier of the `building_2d` part the building is filed under, or null when no building was drawn\.

```csharp
public System.Nullable<int> CountyId { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Reference'></a>

## OrtoDataBuildingResponse\.Reference Property

Gets or sets the reference of the building, or null when no building was drawn\.

```csharp
public string? Reference { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selected'></a>

## OrtoDataBuildingResponse\.Selected Property

Gets or sets the answer already recorded for the building, or null when none is\.

```csharp
public DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection? Selected { get; set; }
```

#### Property Value
[Selection](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection 'DiGi\.GIS\.WebAPI\.UI\.Classes\.OrtoDataBuildingResponse\.Selection')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Years'></a>

## OrtoDataBuildingResponse\.Years Property

Gets or sets the years that hold a photo of the building \- the only years a card is rendered for\.

Sorted ascending upstream; the bound answers read their oldest and newest ends.

```csharp
public System.Collections.Generic.List<short> Years { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16 'System\.Int16')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection'></a>

## OrtoDataBuildingResponse\.Selection Class

The year built answer recorded for a building: the year it names, how that year relates to the true construction year, and who recorded it when\.

There is at most one user entry per building - the stored year built data is keyed by source - so the page's selection is a single object rather than a list.

```csharp
public class OrtoDataBuildingResponse.Selection
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Selection
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection.DateTime'></a>

## OrtoDataBuildingResponse\.Selection\.DateTime Property

Gets or sets when the answer was recorded \(UTC\), or null for legacy entries written before provenance was kept\.

```csharp
public System.Nullable<System.DateTimeOffset> DateTime { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset 'System\.DateTimeOffset')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection.Relation'></a>

## OrtoDataBuildingResponse\.Selection\.Relation Property

Gets or sets how [Year](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection.Year 'DiGi\.GIS\.WebAPI\.UI\.Classes\.OrtoDataBuildingResponse\.Selection\.Year') relates to the true construction year, as the integer value of `DiGi.GIS.Enums.YearBuiltRelation`: 0 exact, 1 at or before, 2 after\.

```csharp
public System.Nullable<int> Relation { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection.UserName'></a>

## OrtoDataBuildingResponse\.Selection\.UserName Property

Gets or sets who recorded the answer, or null for legacy entries written before provenance was kept\.

```csharp
public string? UserName { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.OrtoDataBuildingResponse.Selection.Year'></a>

## OrtoDataBuildingResponse\.Selection\.Year Property

Gets or sets the photo year the answer names, or the bound year for an at\-or\-before or after answer\.

```csharp
public System.Nullable<short> Year { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16 'System\.Int16')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter'></a>

## TypologyDefinitionLevelParameter Class

One level of the Typology definition page state: a selected building\-data column, its rule type and the rows of the active rule editor\.

[Name](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.Name 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter\.Name'), [DataType](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.DataType 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter\.DataType') and [IsNumeric](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.IsNumeric 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter\.IsNumeric') describe the column as listed by the deployed GIS Web API; the server fills them from the live column catalog on import and ignores them on export, where the column is identified by [UniqueId](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.UniqueId 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter\.UniqueId') alone.

```csharp
public class TypologyDefinitionLevelParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyDefinitionLevelParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.DataType'></a>

## TypologyDefinitionLevelParameter\.DataType Property

Gets or sets the integer [DiGi\.Core\.Enums\.DataType](https://learn.microsoft.com/en-us/dotnet/api/digi.core.enums.datatype 'DiGi\.Core\.Enums\.DataType') value of the column \(0 Undefined through 14 String\)\.

```csharp
public int DataType { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.IsNumeric'></a>

## TypologyDefinitionLevelParameter\.IsNumeric Property

Gets or sets a value indicating whether the column is numeric and eligible for a range rule\.

```csharp
public bool IsNumeric { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.Name'></a>

## TypologyDefinitionLevelParameter\.Name Property

Gets or sets the display name of the column\.

```csharp
public string? Name { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.Ranges'></a>

## TypologyDefinitionLevelParameter\.Ranges Property

Gets or sets the range rows, read when [RuleType](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.RuleType 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter\.RuleType') names a range rule\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionRangeParameter>? Ranges { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyDefinitionRangeParameter](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionRangeParameter 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionRangeParameter')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.RuleType'></a>

## TypologyDefinitionLevelParameter\.RuleType Property

Gets or sets the rule class name: `VisualIntegerRangeFilterRule`, `VisualDoubleRangeFilterRule` or `VisualUniqueValueFilterRule`\. Null when no rule has been chosen yet\.

```csharp
public string? RuleType { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.UniqueId'></a>

## TypologyDefinitionLevelParameter\.UniqueId Property

Gets or sets the unique identifier \(slug\) of the column, as addressed by the deployed GIS Web API\.

```csharp
public string? UniqueId { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.UniqueValueColors'></a>

## TypologyDefinitionLevelParameter\.UniqueValueColors Property

Gets or sets the unique\-value rows, read when [RuleType](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter.RuleType 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter\.RuleType') names the unique\-value rule\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionUniqueValueParameter>? UniqueValueColors { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyDefinitionUniqueValueParameter](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionUniqueValueParameter 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionUniqueValueParameter')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionParameter'></a>

## TypologyDefinitionParameter Class

The page state of the Typology definition page: the chain of selected building\-data columns, each with its rule type, ranges and colors\.

This is the wire contract of `POST /typology/definition/export` (request) and `POST /typology/definition/validate` (response), serialized camelCase by the application's default JSON options, exactly as `typology.js` holds it. The DiGi document ([DiGi\.Typology\.Visual\.Classes\.VisualColumnTypologyFilter](https://learn.microsoft.com/en-us/dotnet/api/digi.typology.visual.classes.visualcolumntypologyfilter 'DiGi\.Typology\.Visual\.Classes\.VisualColumnTypologyFilter')) is composed and parsed on the server only, so the page never spells a `_type` or a `TypologyAppearanceCollection` key.

```csharp
public class TypologyDefinitionParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyDefinitionParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionParameter.Levels'></a>

## TypologyDefinitionParameter\.Levels Property

Gets or sets the levels of the chain, root first\.

```csharp
public System.Collections.Generic.List<DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter>? Levels { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[TypologyDefinitionLevelParameter](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionLevelParameter 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionLevelParameter')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionRangeParameter'></a>

## TypologyDefinitionRangeParameter Class

One range row of a Typology definition level: a closed interval and the color of the bucket it maps to\.

```csharp
public class TypologyDefinitionRangeParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyDefinitionRangeParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionRangeParameter.Color'></a>

## TypologyDefinitionRangeParameter\.Color Property

Gets or sets the bucket color as the color picker holds it, `#rrggbb`\.

```csharp
public string? Color { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionRangeParameter.Max'></a>

## TypologyDefinitionRangeParameter\.Max Property

Gets or sets the inclusive upper bound\. Null when the row is still being typed\.

```csharp
public System.Nullable<double> Max { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionRangeParameter.Min'></a>

## TypologyDefinitionRangeParameter\.Min Property

Gets or sets the inclusive lower bound\. Null when the row is still being typed\.

```csharp
public System.Nullable<double> Min { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionUniqueValueParameter'></a>

## TypologyDefinitionUniqueValueParameter Class

One unique\-value row of a Typology definition level: a value of the column and the color of the bucket it maps to\.

```csharp
public class TypologyDefinitionUniqueValueParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyDefinitionUniqueValueParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionUniqueValueParameter.Color'></a>

## TypologyDefinitionUniqueValueParameter\.Color Property

Gets or sets the bucket color as the color picker holds it, `#rrggbb`\.

```csharp
public string? Color { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionUniqueValueParameter.Value'></a>

## TypologyDefinitionUniqueValueParameter\.Value Property

Gets or sets the value, a JSON primitive or null for the NULL bucket\. Bound from a request body it arrives as a [System\.Text\.Json\.JsonElement](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement 'System\.Text\.Json\.JsonElement'); read it through [TryConvertValue\(object, DataType, object\)](DiGi.GIS.WebAPI.UI.md#DiGi.GIS.WebAPI.UI.Query.TryConvertValue(object,DiGi.Core.Enums.DataType,object) 'DiGi\.GIS\.WebAPI\.UI\.Query\.TryConvertValue\(object, DiGi\.Core\.Enums\.DataType, object\)'), never by pattern matching on a CLR primitive\.

```csharp
public object? Value { get; set; }
```

#### Property Value
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyHistogramParameter'></a>

## TypologyHistogramParameter Class

The body the `histogramsummary` relay posts to the upstream `gis/BuildingData/histogramsummary` endpoint\.

The property names are the wire contract: the upstream binds its own `HistogramRequestParameter` from the body, and `Query.PostJsonAsync` sends the declared names as-is (`JsonSerializerOptions.Default`), so nothing relies on the receiver binding names case-insensitively (see Coding - WebAPI Contracts, section 2). The upstream's `FilterGroup` is deliberately absent — the definition page never filters a histogram (the county-part scope is #38).

```csharp
public class TypologyHistogramParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologyHistogramParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyHistogramParameter.BucketCount'></a>

## TypologyHistogramParameter\.BucketCount Property

Gets or sets the number of buckets; the relay fixes it to [HistogramBucketCount](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.HistogramBucketCount 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.HistogramBucketCount')\.

```csharp
public int BucketCount { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyHistogramParameter.ColumnUniqueId'></a>

## TypologyHistogramParameter\.ColumnUniqueId Property

Gets or sets the unique identifier \(slug\) of the column to histogram, as listed by the `columns` endpoint\.

```csharp
public string ColumnUniqueId { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyHistogramParameter.CountyId'></a>

## TypologyHistogramParameter\.CountyId Property

Gets or sets the county part identifier scoping the histogram; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') asks for the whole table\.

```csharp
public System.Nullable<int> CountyId { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologyHistogramParameter.HistogramBucketing'></a>

## TypologyHistogramParameter\.HistogramBucketing Property

Gets or sets the bucketing rule, sent as its integer value; the relay asks for [DiGi\.PostgreSQL\.Table\.Enums\.HistogramBucketing\.EqualCount](https://learn.microsoft.com/en-us/dotnet/api/digi.postgresql.table.enums.histogrambucketing.equalcount 'DiGi\.PostgreSQL\.Table\.Enums\.HistogramBucketing\.EqualCount') \(issue \#37\), so every bucket holds the same number of buildings and the Load's quantile bounds follow the buildings rather than the value span\. A host without ZiolkowskiJakub/DiGi\.GIS\.WebAPI\#35 ignores the property and answers equal\-width buckets\.

```csharp
public DiGi.PostgreSQL.Table.Enums.HistogramBucketing HistogramBucketing { get; set; }
```

#### Property Value
[DiGi\.PostgreSQL\.Table\.Enums\.HistogramBucketing](https://learn.microsoft.com/en-us/dotnet/api/digi.postgresql.table.enums.histogrambucketing 'DiGi\.PostgreSQL\.Table\.Enums\.HistogramBucketing')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter'></a>

## TypologySolveParameter Class

The request body of `POST /typology/buildings`: the Typology definition page state to solve, and the administrative area it is solved for\.

The [Definition](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.Definition 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologySolveParameter\.Definition') is the same page state the definition page exports, resolved server-side against the live column catalog before the solve. The [Id](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.Id 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologySolveParameter\.Id'), [Code](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.Code 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologySolveParameter\.Code') and [AdministrativeArealType](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.AdministrativeArealType 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologySolveParameter\.AdministrativeArealType') carry the area context the Load modal selected, and are used to resolve the county part identifiers that scope the building data fetch.

[AdministrativeArealType](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.AdministrativeArealType 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologySolveParameter\.AdministrativeArealType') is nullable on the wire: the `Undefined` sentinel is -1 and not 0, so a non-nullable binding would silently keep `Country` for an omitted parameter (Coding - WebAPI Contracts, section 2). The action rejects [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') and [DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType\.Undefined](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype.undefined 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType\.Undefined') explicitly.

```csharp
public class TypologySolveParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TypologySolveParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.AdministrativeArealType'></a>

## TypologySolveParameter\.AdministrativeArealType Property

Gets or sets the type of the selected administrative area, bound as nullable so an omitted value is refused rather than read as [DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType\.Country](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype.country 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType\.Country')\.

```csharp
public System.Nullable<DiGi.GIS.PostgreSQL.Enums.AdministrativeArealType> AdministrativeArealType { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.postgresql.enums.administrativearealtype 'DiGi\.GIS\.PostgreSQL\.Enums\.AdministrativeArealType')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.Code'></a>

## TypologySolveParameter\.Code Property

Gets or sets the code of the selected administrative area\.

```csharp
public string? Code { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.Definition'></a>

## TypologySolveParameter\.Definition Property

Gets or sets the Typology definition page state: the chain of selected building\-data columns with rule types, ranges and colors\.

```csharp
public DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionParameter? Definition { get; set; }
```

#### Property Value
[TypologyDefinitionParameter](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.TypologyDefinitionParameter 'DiGi\.GIS\.WebAPI\.UI\.Classes\.TypologyDefinitionParameter')

<a name='DiGi.GIS.WebAPI.UI.Classes.TypologySolveParameter.Id'></a>

## TypologySolveParameter\.Id Property

Gets or sets the unique identifier of the selected administrative area, as carried by the Load modal\.

```csharp
public int Id { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.GIS.WebAPI.UI.Classes.UserLoginParameter'></a>

## UserLoginParameter Class

The credentials a visitor submits on the sign\-in page, relayed to the user authentication service\.

These two property names are the wire contract of `POST /user/login` and must match `DiGi.User.Classes.UserLogin`. This application reaches that service over HTTP only, so nothing checks them at compile time and a rename on either side fails silently - diff them by hand whenever either moves (Coding - WebAPI Contracts, section 5).

[Password](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.UserLoginParameter.Password 'DiGi\.GIS\.WebAPI\.UI\.Classes\.UserLoginParameter\.Password') is a secret in transit. It is never logged, never echoed back to the browser and never written into a view.

```csharp
public class UserLoginParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → UserLoginParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.UserLoginParameter.Email'></a>

## UserLoginParameter\.Email Property

Gets or sets the email address identifying the account\.

```csharp
public string? Email { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.UserLoginParameter.Password'></a>

## UserLoginParameter\.Password Property

Gets or sets the password submitted for the account\.

```csharp
public string? Password { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.UserYearBuiltParameter'></a>

## UserYearBuiltParameter Class

The year built answer a reviewer submits on the Orto Data page, relayed to the GIS Web API\.

These property names are the wire contract of `POST gis/yearbuiltdata/setuseryearbuilt` and must match the service's own `UserYearBuiltParameter`. This application reaches that service over HTTP only, so nothing checks them at compile time and a rename on either side fails silently - diff them by hand whenever either moves (Coding - WebAPI Contracts, section 5).

```csharp
public class UserYearBuiltParameter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → UserYearBuiltParameter
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.UserYearBuiltParameter.CountyId'></a>

## UserYearBuiltParameter\.CountyId Property

Gets or sets the identifier of the `building_2d` part the building is filed under\.

Always the part the page received from the random draw or the direct-mode reference resolution, never one re-derived from a county code: a county code can name several parts, and the write is refused when the named part does not hold the reference.

```csharp
public System.Nullable<int> CountyId { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.UserYearBuiltParameter.Reference'></a>

## UserYearBuiltParameter\.Reference Property

Gets or sets the reference of the building the answer is for\.

```csharp
public string? Reference { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.UserYearBuiltParameter.Relation'></a>

## UserYearBuiltParameter\.Relation Property

Gets or sets how [Year](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.UserYearBuiltParameter.Year 'DiGi\.GIS\.WebAPI\.UI\.Classes\.UserYearBuiltParameter\.Year') relates to the true construction year, as the integer value of `DiGi.GIS.Enums.YearBuiltRelation`: 0 exact, 1 at or before, 2 after\.

The integer, never the member name: the service binds the value against its own enum spelling, and a renamed member turns a member-name string into a hard 400 while the integer never moves (Coding - WebAPI Contracts, section 2). The service treats null as 0.

```csharp
public System.Nullable<int> Relation { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.UserYearBuiltParameter.Year'></a>

## UserYearBuiltParameter\.Year Property

Gets or sets the photo year the answer names: the selected card's year, or the oldest or newest listed year for a bound answer\.

```csharp
public System.Nullable<short> Year { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int16](https://learn.microsoft.com/en-us/dotnet/api/system.int16 'System\.Int16')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse'></a>

## WebAPIResponse Class

The outcome of a single request relayed to a Web API: the status it answered with, and the body it carried\.

The other `Query` helpers of this application collapse every failure into [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null'), so that a page assembled from several independent requests survives one of them coming back empty. Authentication, and a user-initiated value load, are the cases that rule does not cover: there the status <em>is</em> the answer. A refused credential (401) has to stay distinct from a faulting authentication service (500) and from a service that could not be reached at all, because collapsing them reports an outage to the visitor as a wrong password and hides it from everyone else; and an empty column - the page's 204 - has to stay distinct from a faulting data service (502) or an unreachable one (503), because collapsing them reads a service failure as a column with no values (issue #40).

```csharp
public class WebAPIResponse
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → WebAPIResponse
### Constructors

<a name='DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse.WebAPIResponse(int,string)'></a>

## WebAPIResponse\(int, string\) Constructor

Initializes a new instance of the [WebAPIResponse](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse 'DiGi\.GIS\.WebAPI\.UI\.Classes\.WebAPIResponse') class\.

```csharp
public WebAPIResponse(int statusCode, string? json=null);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse.WebAPIResponse(int,string).statusCode'></a>

`statusCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The HTTP status code the Web API answered with\.

<a name='DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse.WebAPIResponse(int,string).json'></a>

`json` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The response body, or null when the response carried none\.
### Properties

<a name='DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse.Json'></a>

## WebAPIResponse\.Json Property

Gets the response body, or null when the response carried none\.

```csharp
public string? Json { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.GIS.WebAPI.UI.Classes.WebAPIResponse.StatusCode'></a>

## WebAPIResponse\.StatusCode Property

Gets the HTTP status code the Web API answered with\.

```csharp
public int StatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')