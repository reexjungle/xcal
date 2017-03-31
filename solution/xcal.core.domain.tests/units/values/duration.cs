using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using reexjungle.xcal.core.domain.contracts.models.values;
using Xunit;

namespace xcal.core.domain.tests.units.values
{
    public class DurationTests
    {
        [Fact]
        public void TestWriteDuration()
        {
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(duration.ToString(), "P5W4DT3H2M1S");

        }

        [Fact]
        public void TestReadDuration()
        {
            var duration = new DURATION("P5W4DT3H2M1S");
            Assert.Equal(duration, new DURATION(5,4,3,2,1));
        }

        [Fact]
        public void TestAddDuration()
        {
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(duration + duration, new DURATION(10, 8, 6, 4, 2));

        }

        [Fact]
        public void TestSubtractDuration()
        {
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(duration - duration, DURATION.Zero);

        }

        [Fact]
        public void TestMultiplyDuration()
        {
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(duration * 2, new DURATION(10, 8, 6, 4, 2));

        }

        [Fact]
        public void TestDivideDuration()
        {
            var duration = new DURATION(10, 8, 6, 4, 2);
            Assert.Equal(duration / 2, new DURATION(5, 4, 3, 2, 1));

        }

        [Fact]
        public void TestNegateDuration()
        {
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(-duration, new DURATION(-5, -4, -3, -2, -1));

        }

        [Fact]
        public void TestNegativityOfDuration()
        {
            var duration = new DURATION(0, - 4, 3, 2, 1);
            Assert.Equal(duration.IsNegative(), true);

        }
    }
}
