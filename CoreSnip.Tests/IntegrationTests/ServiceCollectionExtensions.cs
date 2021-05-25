using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CoreSnip.Tests.IntegrationTests
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Removes all registered service of <see cref="TService"/> with the specified lifetime and adds a new registration which uses the <see cref="Func{IServiceProvider, TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service interface which needs to be replaced.</typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime">The lifetime to use to find and replace the service</param>
        /// <param name="implementationFactory">The implementation factory for the specified type.</param>
        private static void SwapService<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime, Func<IServiceProvider, TService> implementationFactory)
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == serviceLifetime))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == serviceLifetime).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }
            services.Add(new ServiceDescriptor(typeof(TService), (sp) => implementationFactory(sp), serviceLifetime));
        }

        /// <summary>
        /// Removes all registered <see cref="ServiceLifetime.Scoped"/> registrations of <see cref="TService"/> and adds a new registration which uses the <see cref="Func{IServiceProvider, TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service interface which needs to be placed.</typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory">The implementation factory for the specified type.</param>
        public static void SwapScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            services.SwapService<TService>(ServiceLifetime.Scoped, implementationFactory);
        }

        /// <summary>
        /// Removes all registered <see cref="ServiceLifetime.Singleton"/> registrations of <see cref="TService"/> and adds a new registration which uses the <see cref="Func{IServiceProvider, TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service interface which needs to be placed.</typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory">The implementation factory for the specified type.</param>
        public static void SwapSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            services.SwapService<TService>(ServiceLifetime.Singleton, implementationFactory);
        }

        /// <summary>
        /// Removes all registered <see cref="ServiceLifetime.Transient"/> registrations of <see cref="TService"/> and adds a new registration which uses the <see cref="Func{IServiceProvider, TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service interface which needs to be placed.</typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory">The implementation factory for the specified type.</param>
        public static void SwapTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            services.SwapService<TService>(ServiceLifetime.Transient, implementationFactory);
        }
    }
}
