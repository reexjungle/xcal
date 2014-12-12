using Funq;
using ServiceStack.Service;

namespace reexjungle.xcal.test.server.integration.contracts
{
    /// <summary>
    /// Specifies a contract for all service interface integration tests from service clients
    /// </summary>
    /// <typeparam name="TServiceClient">The type of the service client.</typeparam>
    public interface IServiceTests<T>
        where T : IServiceClient
    {
        /// <summary>
        /// Gets or sets the base URI (Uniform Resoure Identifier) for the connection to the server.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        string BaseUri { get; set; }

        /// <summary>
        /// Creates the service client.
        /// </summary>
        /// <returns></returns>
        T CreateServiceClient();
    }
}