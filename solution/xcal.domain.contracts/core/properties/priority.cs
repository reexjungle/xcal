namespace xcal.domain.contracts.core.properties
{
    public interface IPRIORITY
    {
        int Value { get; }
        PriorityType Format { get; }
        PRIORITYLEVEL Level { get; }
        PRIORITYSCHEMA Schema { get; }
    }
}