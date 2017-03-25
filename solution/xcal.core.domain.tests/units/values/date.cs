using reexjungle.xcal.core.domain.contracts.models.values;
using Xunit;

namespace xcal.core.domain.tests.units.values
{
    public class DateTests
    {
        [Fact]
        public void TestWriteDate()
        {
            var date = new DATE(1997, 7, 14);
            Assert.Equal(date.ToString(), "19970714");
        }

        [Fact]
        public void TestReadDate()
        {
            var date = new DATE("19970714");
            Assert.Equal(date, new DATE(1997, 7, 14));
        }

        [Fact]
        public void TestAddDays()
        {
            var date = new DATE(1997, 7, 14);
            var successor = date.AddDays(1);
            Assert.Equal(successor, new DATE(1997, 7, 15));
        }


        [Fact]
        public void TestSubtractDays()
        {
            var date = new DATE(1997, 7, 14);
            var predecessor = date.AddDays(-1);
            Assert.Equal(predecessor, new DATE(1997, 7, 13));
        }


        [Fact]
        public void TestAddWeeks()
        {
            var date = new DATE(1997, 7, 14);
            var successor = date.AddWeeks(1);
            Assert.Equal(successor, new DATE(1997, 7, 21));
        }


        [Fact]
        public void TestSubtractWeeks()
        {
            var date = new DATE(1997, 7, 14);
            var predecessor = date.AddWeeks(-1);
            Assert.Equal(predecessor, new DATE(1997, 7, 7));
        }


        [Fact]
        public void TestAddMonths()
        {
            var date = new DATE(1997, 7, 14);
            var successor = date.AddMonths(1);
            Assert.Equal(successor, new DATE(1997, 8, 14));
        }


        [Fact]
        public void TestSubtractMonths()
        {
            var date = new DATE(1997, 7, 14);
            var predecessor = date.AddMonths(-1);
            Assert.Equal(predecessor, new DATE(1997, 6, 14));
        }

        [Fact]
        public void TestAddYears()
        {
            var date = new DATE(1997, 7, 14);
            var successor = date.AddYears(2);
            Assert.Equal(successor, new DATE(1999, 7, 14));
        }


        [Fact]
        public void TestSubtractYears()
        {
            var date = new DATE(1997, 7, 14);
            var predecessor = date.AddYears(-2);
            Assert.Equal(predecessor, new DATE(1995, 7, 14));
        }

        [Fact]
        public void TestAddDuration()
        {
            var date = new DATE(1997, 7, 14);
            Assert.Equal(date + new DURATION(5, 4, 3, 2, 1), new DATE(1997, 8, 22));
        }

        [Fact]
        public void TestSubtractDuration()
        {
            var date = new DATE(1997, 7, 14);
            var duration = new DURATION(5, 4, 3, 2, 1);
            Assert.Equal(date - duration, new DATE(1997, 6, 4));
        }


        [Fact]
        public void TestSubtractDate()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(2000, 9, 17);
            Assert.Equal(other - date, new DURATION(165, 6));            
        }

        [Fact]
        public void TestCompareSameDates()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(1997, 7, 14);
            Assert.Equal(date == other, true);               
        }

        [Fact]
        public void TestCompareDifferentDates()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(2000, 9, 17);
            Assert.Equal(date != other, true);
        }

        [Fact]
        public void TestEarlierDate()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(2000, 9, 17);
            Assert.Equal(date < other, true);
        }

        [Fact]
        public void TestLaterDate()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(2000, 9, 17);
            Assert.Equal(other > date, true);
        }

        [Fact]
        public void TestEarlierOrEqualDate()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(2000, 9, 17);
            Assert.Equal(date <= other, true);
        }

        [Fact]
        public void TestLaterOrEqualDate()
        {
            var date = new DATE(1997, 7, 14);
            var other = new DATE(1997, 7, 14);
            Assert.Equal(other >= date, true);
        }

    }
}
