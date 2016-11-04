namespace reexjungle.xcal.core.domain.contracts.models.properties
{
    public interface IPRIORITY
    {
        int Value { get; }
        PriorityType Format { get; }
        PRIORITYLEVEL Level { get; }
        PRIORITYSCHEMA Schema { get; }
    }
}