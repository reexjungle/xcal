using System;
using System.Runtime.Serialization;

namespace reexjungle.xcal.domain.contracts.core.values
{
    public interface CAL_ADDRESS : ISerializable
    {
        Uri Value { get; }
    }
}
