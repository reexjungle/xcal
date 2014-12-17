using Funq;
using reexjungle.xcal.test.units.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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