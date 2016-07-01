using System;
using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface IValuesFactory
    {
        DATE CreateDate();

        IEnumerable<DATE> CreateDates(int quantity);

        DATE_TIME CreateDateTime();

        IEnumerable<DATE_TIME> CreateDateTimes(int quantity);

        TIME CreateTime();

        IEnumerable<TIME> CreateTimes(int quantity);

        DURATION CreateDuration();

        IEnumerable<DURATION> CreateDurations(int quantity);

        WEEKDAYNUM CreateWeekdaynum();

        IEnumerable<WEEKDAYNUM> CreateWeekdaynums(int quantity);

        UTC_OFFSET CreateUtcOffset();

        IEnumerable<UTC_OFFSET> CreateUtcOffsets(int quantity);

        PERIOD CreatePeriod();

        IEnumerable<PERIOD> CreatePeriods(int quantity);

        RECUR CreateRecurrence();

        IEnumerable<RECUR> CreateRecurrences(int quantity);

        CAL_ADDRESS CreateEmail(string username);

        IEnumerable<CAL_ADDRESS> CreateEmails(IEnumerable<string> usernames, int quantity);

        Uri CreateUri();

        IEnumerable<Uri> CreateUris(int quantity);
    }
}