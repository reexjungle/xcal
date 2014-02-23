using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexmonkey.xcal.domain.contracts
{

    public interface IPROPERTY 
    {
        bool IsDefault();
    }

    #region Descriptive Component Properties

    /// <summary>
    /// Specifies a general contract for associating a document object (attachment) with an iCalendar object.
    /// </summary>
    public interface IATTACH : IPROPERTY 
    {
        /// <summary>
        /// Gets or sets the format type of the resource being attached.
        /// </summary>
        IFMTTYPE FormatType { get; set; }
    }

    /// <summary>
    /// Attachment Property
    /// Specifies a contract for associating a document object with an iCalendar object.
    /// </summary>
    /// <typeparam name="TContent">The type parameter for the document content</typeparam>
    public interface IATTACH<TContent>: IATTACH
    {        
        /// <summary>
        /// Gets or sets the inline attached content
        /// </summary>
        TContent Content { get; set; }
    }

    /// <summary>
    /// Categories Property
    /// Specifies a contract for defining categories for a calendar component.
    /// </summary>
    public interface ICATEGORIES : IPROPERTY
    {
        /// <summary>
        /// Gets or sets the value of the categories
        /// </summary>
        List<string> Values { get; set; }

        /// <summary>
        /// Gets or sets the language used for the categories 
        /// </summary>
        ILANGUAGE Language { get; set; }
    }

    //public interface ICOMMENT : IPROPERTY
    //{
    //    /// <summary>
    //    /// Gets or sets the text of the comment
    //    /// </summary>
    //    string Text { get; set; }

    //    IALTREP AlternativeText { get; set; }

    //    ILANGUAGE Language { get; set; }

    //}

    public interface ITEXT : IPROPERTY
    {
        IALTREP AlternativeText { get; set; }

        ILANGUAGE Language { get; set; }

        /// <summary>
        /// Gets or sets the text of the description
        /// </summary>
        string Text { get; set; }
    }

    public interface IGEO : IPROPERTY
    {
        float Longitude { get; set; }
        float Latitude { get; set; }
    } 
  
    //public interface ILOCATION : IPROPERTY
    //{
    //    IALTREP AlternativeText { get; set; }

    //    ILANGUAGE Language { get; set; }

    //    string Text { get; set; }
    //} 

    public interface IRESOURCES : IPROPERTY
    {
        IALTREP AlternativeText { get; set; }

        ILANGUAGE Language { get; set; }

        List<string> Values { get; set; }
    }

    //public interface ISUMMARY : IPROPERTY
    //{
    //    IALTREP AlternativeText { get; set; }

    //    ILANGUAGE Language { get; set; }

    //    string Text { get; set; }
    //}

    public interface IPRIORITY: IPROPERTY
    {
        int Value { get; set; }
        PriorityFormat Format { get; set; }
        PRIORITYLEVEL Level { get; set; }
        PRIORITYSCHEMA Schema { get; set; }
    }

    #endregion

    #region Date and Time Component Properties

    public interface IDURATIONPROP: IPROPERTY
    {
        IDURATION Value { get; set; }
    }

    public interface IFREEBUSY : IPROPERTY
    {
        FBTYPE Type { get; set; }

        IEnumerable<IPERIOD> Periods { get; set; }
    }

#endregion

    #region Time Zone Component Properties

    public interface ITZIDPROP : IPROPERTY
    {
        /// <summary>
        /// Gets or sets whether the time-zone prefix should be included
        /// </summary>
        bool GloballyUnique { get; set; }

        string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        string Value { get; set; }
    }

    public interface ITZNAME : IPROPERTY
    {
        ILANGUAGE Language { get; set; }

        string Text { get; set; }
    }

    public interface ITZOFFSETFROM : IPROPERTY
    {
        IUTC_OFFSET Offset { get; set; }
    }

    public interface ITZOFFSETTO : IPROPERTY
    {
        IUTC_OFFSET Offset { get; set; }
    }

    public interface ITZURL : IPROPERTY
    {
        IURI Uri { get; set; }
    }


    #endregion

    #region Relationship Component Properties

    public interface IATTENDEE : IPROPERTY
    {
        IURI Address { get; set; }
        CUTYPE CalendarUserType { get; set; }
        IMEMBER Member { get; set; }
        ROLE Role { get; set; }
        PARTSTAT Participation { get; set; }
        BOOLEAN Rsvp { get; set; }
        IDELEGATE Delegate { get; set; }
        IDELEGATE Delegator { get; set; }
        IURI SentBy { get; set; }
        string CN { get; set; }
        IURI Directory { get; set; }
        ILANGUAGE Language { get; set; }
    }

    public interface ICONTACT : IPROPERTY
    {
        string Value { get; set; }

        IALTREP AlternativeText { get; set; }

        ILANGUAGE Language { get; set; }

    }

    public interface IORGANIZER : IPROPERTY
    {
        IURI Address { get; set; }
        string CN { get; set; }
        IURI Directory { get; set; }
        IURI SentBy { get; set; }
        ILANGUAGE Language { get; set; }
    }

    public interface IRECURRENCE_ID : IPROPERTY 
    {
        IDATE_TIME Value { get; set; }
        RANGE Range { get; set; }
        ITZID TimeZoneId { get; set; }
    }
   
    public interface IRELATEDTO : IPROPERTY
    {
        string Reference { get; set; }
        RELTYPE RelationshipType { get; set; }
    }

    public interface IURL : IPROPERTY
    {
        IURI Uri { get; set; }
    }

    public interface IUID : IPROPERTY
    {
        string Value {get; set;}
    }

    #endregion

    #region Recurrence Component Properties

    public interface IEXDATE : IPROPERTY
    {
        IEnumerable<IDATE_TIME> DateTimes { get; set; }
        ITZID TimeZoneId { get; set; }
        ValueFormat Format { get; set; }
    }

    public interface IRDATE : IPROPERTY
    { 
        IEnumerable<IDATE_TIME> DateTimes { get; set; }
        IEnumerable<IPERIOD> Periods { get; set; }
        ITZID TimeZoneId { get; set; }
        ValueFormat Format { get; set; }
    }

    public interface IRRULE: IPROPERTY
    {
        IRECUR Value { get; set; }
    }

    #endregion

    #region Alarm Component Properties

    public interface ITRIGGER : IPROPERTY
    {
        //trigrel
        IDURATION Duration { get; set; }
        RELATED Related { get; set; }

        //trigabs
        IDATE_TIME DateTime {get; set;}

        //Selector
        ValueFormat Format { get; set; }

    }

    public interface IREPEAT: IPROPERTY
    {
        int Value { get; set; }
    }

	#endregion

    #region Change Management Component Properties

    public interface ICREATED: IPROPERTY
    {
        IDATE_TIME Value { get; set; }
    }

    public interface IDTSTAMP: IPROPERTY
    {
        IDATE_TIME Value { get; set; }
    }

    public interface ILASTMODIFIED : IPROPERTY
    {
        IDATE_TIME Value { get; set; }
    }

    public interface ISEQUENCE : IPROPERTY
    {
        int Value { get; set; }
    }

    #endregion

    #region Miscellaneous Component Properties

    public interface IIANA_PROPERTY : IPROPERTY { }

    public interface IIANA_PROPERTY<TValue>: IIANA_PROPERTY
    {
        string Name { get; set; }
        IEnumerable<IPARAMETER> Parameters { get; set; }
        TValue Value { get; set; }
    }

    public interface  IXPROPERTY: IPROPERTY { }

    public interface IXPROPERTY<TValue>: IXPROPERTY
    {
        string Name { get; set; }
        IEnumerable<IPARAMETER> Parameters { get; set; }
        TValue Value { get; set; }
    }

    /// <summary>
    /// Specifies an interface for a return status code
    /// </summary>
    public interface ISTATCODE
    {
        /// <summary>
        /// First level of granularity
        /// </summary>
        uint L1 { get; set; }

        /// <summary>
        /// Second level of granularity
        /// </summary>
        uint L2 { get; set; }

        /// <summary>
        /// Third level of granularity
        /// </summary>
        uint L3 { get; set; }
    }

    public interface IREQUEST_STATUS : IPROPERTY
    {
        ISTATCODE Code { get; set; }
        string Description { get; set; }
        string ExceptionData { get; set; }
        ILANGUAGE Language { get; set; }

    }

    #endregion

}
