using reexjungle.xcal.core.domain.contracts.models.parameters;
using System.Collections.Generic;

namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    /// <summary>
    /// Categories Property
    /// Specifies a contract for defining categories for a calendar component.
    /// </summary>
    public interface ICATEGORIES
    {
        /// <summary>
        /// Gets or sets the value of the categories
        /// </summary>
        List<string> Values { get; }

        /// <summary>
        /// Gets or sets the language used for the categories 
        /// </summary>
        ILANGUAGE Language { get; }
    }
}