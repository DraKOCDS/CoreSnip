using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreSnip.HostedService
{
    /// <summary>
    /// A <see cref="BackgroundService"/> to execute a job based on a schedule
    /// </summary>
    public abstract class ScheduledJobService : BackgroundService
    {
        private System.Timers.Timer timer;

        /// <summary>
        /// Base <see cref="ExecuteAsync"/> implementation to run the <see cref="ExecuteJob"/> method at the right occurrence.
        /// </summary>
        /// <param name="stoppingToken"></param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var next = await GetNextOccurrenceAsync(DateTimeOffset.Now);
            if (next.HasValue && !stoppingToken.IsCancellationRequested)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ExecuteAsync(stoppingToken);
                }

                //refer to https://docs.microsoft.com/en-us/dotnet/standard/threading/timers for Timers
                timer = new System.Timers.Timer(delay.TotalMilliseconds)
                {
                    AutoReset = false
                };
                timer.Elapsed += async (sender, args) => await OnElapsedTimer(stoppingToken);
                timer.Start();
            }
            await Task.CompletedTask;
        }

        private async Task OnElapsedTimer(CancellationToken stoppingToken)
        {
            timer.Dispose();  // reset and dispose timer
            timer = null;

            if (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteJob(stoppingToken);
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteAsync(stoppingToken);    // reschedule next
            }
        }

        /// <summary>
        /// Provide the implementation of the job to execute.
        /// </summary>
        /// <param name="stoppingToken"></param>
        protected abstract Task ExecuteJob(CancellationToken stoppingToken);

        /// <summary>
        /// Get the next date time the job need to run
        /// </summary>
        /// <param name="fromDate">The start date to calculate the next occurrence</param>
        /// <returns>The next <see cref="DateTimeOffset"/> the job need to run</returns>
        protected abstract Task<DateTimeOffset?> GetNextOccurrenceAsync(DateTimeOffset fromDate);

        /// <summary>
        /// Stop the background service
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Stop();
            await base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Dispose the background service
        /// </summary>
        public override void Dispose()
        {
            timer?.Dispose();
            base.Dispose();
        }
    }
}
