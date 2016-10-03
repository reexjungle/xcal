using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface ITEXTUAL
    {
        IALTREP AlternativeText { get; set; }

        ILANGUAGE Language { get; set; }

        /// <summary>
        /// Gets or sets the text of the description
        /// </summary>
        string Text { get; set; }
    }
}