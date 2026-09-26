using DiGi.GIS.WebAPI.UI.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.HostedServices
{
    /// <summary>
    /// The single consumer of the background solar radiation jobs (<see cref="SolarJobQueue"/>): runs them one after the other, in the order they were queued, behind the solve gate it shares with the synchronous requests of <c>SolarController</c> (DiGi.GIS.WebAPI.UI#60).
    /// <para>A framework type, like a controller: ASP.NET Core runs a background loop only through <see cref="BackgroundService"/>. It holds the loop and nothing else; the job logic is <c>Modify.SolveAsync</c>. The consumer runs on this host, the web server: see <c>Coding - Deployed WebAPI.md</c> section 5 and the decision recorded on DiGi.GIS.WebAPI.UI#60.</para>
    /// </summary>
    public class SolarJobHostedService : BackgroundService
    {
        private readonly ILogger<SolarJobHostedService> logger;
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly SolarJobQueue solarJobQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolarJobHostedService"/> class.
        /// </summary>
        /// <param name="solarJobQueue">The queue of jobs to consume.</param>
        /// <param name="semaphoreSlim">The gate shared by every solar radiation solve on this host, registered under <see cref="Constants.Default.SolarSolveGateKey"/>.</param>
        /// <param name="logger">The logger receiving one line per job.</param>
        public SolarJobHostedService(SolarJobQueue solarJobQueue, [FromKeyedServices(Constants.Default.SolarSolveGateKey)] SemaphoreSlim semaphoreSlim, ILogger<SolarJobHostedService> logger)
        {
            this.solarJobQueue = solarJobQueue;
            this.semaphoreSlim = semaphoreSlim;
            this.logger = logger;
        }

        /// <summary>
        /// Runs every queued job, one at a time, until the application stops. A failure of one job is logged and never ends the loop: an exception escaping here would stop the whole application.
        /// </summary>
        /// <param name="stoppingToken">Signalled when the application stops; a running calculation cannot be interrupted and finishes first.</param>
        /// <returns>A task that completes when the application stops.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (SolarJob solarJob in solarJobQueue.Channel.Reader.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        await solarJobQueue.SolveAsync(solarJob, semaphoreSlim, message => logger.LogInformation("{Message}", message), stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        return;
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Solar radiation job {JobId} for building {Id} failed outside its calculation.", solarJob.Id, solarJob.BuildingModelId);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Application stopping; queued jobs are lost with it.
            }
        }
    }
}
