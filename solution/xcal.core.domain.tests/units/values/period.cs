using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using reexjungle.xcal.core.domain.contracts.models;
using reexjungle.xcal.core.domain.contracts.models.values;
using Xunit;

namespace xcal.core.domain.tests.units.values
{
    public class PeriodTests
    {
        [Fact]
        public void TestWriteExplicitPeriod()
        {
            var start = new DATE_TIME(1997, 01, 01, 18, 0, 0, TIME_FORM.UTC);
            var end = new DATE_TIME(1997, 1, 2, 7, 0, 0, TIME_FORM.UTC);
            var period = new PERIOD(start, end);
            Assert.Equal(period.ToString(), "19970101T180000Z/19970102T070000Z");
        }

        [Fact]
        public void TestWriteStartPeriod()
        {
            var start = new DATE_TIME(1997, 01, 01, 18, 0, 0, TIME_FORM.UTC);
            var duration = new DURATION(0, 0, 5, 30);
            var period = new PERIOD(start, duration);
            Assert.Equal(period.ToString(), "19970101T180000Z/PT5H30M");
        }

        [Fact]
        public void TestReadExplicitPeriod()
        {
            var start = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var end = new DATE_TIME(1997, 7, 15, 1, 2, 3);
            var period = new PERIOD("19970714T010203/19970715T010203");
            Assert.Equal(period, new PERIOD(start, end));
        }

        [Fact]
        public void TestReadStartPeriod()
        {
            var start = new DATE_TIME(1997, 7, 14, 1, 2, 3);
            var duration = new DURATION(0, 1);
            var period = new PERIOD("19970714T010203/P1D");
            Assert.Equal(period, new PERIOD(start, duration));
        }

        [Fact]
        public void TestAddPeriods()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 15, 1, 2, 3), new DURATION(0, 1));
            var other = new PERIOD(new DATE_TIME(1997, 7, 13, 1, 2, 3), new DURATION(0, 1));
            Assert.Equal(other + period, new PERIOD(new DATE_TIME(1997, 7, 13, 1, 2, 3), new DATE_TIME(1997, 7, 16, 1, 2, 3)));
        }


        /// <summary>
        /// Tests the following period subtraction case:
        /// ________ ________
        /// </summary>
        [Fact]
        public void TestSubtractDisjointPeriod()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 1));
            var other = new PERIOD(new DATE_TIME(1997, 7, 15, 1, 2, 3), new DURATION(0, 1));
            Assert.Equal(period - other, new []
            {
                new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DATE_TIME(1997, 7, 13, 1, 2, 3)),
                new PERIOD(new DATE_TIME(1997, 7, 16, 1, 2, 3), new DATE_TIME(1997, 7, 15, 1, 2, 3))
            });

        }

        /// <summary>
        /// Tests the following period subtraction case:
        // ________
        //    ________
        /// </summary>
        [Fact]
        public void TestSubtractPeriodLeftOverlap()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 2));
            var other = new PERIOD(new DATE_TIME(1997, 7, 13, 1, 2, 3), new DURATION(0, 3));
            Assert.Equal(period - other, new []
            {
                new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DATE_TIME(1997, 7, 13, 1, 2, 3)), 
                new PERIOD(new DATE_TIME(1997, 7, 16, 1, 2, 3), new DATE_TIME(1997, 7, 14, 1, 2, 3))                
            });

        }

        /// <summary>
        /// Tests the following period subtraction case:
        // ___________
        //    ________
        /// </summary>
        [Fact]
        public void TestSubtractPeriodLeftOnlyOverlap()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 3));
            var other = new PERIOD(new DATE_TIME(1997, 7, 13, 1, 2, 3), new DURATION(0, 2));
            Assert.Equal(period - other, new []
            {
                new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DATE_TIME(1997, 7, 13, 1, 2, 3))               
            });

        }
        /// <summary>
        /// Tests the following period subtraction case:
        //     _______
        // ________
        /// </summary>
        [Fact]
        public void TestSubtractPeriodRightOverlap()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 13, 1, 2, 3), new DURATION(0, 3));
            var other = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 2));
            Assert.Equal(period - other, new[]
            {
                new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DATE_TIME(1997, 7, 13, 1, 2, 3)),
                new PERIOD(new DATE_TIME(1997, 7, 16, 1, 2, 3), new DATE_TIME(1997, 7, 14, 1, 2, 3))
            });

        }

        /// <summary>
        /// Tests the following period subtraction case:
        //     _______
        // ________
        /// </summary>
        [Fact]
        public void TestSubtractPeriodRightOnlyOverlap()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 3));
            var other = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 2));
            Assert.Equal(period - other, new[]
            {
                new PERIOD(new DATE_TIME(1997, 7, 14, 1, 2, 3), new DATE_TIME(1997, 7, 15, 1, 2, 3))
            });

        }

        /// <summary>
        /// Tests the following period subtraction case:
        // ________
        //   ____
        /// </summary>
        [Fact]
        public void TestSubtractInnerOverlappedPeriod()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 6));
            var other = new PERIOD(new DATE_TIME(1997, 7, 14, 1, 2, 3), new DURATION(0, 2));
            Assert.Equal(period - other, new[]
            {
                new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DATE_TIME(1997, 7, 14, 1, 2, 3)),
                new PERIOD(new DATE_TIME(1997, 7, 18, 1, 2, 3), new DATE_TIME(1997, 7, 16, 1, 2, 3))
            });

        }

        /// <summary>
        /// Tests the following period subtraction case:
        //   ____
        // ________
        /// </summary>
        [Fact]
        public void TestSubtractOuterOverlappedPeriod()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 14, 1, 2, 3), new DURATION(0, 2));
            var other = new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DURATION(0, 6));
            Assert.Equal(period - other, new[]
            {
                new PERIOD(new DATE_TIME(1997, 7, 12, 1, 2, 3), new DATE_TIME(1997, 7, 14, 1, 2, 3)),
                new PERIOD(new DATE_TIME(1997, 7, 18, 1, 2, 3), new DATE_TIME(1997, 7, 16, 1, 2, 3))
            });

        }

        /// <summary>
        /// Tests the following period subtraction case:
        // ________
        // ________
        /// </summary>
        [Fact]
        public void TestSubtractEqualOverlappingPeriods()
        {
            var period = new PERIOD(new DATE_TIME(1997, 7, 14, 1, 2, 3), new DURATION(0, 2));
            Assert.Equal(period - period, new PERIOD[]{});

        }
    }
}
