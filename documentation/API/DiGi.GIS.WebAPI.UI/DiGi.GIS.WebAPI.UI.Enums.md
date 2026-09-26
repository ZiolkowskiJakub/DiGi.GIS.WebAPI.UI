#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.Enums Namespace
### Enums

<a name='DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus'></a>

## SolarJobStatus Enum

The state of a background solar radiation job \([SolarJob](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.SolarJob 'DiGi\.GIS\.WebAPI\.UI\.Classes\.SolarJob')\)\.

```csharp
public enum SolarJobStatus
```
### Fields

<a name='DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus.Queued'></a>

`Queued` 0

Waiting in the queue, or for the solve gate the job shares with the synchronous requests\.

<a name='DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus.Running'></a>

`Running` 1

Being calculated\.

<a name='DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus.Completed'></a>

`Completed` 2

Calculated; its results can be fetched until the job expires\.

<a name='DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus.Failed'></a>

`Failed` 3

The calculation threw or produced no result; the job carries the error text\.

<a name='DiGi.GIS.WebAPI.UI.Enums.SolarJobStatus.Cancelled'></a>

`Cancelled` 4

Cancelled by the client: a queued job is never calculated, and the results of a running one are discarded\.