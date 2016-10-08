using xcal.domain.contracts.core.parameters;

namespace xcal.domain.contracts.core.properties
{
    public interface ITEXTUAL
    {
        IALTREP AlternativeText { get; }

        ILANGUAGE Language { get; }

        /// <summary>
        /// Gets or sets the text of the description
        /// </summary>
        string Text { get; }
    }

    public interface IDESCRIPTION : ITEXTUAL
    {

    }

    public interface ISUMMARY : ITEXTUAL
    {

    }

    public interface ILOCATION : ITEXTUAL
    {

    }

    public interface ICONTACT : ITEXTUAL
    {

    }

    public interface ICOMMENT : ITEXTUAL
    {

    }
}