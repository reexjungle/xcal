using reexjungle.xcal.core.domain.contracts.models.values;
using Xunit;

namespace xcal.core.domain.tests.units.values
{
    public class DatetimeTimeTests
    {
        [Fact]
        public void TestWriteDatetime()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2, 3);
            Assert.Equal(datetime.ToString(), "19970714T010203");
        }

        [Fact]
        public void TestReadDatetime()
        {
            var datetime = new DATE_TIME("19970714T010203");
            Assert.Equal(datetime, new DATE_TIME(1997, 7, 14,1,2,3));
        }


        [Fact]
        public void TestAddSeconds()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var successor = datetime.AddSeconds(1);
            Assert.Equal(successor, new DATE_TIME(1997, 7, 14, 1, 2, 4));
        }


        [Fact]
        public void TestSubtractSeconds()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var predecessor = datetime.AddSeconds(-1);
            Assert.Equal(predecessor, new DATE_TIME(1997, 7, 14, 1, 2, 2));
        }


        [Fact]
        public void TestAddMinutes()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var successor = datetime.AddMinutes(1);
            Assert.Equal(successor, new DATE_TIME(1997, 7, 14, 1, 3, 3));
        }


        [Fact]
        public void TestSubtractMinutes()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var predecessor = datetime.AddMinutes(-1);
            Assert.Equal(predecessor, new DATE_TIME(1997, 7, 14, 1, 1, 3));
        }


        [Fact]
        public void TestAddHours()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var successor = datetime.AddHours(1);
            Assert.Equal(successor, new DATE_TIME(1997, 7, 14, 2, 2, 3));
        }


        [Fact]
        public void TestSubtractHours()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var predecessor = datetime.AddHours(-1);
            Assert.Equal(predecessor, new DATE_TIME(1997, 7, 14, 0, 2, 3));
        }

        [Fact]
        public void TestAddDays()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var successor = datetime.AddDays(1);
            Assert.Equal(successor, new DATE_TIME(1997, 7, 15, 1,2,3));
        }


        [Fact]
        public void TestSubtractDays()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var predecessor = datetime.AddDays(-1);
            Assert.Equal(predecessor, new DATE_TIME(1997, 7, 13, 1,2,3));
        }


        [Fact]
        public void TestAddWeeks()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1 , 2, 3);
            var successor = datetime.AddWeeks(1);
            Assert.Equal(successor, new DATE_TIME(1997, 7, 21, 1, 2, 3));
        }


        [Fact]
        public void TestSubtractWeeks()
        {
            var datetime = new DATE_TIME(1997, 7, 14,1,2,3);
            var predecessor = datetime.AddWeeks(-1);
            Assert.Equal(predecessor, new DATE_TIME(1997, 7, 7, 1, 2, 3));
        }


        [Fact]
        public void TestAddMonths()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var successor = datetime.AddMonths(1);
            Assert.Equal(successor, new DATE_TIME(1997, 8, 14, 1, 2, 3));
        }


        [Fact]
        public void TestSubtractMonths()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2, 3);
            var predecessor = datetime.AddMonths(-1);
            Assert.Equal(predecessor, new DATE_TIME(1997, 6, 14, 1,2, 3));
        }

        [Fact]
        public void TestAddYears()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2,3);
            var successor = datetime.AddYears(2);
            Assert.Equal(successor, new DATE_TIME(1999, 7, 14,1,2,3));
        }


        [Fact]
        public void TestSubtractYears()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2,3);
            var predecessor = datetime.AddYears(-2);
            Assert.Equal(predecessor, new DATE_TIME(1995, 7, 14, 1,2, 3));
        }

        [Fact]
        public void TestAddDuration()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2,3);
            Assert.Equal(datetime + new DURATION(5, 4, 3, 2, 1), new DATE_TIME(1997, 8, 22, 4,4,4));
        }

        [Fact]
        public void TestSubtractDuration()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(datetime - duration, new DATE_TIME(1997, 6, 4, 22, 0, 2));
        }


        [Fact]
        public void TestSubtractDatetime()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var other = new DATE_TIME(2000, 9, 17, 3, 2, 1);
            Assert.Equal(other - datetime, new DURATION(165, 6, 1,59,58));
        }

        [Fact]
        public void TestCompareSameDatetimes()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var other = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            Assert.Equal(datetime == other, true);
        }

        [Fact]
        public void TestCompareDifferentDatetimes()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var other = new DATE_TIME(2000, 9, 17, 3,2,1);
            Assert.Equal(datetime != other, true);
        }

        [Fact]
        public void TestEarlierDatetime()
        {
            var datetime = new DATE_TIME(1997, 7, 14,1, 2, 3);
            var other = new DATE_TIME(2000, 9, 17, 3,2,1);
            Assert.Equal(datetime < other, true);
        }

        [Fact]
        public void TestLaterDatetime()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2,3);
            var other = new DATE_TIME(2000, 9, 17, 3,2,1);
            Assert.Equal(other > datetime, true);
        }

        [Fact]
        public void TestEarlierOrEqualDatetime()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2,3);
            var other = new DATE_TIME(2000, 9, 17,3,2,1);
            Assert.Equal(datetime <= other, true);
        }

        [Fact]
        public void TestLaterOrEqualDatetime()
        {
            var datetime = new DATE_TIME(1997, 7, 14, 1,2,3);
            var other = new DATE_TIME(1997, 7, 14, 3,2,1);
            Assert.Equal(other >= datetime, true);
        }
    }
}
