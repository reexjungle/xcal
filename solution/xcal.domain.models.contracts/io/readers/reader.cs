namespace reexjungle.xcal.core.domain.contracts.io.readers
{
    public interface ICalendarReader
    {
        string Name { get; }

        string Value { get; }

        NodeType NodeType { get; }

        bool Read();

        bool EOF();

        void Close();

        ICalendarReader ReadFragment();
    }
}
