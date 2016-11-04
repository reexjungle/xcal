using reexjungle.xcal.core.domain.contracts.models.parameters;

namespace reexjungle.xcal.core.domain.contracts.models.properties
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
