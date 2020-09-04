using Microsoft.Extensions.DependencyInjection;

namespace DerMistkaefer.DvbLive.TriasCommunication.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to add the TriasCommunication library to the .NET Core
    /// dependency injection container.
    /// </summary>
    public static class TriasCommunicationLoader
    {
        /// <summary>
        /// Adds the TriasCommunicator to the dependency injection container.
        /// </summary>
        /// <param name="services">Collection of services for building a dependency injection container.</param>
        public static void AddTriasCommunication(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<ITriasCommunicator, TriasCommunicator>();
        }
    }
}
