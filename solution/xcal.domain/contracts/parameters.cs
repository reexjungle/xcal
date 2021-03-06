﻿using System;
using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.domain.contracts
{
    /// <summary>
    /// Delegators
    /// Specfies the calendar users that have delegated their participation to the calendar user specfied by the property.
    /// </summary>
    public interface IDELEGATE
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the participation delegators
        /// </summary>
        List<CAL_ADDRESS> Addresses { get; set; }
    }

    public interface IFMTTYPE
    {
        string TypeName { get; set; }

        string SubTypeName { get; set; }
    }

    public interface ILANGUAGE
    {
        string Tag { get; set; }

        string SubTag { get; set; }
    }

    /// <summary>
    /// Group or List Membership.
    /// Specifies the group or list membership of the calendar user specified by a property
    /// </summary>
    public interface IMEMBER
    {
        /// <summary>
        /// Gets or sets the calendar addresses of the group members
        /// </summary>
        List<CAL_ADDRESS> Addresses { get; set; }
    }

    /// <summary>
    /// Time Zone Identifier.
    /// Specifies the identifier for the time zone definition for a time component in a property value.
    /// </summary>
    /// <remarks>
    /// This paramter MUST be specified on the DATE_TIME, DATE_TIME, DUE, EXDATE and RDATE propertiws when either a DATE-TIME
    /// or TIME value type is specified and when the value is neither a UTC or &quot; floating time &quot; time.
    /// </remarks>
    public interface ITZID
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

    public interface IContainsUri
    {
        Uri Uri { get; set; }
    }

    public interface ISENT_BY
    {
        CAL_ADDRESS Address { get; set; }
    }
}