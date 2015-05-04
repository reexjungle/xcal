using Funq;
using reexjungle.xcal.test.units.contracts;

namespace reexjungle.xcal.test.server.integration.contracts
{
    public interface IIntegrationTests : ITests { }

    public interface IWebServiceIntegrationTests : IIntegrationTests
    {
        void Initialize();

        void TearDown();
    }

    public interface IMockServiceIntegrationTests : IIntegrationTests
    {
        Container Container { get; }

        void Configure();
    }
}