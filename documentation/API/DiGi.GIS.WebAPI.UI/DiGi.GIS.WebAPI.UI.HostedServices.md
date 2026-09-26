#### [DiGi\.GIS\.WebAPI\.UI](DiGi.GIS.WebAPI.UI.Overview.md 'DiGi\.GIS\.WebAPI\.UI\.Overview')

## DiGi\.GIS\.WebAPI\.UI\.HostedServices Namespace
### Classes

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService'></a>

## SolarJobHostedService Class

The single consumer of the background solar radiation jobs \([SolarJobQueue](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue 'DiGi\.GIS\.WebAPI\.UI\.Classes\.SolarJobQueue')\): runs them one after the other, in the order they were queued, behind the solve gate it shares with the synchronous requests of `SolarController` \(DiGi\.GIS\.WebAPI\.UI\#60\)\.

A framework type, like a controller: ASP.NET Core runs a background loop only through [Microsoft\.Extensions\.Hosting\.BackgroundService](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.backgroundservice 'Microsoft\.Extensions\.Hosting\.BackgroundService'). It holds the loop and nothing else; the job logic is `Modify.SolveAsync`. The consumer runs on this host, the web server: see `Coding - Deployed WebAPI.md` section 5 and the decision recorded on DiGi.GIS.WebAPI.UI#60.

```csharp
public class SolarJobHostedService : Microsoft.Extensions.Hosting.BackgroundService
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [Microsoft\.Extensions\.Hosting\.BackgroundService](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.backgroundservice 'Microsoft\.Extensions\.Hosting\.BackgroundService') → SolarJobHostedService
### Constructors

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService.SolarJobHostedService(DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue,System.Threading.SemaphoreSlim,Microsoft.Extensions.Logging.ILogger_DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService_)'></a>

## SolarJobHostedService\(SolarJobQueue, SemaphoreSlim, ILogger\<SolarJobHostedService\>\) Constructor

Initializes a new instance of the [SolarJobHostedService](DiGi.GIS.WebAPI.UI.HostedServices.md#DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService 'DiGi\.GIS\.WebAPI\.UI\.HostedServices\.SolarJobHostedService') class\.

```csharp
public SolarJobHostedService(DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue solarJobQueue, System.Threading.SemaphoreSlim semaphoreSlim, Microsoft.Extensions.Logging.ILogger<DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService> logger);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService.SolarJobHostedService(DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue,System.Threading.SemaphoreSlim,Microsoft.Extensions.Logging.ILogger_DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService_).solarJobQueue'></a>

`solarJobQueue` [SolarJobQueue](DiGi.GIS.WebAPI.UI.Classes.md#DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue 'DiGi\.GIS\.WebAPI\.UI\.Classes\.SolarJobQueue')

The queue of jobs to consume\.

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService.SolarJobHostedService(DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue,System.Threading.SemaphoreSlim,Microsoft.Extensions.Logging.ILogger_DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService_).semaphoreSlim'></a>

`semaphoreSlim` [System\.Threading\.SemaphoreSlim](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim 'System\.Threading\.SemaphoreSlim')

The gate shared by every solar radiation solve on this host, registered under [SolarSolveGateKey](DiGi.GIS.WebAPI.UI.Constants.md#DiGi.GIS.WebAPI.UI.Constants.Default.SolarSolveGateKey 'DiGi\.GIS\.WebAPI\.UI\.Constants\.Default\.SolarSolveGateKey')\.

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService.SolarJobHostedService(DiGi.GIS.WebAPI.UI.Classes.SolarJobQueue,System.Threading.SemaphoreSlim,Microsoft.Extensions.Logging.ILogger_DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService_).logger'></a>

`logger` [Microsoft\.Extensions\.Logging\.ILogger&lt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger-1 'Microsoft\.Extensions\.Logging\.ILogger\`1')[SolarJobHostedService](DiGi.GIS.WebAPI.UI.HostedServices.md#DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService 'DiGi\.GIS\.WebAPI\.UI\.HostedServices\.SolarJobHostedService')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger-1 'Microsoft\.Extensions\.Logging\.ILogger\`1')

The logger receiving one line per job\.
### Methods

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService.ExecuteAsync(System.Threading.CancellationToken)'></a>

## SolarJobHostedService\.ExecuteAsync\(CancellationToken\) Method

Runs every queued job, one at a time, until the application stops\. A failure of one job is logged and never ends the loop: an exception escaping here would stop the whole application\.

```csharp
protected override System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken stoppingToken);
```
#### Parameters

<a name='DiGi.GIS.WebAPI.UI.HostedServices.SolarJobHostedService.ExecuteAsync(System.Threading.CancellationToken).stoppingToken'></a>

`stoppingToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Signalled when the application stops; a running calculation cannot be interrupted and finishes first\.

#### Returns
[System\.Threading\.Tasks\.Task](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task 'System\.Threading\.Tasks\.Task')  
A task that completes when the application stops\.