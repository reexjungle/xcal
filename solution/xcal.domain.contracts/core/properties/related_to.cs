namespace xcal.domain.contracts.core.properties
{
    public interface IRELATED_TO
    {
        string Reference { get; }

        RELTYPE RelationshipType { get; }
    }

}
