using System.Collections.Generic;
using System.IO;
using reexjungle.xcal.domain.models;

namespace reexjungle.xcal.domain.contracts
{

    #region Descriptive Component Properties

    /// <summary>
    /// Specifies a general contract for associating a document object (attachment) with an iCalendar object.
    /// </summary>
    public interface IATTACH 
    {
        /// <summary>
        /// Gets or sets the format type of the resource being attached.
        /// </summary>
        FMTTYPE FormatType { get; set; }
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
    public interface ICATEGORIES
    {
        /// <summary>
        /// Gets or sets the value of the categories
        /// </summary>
        List<string> Values { get; set; }

        /// <summary>
        /// Gets or sets the language used for the categories 
        /// </summary>
        LANGUAGE Language { get; set; }
    }

    public interface ITEXTUAL
    {
        ALTREP AlternativeText { get; set; }

        LANGUAGE Language { get; set; }

        /// <summary>
        /// Gets or sets the text of the description
        /// </summary>
        string Text { get; set; }
    }

    public interface IGEO 
    {
        float Longitude { get; }
        float Latitude { get; }
    } 
 
    public interface IRESOURCES
    {
        ALTREP AlternativeText { get; set; }

        LANGUAGE Language { get; set; }

        List<string> Values { get; set; }
    }

    public interface IPRIORITY
    {
        int Value { get; }
        PriorityType Format { get; }
        PRIORITYLEVEL Level { get;}
        PRIORITYSCHEMA Schema { get; }
    }

    #endregion

    #region Date and Time Component Properties

    public interface IFREEBUSY_PROPERTY
    {
        FBTYPE Type { get; set; }

        List<PERIOD> Periods { get; set; }
    }
    
    #endregion

    #region Time Zone Component Properties

    public interface ITZNAME
    {
        LANGUAGE Language { get; set; }

        string Text { get; set; }
    }

    #endregion

    #region Relationship Component Properties

    public interface IATTENDEE
    {
        CAL_ADDRESS Address { get; set; }
        CUTYPE CalendarUserType { get; set; }
        MEMBER Member { get; set; }
        ROLE Role { get; set; }
        PARTSTAT Participation { get; set; }
        BOOLEAN Rsvp { get; set; }
        DELEGATED_TO Delegatee { get; set; }
        DELEGATED_FROM Delegator { get; set; }
        SENT_BY SentBy { get; set; }
        string CN { get; set; }
        DIR Directory { get; set; }
        LANGUAGE Language { get; set; }
    }

    public interface IORGANIZER 
    {
        CAL_ADDRESS Address { get; set; }
        string CN { get; set; }
        DIR Directory { get; set; }
        SENT_BY SentBy { get; set; }
        LANGUAGE Language { get; set; }
    }

    public interface IRECURRENCE_ID  
    {
        DATE_TIME Value { get; set; }
        RANGE Range { get; set; }
        TZID TimeZoneId { get; set; }
    }
   
    public interface IRELATEDTO
    {
        string Reference { get; set; }
        RELTYPE RelationshipType { get; set; }
    }

    #endregion

    #region Recurrence Component Properties

    public interface IEXDATE
    {
        List<DATE_TIME> DateTimes { get; set; }
        TZID TimeZoneId { get; set; }
    }

    public interface IRDATE
    { 
        List<DATE_TIME> DateTimes { get; set; }
        List<PERIOD> Periods { get; set; }
        TZID TimeZoneId { get; set; }
        VALUE ValueType { get; set; }
    }

    #endregion

    #region Alarm Component Properties

    public interface ITRIGGER
    {
        //trigrel
        DURATION Duration { get; set; }

        RELATED Related { get; set; }

        //trigabs
        DATE_TIME DateTime {get; set;}

        //Selector
        VALUE ValueType { get; set; }

    }

	#endregion

    #region Miscellaneous Component Properties

    public interface IMISC_PROPERTY
    { 

        string Name { get; set; }
        List<object> Parameters { get; set; }
        object Value { get; set; }   
        VALUE ValueType {get; set; }
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
        uint? L3 { get; set; }
    }

    /// <summary>
    /// Specifies an interface for an IANA-registered property.
    /// </summary>
    public interface IIANA_PROPERTY
    {
        /// <summary>
        /// An IANA-registered property name.
        /// </summary>
        string Token { get; set; }

        /// <summary>
        /// A list of registered iCalendar parameters. Any iCalendar parameter can be specified in the list.
        /// </summary>
        List<object> Parameters { get; set; }


        /// <summary>
        /// A value for the IANA-registered property. The default value type is TEXT (string) although any value type can be set.
        /// </summary>
        object Value { get; set; }
    }


    public interface IX_PROPERTY
    {
        /// <summary>
        /// An custom property name. The name must have an &quot;X-&quot; prefix.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A list of registered iCalendar parameters. Any iCalendar parameter can be specified in the list.
        /// </summary>
        List<object> Parameters { get; set; }


        /// <summary>
        /// A value for the custom property. The default value type is TEXT (string) although any value type can be set.
        /// </summary>
        object Value { get; set; }
    }

    public interface IREQUEST_STATUS
    {
        STATCODE Code { get; set; }
        string Description { get; set; }
        string ExceptionData { get; set; }
        LANGUAGE Language { get; set; }

    }

    #endregion

}
