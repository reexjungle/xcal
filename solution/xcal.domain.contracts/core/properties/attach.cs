using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    /// <summary>
    /// Specifies a general contract for associating a document object (attachment) with an iCalendar object.
    /// </summary>
    public interface IATTACH
    {
        /// <summary>
        /// Gets or sets the format type of the resource being attached.
        /// </summary>
        IFMTTYPE FormatType { get; }

        /// <summary>
        /// Gets or sets the inline attached content
        /// </summary>
        string Content { get; }
    }
}
