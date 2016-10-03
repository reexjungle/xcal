using System.Collections.Generic;
using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
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
        List<string> Values { get; set; }

        /// <summary>
        /// Gets or sets the language used for the categories 
        /// </summary>
        ILANGUAGE Language { get; set; }
    }
}