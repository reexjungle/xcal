using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using reexjungle.xcal.infrastructure.contracts;

namespace reexjungle.xcal.infrastructure.serialization
{
    public class CalendarSerializer: ICalendarSerializer, ISerializable, IObjectReference
    {
        private readonly Type type;

        public CalendarSerializer(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            this.type = type;
        }

        protected void SerializePrimitive(CalendarWriter writer, object o)
        {
            var otype = o.GetType();
            switch (Type.GetTypeCode(otype))
            {
                case TypeCode.Boolean:
                    writer.Write((bool)o ? "TRUE" : "FALSE");
                    break;
                case TypeCode.Char:
                    writer.Write((char)o);
                    break;
                case TypeCode.SByte:
                    writer.Write((sbyte) o);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)o);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)o);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)o);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)o);
                    break;
                case TypeCode.UInt32:
                    writer.Write((int)o);
                    break;
                case TypeCode.Int64:
                    writer.Write((int)o);
                    break;
                case TypeCode.UInt64:
                    writer.Write((int)o);
                    break;
                case TypeCode.Single:
                    writer.Write((float)o);
                    break;
                case TypeCode.Double:
                    writer.Write((double)o);
                    break;
                case TypeCode.Decimal:
                    writer.Write((decimal)o);
                    break;
                case TypeCode.DateTime:
                    var date = (DateTime)o;
                    if (date.Kind == DateTimeKind.Local || date.Kind == DateTimeKind.Unspecified)
                    {
                        writer.WriteValue($"{date.Year:D4}{date.Month:D2}{date.Day:D2}T{date.Hour:D2}{date.Minute:D2}{date.Second:D2}");
                    }
                    if (date.Kind == DateTimeKind.Utc)
                    {
                        writer.WriteValue($"{date.Year:D4}{date.Month:D2}{date.Day:D2}T{date.Hour:D2}{date.Minute:D2}{date.Second:D2}Z");
                    }
                    break;

                case TypeCode.String:
                    writer.Write((string)o);
                    break;
            }

            if (otype == typeof (TimeSpan))
            {
                var ts = (TimeSpan)o;
                var sb = new StringBuilder();
                var sign = ( ts.Days < 0 || ts.Hours < 0 || ts.Minutes < 0 || ts.Seconds < 0) ? "-" : string.Empty;
                sb.AppendFormat("{0}P", sign);
                if (ts.Days != 0) sb.AppendFormat("{0}D", ts.Days);
                if (ts.Hours != 0 || ts.Minutes != 0 || ts.Seconds != 0) sb.Append("T");
                if (ts.Hours != 0) sb.AppendFormat("{0}H", ts.Hours);
                if (ts.Minutes != 0) sb.AppendFormat("{0}M", ts.Minutes);
                if (ts.Seconds != 0) sb.AppendFormat("{0}S", ts.Seconds);
            }

            if (otype == typeof(byte[]))
            {
                var bytes = (byte[])o;
                writer.Write(Convert.ToBase64String(bytes, Base64FormattingOptions.None));
            }
            if (otype == typeof(Guid))
            {
                writer.Write(((Guid) o).ToString());
            }
        }

        public void Serialize(CalendarWriter writer, object o)
        {
            if (o == null) return;

            try
            {
                var otype = o.GetType();
                if (otype.IsPrimitive
                    || otype == typeof (decimal)
                    || otype == typeof (string)
                    || otype == typeof (DateTime)
                    || otype == (typeof (byte[]))
                    || otype == typeof (TimeSpan)
                    || otype == typeof (Guid))
                {
                    SerializePrimitive(writer, o);
                    return;
                }

                var serializable = o as ICalendarSerializable;
                if (serializable == null)
                    throw new InvalidOperationException("Object of type:" + otype.FullName + " does not support ICalendarSerializable!");

                serializable.WriteCalendar(writer);
            }
            catch (Exception inner)
            {
                if (inner is ThreadAbortException || inner is StackOverflowException || inner is OutOfMemoryException)
                {
                    throw;
                }
                if (inner is TargetInvocationException)
                {
                    inner = inner.InnerException;
                }
                throw new InvalidOperationException("Calendar Serialization Error", inner);
            }
            writer.Flush();
        }


        protected object DeserializePrimitive(CalendarReader reader)
        {
            object o;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    
                    break;
            }

            throw new NotImplementedException();
        }
        public object Deserialize(CalendarReader reader)
        {
            if (type.IsPrimitive
                || type == typeof(decimal)
                || type == typeof(string)
                || type == typeof(DateTime)
                || type == (typeof(byte[]))
                || type == typeof(TimeSpan)
                || type == typeof(Guid))
            {
              return  DeserializePrimitive(reader);
            }
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public object GetRealObject(StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class CalendarSerializer<TValue>: CalendarSerializer,  ICalendarSerializer<TValue>
    {
        public CalendarSerializer() : base(typeof(TValue))
        {
        }

        public void Serialize(CalendarWriter writer, TValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            try
            {
                var otype = typeof(TValue);
                if (otype.IsPrimitive
                    || otype == typeof(decimal)
                    || otype == typeof(string)
                    || otype == typeof(DateTime)
                    || otype == (typeof(byte[]))
                    || otype == typeof(TimeSpan)
                    || otype == typeof(Guid))
                {
                    SerializePrimitive(writer, value);
                }

                var serializable = value as ICalendarSerializable;
                if (serializable == null)
                    throw new InvalidOperationException("Object of type:" + otype.FullName + " does not support ICalendarSerializable!");

                serializable.WriteCalendar(writer);
            }
            catch (Exception inner)
            {
                if (inner is ThreadAbortException || inner is StackOverflowException || inner is OutOfMemoryException)
                {
                    throw;
                }
                if (inner is TargetInvocationException)
                {
                    inner = inner.InnerException;
                }
                throw new InvalidOperationException("Calendar Serialization Error", inner);
            }
            writer.Flush();
        }

        public new TValue Deserialize(CalendarReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
