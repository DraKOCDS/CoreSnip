using System;

namespace CoreSnip.HostedService.CronJob
{
    /// <summary>
    /// Config utils interface for <see cref="CronJobService"/>
    /// </summary>
    /// <typeparam name="T">The type T that inherits <see cref="CronJobService"/></typeparam>
    public interface ICronJobConfig<T>
    {
        /// <summary>
        /// String rappresenting the Cron expression for the schedule.
        /// </summary>
        string CronExpression { get; set; }

        /// <summary>
        /// The time zone to be used to determine the next Cron occurrence
        /// </summary>
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    /// <summary>
    /// Config class for <see cref="CronJobService"/>
    /// </summary>
    /// <typeparam name="T">The type T that inherits <see cref="CronJobService"/></typeparam>
    public class CronJobConfig<T> : ICronJobConfig<T>
    {
        /// <summary>
        /// String rappresenting the Cron expression for the schedule.
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// The time zone to be used to determine the next Cron occurrence
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
