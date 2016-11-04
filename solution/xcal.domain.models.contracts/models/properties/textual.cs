using reexjungle.xcal.core.domain.contracts.models.parameters;

namespace reexjungle.xcal.core.domain.contracts.models.properties
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