using System.Collections.Generic;

namespace reexmonkey.xcal.domain.contracts
{
    /// <summary>
    /// General contract for calendar parameters
    /// </summary>
    public interface IPARAMETER
    {
        bool IsDefault();
    }

    /// <summary>
    /// Alternative Text Representation Parameter.
    /// Specifies the alternative text representation of a property value.
    /// </summary>
    public interface IALTREP : IURI
    {
        ///// <summary>
        ///// Gets or sets the Uniform Resource Identifier (URI) that points to an alternative representation for a textual property value
        ///// </summary>
        //IURI Uri{ get; set; }
    }

    public interface ICN : IPARAMETER
    {
        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) that points to an alternative representation for a textual property value
        /// </summary>
        string Value{ get; set; }
    }

    /// <summary>
    /// Delegators
    /// Specfies the calendar users that have delegated their participation to the calendar user specfied by the property.
    /// </summary>
    public interface IDELEGATE : IPARAMETER
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        List<IURI> Addresses { get; set; }
    }

    /// <summary>
    /// Directory Entry Reference. 
    /// Specifies a reference to a directory entry associated with the calendar user specified by the user.
    /// </summary>
    public interface IDIR : IPARAMETER
    {
        /// <summary>
        /// Gets or sets URI reference to the directory entry.
        /// </summary>
        IURI Uri { get; set; }
    }

    public interface IFMTTYPE : IPARAMETER
    {
        string TypeName { get; set; }
        string SubTypeName { get; set; }
    }

    public interface ILANGUAGE : IPARAMETER
    {
        string Tag { get; set; }
        string SubTag { get; set; }
    }

    /// <summary>
    /// Group or List Membership.
    /// Specifies the group or list membership of the calendar user specified by a property
    /// </summary>
    public interface IMEMBER : IPARAMETER
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the group members
        /// </summary>
        IEnumerable<IURI> Addresses { get; set; }
    }

    /// <summary>
    /// Sent By.
    /// Specifies the calendar user that is acting on behalf of the calendar user specified by a property.
    /// </summary>
    public interface ISENT_BY : IPARAMETER
    {
        /// <summary>
        /// Gets or sets the address of the calendar user, who is acting on behalf of another user.
        /// </summary>
        IURI Address { get; set; }
    }

    /// <summary>
    /// Time Zone Identifier.
    /// Specifies the identifier for the time zone definition for a time component in a property value.
    /// </summary>
    /// <remarks>
    /// This paramter MUST be specified on the DATE_TIME, DATE_TIME, DUE, EXDATE and RDATE propertiws when either a DATE-TIME 
    /// or TIME value type is specified and when the value is neither a UTC or &quot; floating time &quot; time.
    /// </remarks>
    public interface ITZID: IPARAMETER
    {
        /// <summary>
        /// Gets or sets whether the time-zone prefix should be included
        /// </summary>
        bool GloballyUnique { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the time zone definition for a time component
        /// </summary>
        string Suffix { get; set; }
    }

}
