using System;
using System.Collections.Generic;
using System.Linq;
using reexjungle.xcal.infrastructure.contracts;

namespace reexjungle.xcal.infrastructure.serialization
{
    public abstract class CalendarNode : IEquatable<CalendarNode>
    {
        public string Name { get; }

        public CalendarNodeType Type { get; }

        public string Value { get; }

        protected CalendarNode(CalendarNodeType type) : this(type, string.Empty)
        {
        }

        protected CalendarNode(CalendarNodeType type, string value) : this(string.Empty, type, string.Empty)
        {
        }

        protected CalendarNode(string name, CalendarNodeType type, string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            Name = name;
            Type = type;
            Value = value;
        }

        public static string Flatten(string separator, params string[] tokens)
            => tokens != null && tokens.Length > 0 ? string.Join(separator, tokens) : string.Empty;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CalendarNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase)
                   && Type == other.Type
                   && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CalendarNode)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Value?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(CalendarNode left, CalendarNode right) => Equals(left, right);

        public static bool operator !=(CalendarNode left, CalendarNode right) => !Equals(left, right);
    }

    public class BeginCalendarNode : CalendarNode
    {
        public static BeginCalendarNode Empty = new BeginCalendarNode(string.Empty);
        public BeginCalendarNode(string value) : base(string.Empty, CalendarNodeType.BEGIN, value)
        {
        }
    }

    public class EndCalendarNode : CalendarNode
    {
        public static EndCalendarNode Empty = new EndCalendarNode(string.Empty);

        public EndCalendarNode(string value) : base(string.Empty, CalendarNodeType.END, value)
        {
        }
    }

    public class ValueCalendarNode : CalendarNode
    {
        public static ValueCalendarNode Empty = new ValueCalendarNode(string.Empty);

        public ValueCalendarNode(string value) : base(CalendarNodeType.VALUE, value)
        {
        }

        public static ValueCalendarNode Flatten(string separator, IEnumerable<ValueCalendarNode> nodes)
            => new ValueCalendarNode(Flatten(separator, nodes?.Select(x => x.Value).ToArray()));
    }

    public class ParameterCalendarNode : CalendarNode
    {
        public static ParameterCalendarNode Empty = new ParameterCalendarNode(string.Empty, ValueCalendarNode.Empty);

        public List<ValueCalendarNode> ValueNodes { get; }

        private static string CreateText(string name, string[] values)
            => values != null ? $"{name}={Flatten("\\,", values)}" : string.Empty;

        public ParameterCalendarNode(string name, params ValueCalendarNode[] values)
            : base(name, CalendarNodeType.PARAMETER, CreateText(name, values?.Select(x => x.Value).ToArray()))
        {
            ValueNodes = values != null && !string.IsNullOrEmpty(Value)
                ? new List<ValueCalendarNode>(values)
                : new List<ValueCalendarNode>();
        }

        public ParameterCalendarNode(string name, params string[] values)
            : base(name, CalendarNodeType.PARAMETER, CreateText(name, values))
        {
            ValueNodes = values != null && !string.IsNullOrEmpty(Value)
                ? values.Select(x => new ValueCalendarNode(x)).ToList()
                : new List<ValueCalendarNode>();
        }
    }

    public class PropertyCalendarNode : CalendarNode
    {
        public static PropertyCalendarNode Empty = new PropertyCalendarNode(string.Empty, Enumerable.Empty<ParameterCalendarNode>(), Enumerable.Empty<ValueCalendarNode>());

        public List<ParameterCalendarNode> ParameterNodes { get; }

        public List<ValueCalendarNode> ValueNodes { get; }

        private static string CreateText(string name, params string[] values)
            => values != null && values.Any() ? $"{name};{Flatten("\\;", values)}" : string.Empty;

        private static string CreateText(string name, string[] parameters, string[] values)
            =>
                values != null && values.Any() && parameters != null && parameters.Any()
                    ? $"{name};{Flatten("\\;", parameters)}:{Flatten("\\,", values)}"
                    : string.Empty;

        public PropertyCalendarNode(string name, IEnumerable<string> values)
            : base(name, CalendarNodeType.PROPERTY, CreateText(name, CreateText(name, values?.ToArray())))
        {
            ValueNodes = values != null && !string.IsNullOrEmpty(Value)
                ? values.Select(x => new ValueCalendarNode(x)).ToList()
                : new List<ValueCalendarNode>();

            ParameterNodes = new List<ParameterCalendarNode>();
        }

        public PropertyCalendarNode(string name, IEnumerable<ValueCalendarNode> values)
            : base(name, CalendarNodeType.PROPERTY, CreateText(name, values?.Select(x => x.Value).ToArray()))
        {
            ValueNodes = values != null && !string.IsNullOrEmpty(Value)
                ? new List<ValueCalendarNode>(values)
                : new List<ValueCalendarNode>();

            ParameterNodes = new List<ParameterCalendarNode>();
        }

        public PropertyCalendarNode(string name, IEnumerable<ParameterCalendarNode> parameters,
            IEnumerable<string> values)
            : base(
                name, CalendarNodeType.PROPERTY,
                CreateText(name, parameters?.Select(x => x.Value).ToArray(), values?.ToArray()))
        {
            ValueNodes = values != null && !string.IsNullOrEmpty(Value)
                ? values.Select(x => new ValueCalendarNode(x)).ToList()
                : new List<ValueCalendarNode>();

            ParameterNodes = parameters != null && parameters.Any()
                ? new List<ParameterCalendarNode>(parameters)
                : new List<ParameterCalendarNode>();
        }

        public PropertyCalendarNode(string name, IEnumerable<ParameterCalendarNode> parameters,
            IEnumerable<ValueCalendarNode> values)
            : base(name, CalendarNodeType.PROPERTY,
                CreateText(name, parameters?.Select(x => x.Value).ToArray(), values?.Select(x => x.Value).ToArray()))
        {
            ValueNodes = values != null && !string.IsNullOrEmpty(Value)
                ? new List<ValueCalendarNode>(values)
                : new List<ValueCalendarNode>();

            ParameterNodes = parameters != null && parameters.Any()
                ? new List<ParameterCalendarNode>(parameters)
                : new List<ParameterCalendarNode>();
        }
    }
}
