using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreSnip.HostedService.CronJob
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension to register <see cref="CronJobService"/>
    /// </summary>
    public static class CronJobServiceExtensions
    {
        /// <summary>
        /// Adds an <see cref="IHostedService"/> for the given <see cref="CronJobService"/> implementation and a singleton service of the related <see cref="ICronJobConfig{T}"/>
        /// </summary>
        /// <typeparam name="T">The class that define a <see cref="CronJobService"/></typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="options">The <see cref="ICronJobConfig{T}"/> options to configure the <see cref="CronJobService"/> with.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddCronJobService<T>(this IServiceCollection services, Action<ICronJobConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Cron job Configurations.");
            }
            var config = new CronJobConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(CronJobConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }
            var cronFields = config.CronExpression.Trim().Split(" ");
            if (cronFields.Length < 5 || cronFields.Length > 6)
            {
                throw new ArgumentException(nameof(CronJobConfig<T>.CronExpression), @"Only Standard Cron Expression are supported. Consisting of 5 or 6 fields: second (optional), minute, hour, day of month, month, day of week. See more: https://github.com/HangfireIO/Cronos");
            }

            services.AddSingleton<ICronJobConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
