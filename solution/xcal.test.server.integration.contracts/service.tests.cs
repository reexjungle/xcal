using Funq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.test.server.integration.contracts
{
    public interface IWebServiceTests
    {
        void Initialize();

        void TearDown();
    }

    public interface IMockServiceTests
    {
        Container Container { get; }

        void Configure();
    }
}