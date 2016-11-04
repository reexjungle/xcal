namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IRELATED_TO
    {
        string Reference { get; }

        RELTYPE RelationshipType { get; }
    }

}
