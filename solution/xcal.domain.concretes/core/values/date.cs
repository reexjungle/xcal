using System;
using xcal.domain.contracts.core;
using xcal.domain.contracts.core.values;

namespace xcal.domain.concretes.core.values
{
    public struct DATE : IDATE, IDATE<DATE>
    {
        public uint FULLYEAR { get; }
        public uint MONTH { get; }
        public uint MDAY { get; }

        public DATE AddDays(double value)
        {
            throw new NotImplementedException();
        }

        public DATE AddWeeks(int value)
        {
            throw new NotImplementedException();
        }

        public DATE AddMonths(int value)
        {
            throw new NotImplementedException();
        }

        public DATE AddYears(int value)
        {
            throw new NotImplementedException();
        }

        public WEEKDAY GetWeekday()
        {
            throw new NotImplementedException();
        }
    }
}
