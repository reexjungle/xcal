using System.Collections.Generic;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.domain.contracts
{
    /// <summary>
    /// Specifies the value type and format of a property value.
    /// </summary>
    /// <remarks>The property values MUST be of a single value type</remarks>
    public interface IVALUE { }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a character encoding of inline binary data.
    /// The character encoding is based on the Base64 encoding
    /// </summary>
    public interface IBINARY : IVALUE 
    {
        /// <summary>
        /// Gets or sets the Base64 value of this type. 
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets or sets the encoding used for the binary type.
        /// </summary>
        ENCODING Encoding { get; set; }
    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a calendar date.
    /// Format: [YYYYMMDD] where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// </summary>
    public interface IDATE : IVALUE 
    {
        /// <summary>
        /// Gets the 4-digit representation of a full year e.g. 2013 
        /// </summary>
        uint FULLYEAR { get; }

        /// <summary>
        /// Gets the 2-digit representation of a month
        /// </summary>
        uint MONTH { get; }

        /// <summary>
        /// Gets the 2-digit representation of a month-day
        /// </summary>
        uint MDAY { get;}

    }

    /// <summary>
    /// Specifies the contract for identifying values that specify a precise calendar date and time of the date
    /// Format: [YYYYMMSS]&quot;T&quot;[HHMMSS]&quot;Z&quot; 
    /// where YYYY is 4-digit year, MM is 2-digit month and DD is 2-digit day
    /// 
    /// </summary>
    public interface IDATE_TIME : IDATE, ITIME  { }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a duration of time
    /// Format: &quot;+&quot;/&quot;-&quot;&quot;P&quot;[days &quot;D &quot;/[&quot;T&quot; [hours &quot;H&quot; / minutes &quot;M&quot; /seconds &quot;S&quot;] weeks &quot;W&quot;]]
    /// </summary>
    public interface IDURATION : IVALUE
    {
        /// <summary>
        /// Gets the duration in weeks
        /// </summary>
        uint WEEKS { get;}

        /// <summary>
        /// Gets the duration in hours
        /// </summary>
        uint HOURS { get;}

        /// <summary>
        /// Gets the duration in minutes
        /// </summary>
        uint MINUTES { get;}

        /// <summary>
        /// Gets the duration in seconds
        /// </summary>
        uint SECONDS { get; }

        /// <summary>
        /// Gets the duration in days
        /// </summary>
        uint DAYS { get; }

        SignType Sign { get;}
    }

    /// <summary>
    /// Specifies a contract for identifying properties that contain a precise period of time
    /// </summary>
    public interface IPERIOD : IVALUE 
    {
    
        /// <summary>
        /// Gets or sets the start of the period
        /// </summary>
        DATE_TIME Start { get; }

        /// <summary>
        /// Gets or sets the end of the period.
        /// </summary>
        DATE_TIME End { get;}

        /// <summary>
        /// Gets or sets the duration of the period.
        /// </summary>
        DURATION Duration { get;}
    }

    /// <summary>
    /// Specifes a contract for a day (with possible recurrence) representation in the week.
    /// </summary>iCa
    public interface IWEEKDAYNUM
    {
        /// <summary>
        /// Gets or sets the nth occurence of the day within the MONTHLY or YEARLY recurrence rule
        /// </summary>
        int OrdinalWeek { get; }

        /// <summary>
        /// Gets or sets the weekday
        /// </summary>
        WEEKDAY Weekday { get; }
    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a recurrence rule specification.
    /// </summary>
    public interface IRECUR : IVALUE
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
        DATE_TIME UNTIL { get; set; }

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
        List<uint> BYSECOND { get; set; }

        /// <summary>
        /// Gets or sets the collection of minutes within an hour.
        /// Valid values are from 0 to 59.
        /// </summary>
        List<uint> BYMINUTE { get; set; }

        /// <summary>
        /// Gets or sets the collection of hours within a day.
        /// Valid values are from 0 to 23.
        /// </summary>
        List<uint> BYHOUR { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        List<WEEKDAYNUM> BYDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of days of the month. 
        /// Valid values are 1 to 31 or -31 to -1.
        /// Negative values represent the day position from the end of the month e.g -10 is the tenth to the last day of the month
        /// </summary>
        List<int> BYMONTHDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of days of the year.
        /// Valid values are 1 to 366 or -366 to -1.
        /// Negative values represent the day position from the end of the year e.g -306 is the 306th to the last day of the month.
        /// This property MUST NOT be specified when the FREQ property is set to DAILY, WEEKLY, or MONTHLY
        /// </summary>
        List<int> BYYEARDAY { get; set; }

        /// <summary>
        /// Gets or sets the collection of ordinals specifiying the weeks of the year.
        /// Valid values are 1 to 53 or -53 to -1.
        /// This property MUST NOT be specified when the FREQ property is set to anything other than YEARLY.
        /// </summary>
        List<int> BYWEEKNO { get; set; }

        /// <summary>
        /// Gets or sets the collection of months of the year. 
        /// Valid values are 1 to 12.
        /// </summary>
        List<uint> BYMONTH { get; set; }

        /// <summary>
        /// Gets or sets the day on which the work week starts.
        /// The default day is MO.
        /// </summary>
        /// <remarks>
        /// This is used significantly when a weekly recurrence rule has an interval greater than 1 and a BYDAY rule is specified. 
        /// Also, it is significantly used when a BYWEEKNO rule in a YEARLY rule is specified.
        /// </remarks>
        WEEKDAY WKST { get; set; }

        /// <summary>
        /// Gets or sets the list of values corresponfing to the nth occurence within the set of recurrence instances in an interval of the recurrence rule.
        /// Valid values are 1 to 366 or -366 to -1
        /// This property MUST only be used together with another BYxxx rule part (e.g. BYDAY)
        /// </summary>
        /// <remarks>
        /// Example: &quot; the last work day of the month &quot;
        /// FRREQ = MONTHLY; BYDAY = MO, TU, WE, TH, FR; BYSETPOS = -1
        /// </remarks>
        List<int> BYSETPOS { get; set; }

        RecurFormat Format { get; set; }
    
    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a time of the day 
    /// Format 1 (Local Time): [HHMMSS] 
    /// Format 2 (UTC Time): [HHMMSS]&quot;Z&quot; 
    /// where HH is 2-digit hour, MM is 2-digit minute, SS is 2-digit second and Z is UTC zone indicator
    /// </summary>
    public interface ITIME : IVALUE
    {
        /// <summary>
        /// Gets the 2-digit representation of an hour
        /// </summary>
        uint HOUR { get; }

        /// <summary>
        /// Gets or sets the 2-digit representatio of a minute
        /// </summary>
        uint MINUTE { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second
        /// </summary>
        uint SECOND { get;}

        TimeFormat TimeFormat { get;}

        TZID TimeZoneId { get;}

    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain a uniform resource identifier (URI) type of reference to the property value
    /// </summary>
    public interface IURI : IVALUE 
    {
        string Path { get; set; }
    }

    /// <summary>
    /// Specifies the contract for identifying properties that contain an offset from UTC to local time 
    /// </summary>
    public interface IUTC_OFFSET : IVALUE 
    {
        SignType Sign { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of an hour
        /// </summary>
        uint HOUR { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a minute
        /// </summary>
        uint MINUTE { get; }

        /// <summary>
        /// Gets or sets the 2-digit representation of a second
        /// </summary>
        uint SECOND { get; }
    }

}
