using Funq;
using reexjungle.xcal.test.units.contracts;

namespace reexjungle.xcal.test.server.integration.contracts
{
    public interface IWebServiceIntegrationTests
    {
        void TearDown();
    }

    public interface IMockServiceIntegrationTests
    {
        Container Container { get; }

        void Configure();
    }
}