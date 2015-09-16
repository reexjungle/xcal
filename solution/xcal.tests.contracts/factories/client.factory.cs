using System;
using System.Threading;
using ServiceStack.Service;

namespace reexjungle.xcal.tests.contracts.factories
{
    /// <summary>
    /// Specifies a service contract for managing remote service clients
    /// </summary>
    public interface IServiceClientFactory
    {
        /// <summary>
        /// Registers the constructor for a service client
        /// </summary>
        /// <typeparam name="TClient">The type of remote service client to be registered.</typeparam>
        /// <param name="ctor">Constructor of the remote service client.</param>
        void Register<TClient>(Func<TClient> ctor) where TClient : IServiceClient;

        /// <summary>
        /// Gets remote service client from cache or creates a new one.
        /// </summary>
        /// <typeparam name="TClient">The type of remote service client to be retrieved or created.</typeparam>
        /// <returns>The retrieved or created remote service client. </returns>
        TClient GetClient<TClient>() where TClient : IServiceClient;

        /// <summary>
        /// Resets the service by removing all created weremoteb service clients from cache.
        /// </summary>
        void Reset();
    }
}