using reexjungle.xcal.core.domain.contracts.serialization;
using System.Collections.Generic;


namespace reexjungle.xcal.core.domain.contracts.io.writers.specialized
{
    public interface IGenericCalendarWriter
    {
        ICalendarWriter WriteValue<T>(T value) where T : ICalendarSerializable;


        ICalendarWriter WriteParameters<T>(IEnumerable<T> parameters) where T : ICalendarSerializable;

        ICalendarWriter WriteParameter<T>(string name, T value) where T : ICalendarSerializable;

        ICalendarWriter WriteParameter<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter WriteParameterWithDQuotedValue<T>(string name, T value) where T : ICalendarSerializable;

        ICalendarWriter WriteParameterWithDQuotedValues<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter WriteParameterValues<T>(IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter WriteDQuotedParameterValue<T>(T value) where T : ICalendarSerializable;

        ICalendarWriter WriteDQuotedParameterValues<T>(IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter AppendParameterValue<T>(T value) where T : ICalendarSerializable;



        ICalendarWriter AppendParameterValues<T>(IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter AppendParameter<T>(T parameter) where T : ICalendarSerializable;

        ICalendarWriter AppendParameter<T>(string name, T value) where T : ICalendarSerializable;

        ICalendarWriter AppendParameter<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter AppendParameters<T>(IEnumerable<T> parameters) where T : ICalendarSerializable;



        ICalendarWriter WriteProperty<T>(string name, T value) where T : ICalendarSerializable;

        ICalendarWriter WriteProperty<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter WriteProperties<T>(IEnumerable<T> properties) where T : ICalendarSerializable;

        ICalendarWriter AppendProperty<T>(T property) where T : ICalendarSerializable;

        ICalendarWriter AppendProperties<T>(IEnumerable<T> properties) where T : ICalendarSerializable;

        ICalendarWriter AppendPropertyValues<T>(IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter AppendProperty<T>(string name, T value) where T : ICalendarSerializable;

        ICalendarWriter AppendProperty<T>(string name, IEnumerable<T> values) where T : ICalendarSerializable;


        ICalendarWriter WritePropertyValues<T>(IEnumerable<T> values) where T : ICalendarSerializable;

        ICalendarWriter AppendPropertyValue<T>(T value) where T : ICalendarSerializable;


    }
}
