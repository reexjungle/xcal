using System;
using System.Collections.Generic;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.concretes.operations;
using reexjungle.xmisc.infrastructure.contracts;
using ServiceStack.Service;

namespace reexjungle.xcal.tests.concretes.factories
{
    /// <summary>
    ///  Represents a service to manage web service clients
    /// </summary>
    public class ServiceClientFactory : IServiceClientFactory
    {
        private readonly IDictionary<Type, IServiceClient> cache;
        private readonly ISimpleFactory factory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factory">The factory to be used for creating web service clients.</param>
        public ServiceClientFactory()
        {
            this.factory = new Factory();
            cache = new Dictionary<Type, IServiceClient>();
        }

        /// <summary>
        /// Registers the constructor for a service client.
        /// </summary>
        /// <typeparam name="TClient">The type of remote service client to be registered.</typeparam>
        /// <param name="ctor">Constructor of the remote service client.</param>
        public void Register<TClient>(Func<TClient> ctor) where TClient : IServiceClient
        {
            factory.Register(ctor);
        }

        /// <summary>
        /// Gets web service client from cache or creates a new one.
        /// </summary>
        /// <typeparam name="TClient">The type of web service client to be retrieved or created.</typeparam>
        /// <returns>The retrieved or created web service client. </returns>
        public TClient GetClient<TClient>() where TClient : IServiceClient
        {
            var type = typeof(TClient);
            IServiceClient client;
            if (!cache.TryGetValue(type, out client))
            {
                client = factory.Create<TClient>();
                if (client != null) cache.Add(type, client);
            }
            return (TClient)client;
        }

        /// <summary>
        /// Resets the service by removing all created web service clients from cache.
        /// </summary>
        public void Reset()
        {
            cache.Clear();
        }
    }
}