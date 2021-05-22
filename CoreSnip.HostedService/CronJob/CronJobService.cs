using Cronos;
using System;
using System.Threading.Tasks;

namespace CoreSnip.HostedService.CronJob
{
    /// <summary>
    /// A <see cref="BackgroundService"/> to execute a job based on a Cron schedule
    /// </summary>
    public abstract class CronJobService : ScheduledJobService
    {
        private readonly CronExpression expression;
        private readonly TimeZoneInfo timeZoneInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="CronJobService"/> class.
        /// </summary>
        /// <param name="cronExpression">A string rappresenting the Cron schedule for the execution of the job specified in the <see cref="ExecuteJob"/> method.</param>
        /// <param name="timeZoneInfo">The <see cref="System.TimeZoneInfo"/> to determine the occurrences of the scheduled job. <see cref="TimeZoneInfo.Local"/> when no value provided.</param>
        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo = null)
        {
            expression = cronExpression.Trim().Split(" ").Length == 5 ? CronExpression.Parse(cronExpression) : CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
            this.timeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local;
        }

        protected override Task<DateTimeOffset?> GetNextOccurrenceAsync(DateTimeOffset fromDate)
        {
            var next = expression.GetNextOccurrence(DateTimeOffset.Now, timeZoneInfo);
            return Task.FromResult(next);
        }
    }
}
