using ServiceStack.Service;

namespace reexjungle.xcal.test.server.integration.contracts
{
    /// <summary>
    /// Specifies a contract for all service interface integration tests from service clients
    /// </summary>
    /// <typeparam name="TServiceClient">The type of the service client.</typeparam>
    public interface IServiceTestFactory<T>
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
        /// Gets or creates a service client.
        /// </summary>
        /// <returns></returns>
        T GetClient();
    }
}