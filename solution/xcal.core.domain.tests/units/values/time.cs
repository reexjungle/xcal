using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.values;
using Xunit;

namespace xcal.core.domain.tests.units.values
{
    public class TimeTests
    {
        [Fact]
        public void TestWriteTime()
        {
            var time = new TIME(1, 2, 3, TIME_FORM.UTC);
            Assert.Equal(time.ToString(), "010203Z");
        }

        [Fact]
        public void TestReadTime()
        {
            var time = new TIME("010203Z");
            Assert.Equal(time, new TIME(1, 2, 3, TIME_FORM.UTC));
        }


        [Fact]
        public void TestAddSeconds()
        {
            var time = new TIME(1, 2, 3);
            var successor = time.AddSeconds(1);
            Assert.Equal(successor, new TIME(1, 2, 4));
        }


        [Fact]
        public void TestSubtractSeconds()
        {
            var time = new TIME(1, 2, 3, TIME_FORM.UTC);
            var predecessor = time.AddSeconds(-1);
            Assert.Equal(predecessor, new TIME(1, 2, 2, TIME_FORM.UTC));
        }


        [Fact]
        public void TestAddMinutes()
        {
            var time = new TIME(0, 1, 2);
            var successor = time.AddMinutes(1);
            Assert.Equal(successor, new TIME(0, 2, 2));
        }


        [Fact]
        public void TestSubtractMinutes()
        {
            var time = new TIME(1, 0, 0);
            var predecessor = time.AddMinutes(-1);
            Assert.Equal(predecessor, new TIME(0, 59, 0));
        }



        [Fact]
        public void TestAddHours()
        {
            var time = new TIME(1, 2, 3);
            var successor = time.AddHours(1);
            Assert.Equal(successor, new TIME(2, 2, 3));
        }


        [Fact]
        public void TestSubtractHours()
        {
            var time = new TIME(1, 2, 3);
            var predecessor = time.AddHours(-1);
            Assert.Equal(predecessor, new TIME(0, 2, 3));
        }

        [Fact]
        public void TestAddDuration()
        {
            var time = new TIME(1, 2, 3);
            Assert.Equal(time + new DURATION(5, 4, 3, 2, 1), new TIME(4, 4, 4));
        }

        [Fact]
        public void TestSubtractDuration()
        {
            var time = new TIME(1, 2, 3);
            var duration = new DURATION(0, 0, 1, 2, 3);
            Assert.Equal(time - duration, new TIME(0, 0, 0));
        }


        [Fact]
        public void TestSubtractTime()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 3, 2, 1);
            Assert.Equal(other - time, new DURATION(0, 0, 1, 59, 58));
        }

        [Fact]
        public void TestCompareSameTimes()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 1, 2, 3);
            Assert.Equal(time == other, true);
        }

        [Fact]
        public void TestCompareDifferentTimes()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 3, 2, 1);
            Assert.Equal(time != other, true);
        }

        [Fact]
        public void TestEarlierTime()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 3, 2, 1);
            Assert.Equal(time < other, true);
        }

        [Fact]
        public void TestLaterTime()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 3, 2, 1);
            Assert.Equal(other > time, true);
        }

        [Fact]
        public void TestEarlierOrEqualTime()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 3, 2, 1);
            Assert.Equal(time <= other, true);
        }

        [Fact]
        public void TestLaterOrEqualTime()
        {
            var time = new TIME( 1, 2, 3);
            var other = new TIME( 3, 2, 1);
            Assert.Equal(other >= time, true);
        }
    }
}
