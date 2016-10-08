using System.Collections.Generic;

namespace xcal.domain.contracts.core.values
{
    /// <summary>
    /// Specifes a contract for a day (with possible recurrence) representation in the week.
    /// </summary>iCa
    public interface IWEEKDAYNUM
    {
        /// <summary>
        /// Gets or sets the nth occurence of the day within the MONTHLY or YEARLY recurrence rule
        /// </summary>
        int NthOccurrence { get; }

        /// <summary>
        /// Gets or sets the weekday
        /// </summary>
        WEEKDAY Weekday { get; }
    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a recurrence rule specification.
    /// </summary>
    public interface IRECUR
    {
        /// <summary>
        /// Gets or sets the frequency of the recurrence rule.
        /// This property is REQUIRED and must not occur more than once.
        /// </summary>
        FREQ FREQ { get; set; }

        /// <summary>
        /// Gets or sets the end date that bounds the reczrrence rule in an INCLUSIVE manner.
        /// This property is OPTIONAL but MUST be nullified if the COUNT property is non-zero.
        /// </summary>
        IDATE_TIME UNTIL { get; set; }

        /// <summary>
        /// Gets or sets the number of occurrences at which to range-bound the recurrence.
        /// This property is OPTIONAL but MUST be set to zero if the UNTIL property is non-null.
        /// </summary>
        uint COUNT { get; set; }

        /// <summary>
        /// Represents the intervals the recurrence rule repeats.
        /// The default value is 1.
        /// For example in a SECONDLY rule, this property implies a repetition for every second.
        /// </summary>
        uint INTERVAL { get; set; }

        /// <summary>
        /// Gets or sets the collection of seconds within a minute.
        /// Valid values are 0 to 60.
        /// </summary>
        /// <remarks>Normally, the seconds range is from 0 to 59 but the extra 60th second accounts for a leap second, which ma ybe ignored by a non-supporting system. </remarks>
        List<uint> BYSECOND { get; }

        /// <summary>
        /// Gets or sets the collection of minutes within an hour.
        /// Valid values are from 0 to 59.
        /// </summary>
        List<uint> BYMINUTE { get; }

        /// <summary>
        /// Gets or sets the collection of hours within a day.
        /// Valid values are from 0 to 23.
        /// </summary>
        List<uint> BYHOUR { get; }

        /// <summary>
        ///
        /// </summary>
        List<IWEEKDAYNUM> BYDAY { get; }

        /// <summary>
        /// Gets or sets the collection of days of the month.
        /// Valid values are 1 to 31 or -31 to -1.
        /// Negative values represent the day position from the end of the month e.g -10 is the tenth to the last day of the month
        /// </summary>
        List<int> BYMONTHDAY { get; }

        /// <summary>
        /// Gets or sets the collection of days of the year.
        /// Valid values are 1 to 366 or -366 to -1.
        /// Negative values represent the day position from the end of the year e.g -306 is the 306th to the last day of the month.
        /// This property MUST NOT be specified when the FREQ property is set to DAILY, WEEKLY, or MONTHLY
        /// </summary>
        List<int> BYYEARDAY { get; }

        /// <summary>
        /// Gets or sets the collection of ordinals specifiying the weeks of the year.
        /// Valid values are 1 to 53 or -53 to -1.
        /// This property MUST NOT be specified when the FREQ property is set to anything other than YEARLY.
        /// </summary>
        List<int> BYWEEKNO { get; }

        /// <summary>
        /// Gets or sets the collection of months of the year.
        /// Valid values are 1 to 12.
        /// </summary>
        List<uint> BYMONTH { get; }

        /// <summary>
        /// Gets or sets the day on which the work week starts.
        /// The default day is MO.
        /// </summary>
        /// <remarks>
        /// This is used significantly when a weekly recurrence rule has an interval greater than 1 and a BYDAY rule is specified.
        /// Also, it is significantly used when a BYWEEKNO rule in a YEARLY rule is specified.
        /// </remarks>
        WEEKDAY WKST { get; }

        /// <summary>
        /// Gets or sets the list of values corresponfing to the nth occurence within the set of recurrence instances in an interval of the recurrence rule.
        /// Valid values are 1 to 366 or -366 to -1
        /// This property MUST only be used together with another BYxxx rule part (e.g. BYDAY)
        /// </summary>
        /// <remarks>
        /// Example: &quot; the last work day of the month &quot;
        /// FRREQ = MONTHLY; BYDAY = MO, TU, WE, TH, FR; BYSETPOS = -1
        /// </remarks>
        List<int> BYSETPOS { get; }
    }
}
