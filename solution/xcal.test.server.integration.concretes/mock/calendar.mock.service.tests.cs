using Funq;
using reexjungle.xcal.test.server.integration.contracts;

namespace reexjungle.xcal.test.server.integration.concretes.mock
{
    public class CalendarMockServiceTests : IMockServiceIntegrationTests
    {
        private Container container = new Container();

        public CalendarMockServiceTests()
        {
        }

        public Container Container
        {
            get { return this.container; }
        }

        public void Configure()
        {
        }
    }
}