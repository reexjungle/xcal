using reexjungle.xcal.test.server.integration.contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace reexjungle.xcal.test.server.integration.concretes.mock
{
    public class CalendarMockServiceTests : IMockTests
    {
        private Funq.Container container = new Funq.Container();

        public CalendarMockServiceTests()
        {
        }

        public Funq.Container Container
        {
            get { return this.container; }
        }

        public void Configure()
        {
        }
    }
}