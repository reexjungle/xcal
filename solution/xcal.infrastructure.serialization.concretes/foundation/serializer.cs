using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using reexjungle.xcal.infrastructure.contracts;

namespace reexjungle.xcal.infrastructure.serialization
{
    public class CalendarSerializer: ICalendarSerializer, ISerializable
    {
        private readonly Type type;
        private readonly CultureInfo culture;
        private readonly NumberStyles integerStyles;
        private readonly NumberStyles decimalStyles;

        public CalendarSerializer(Type type): this(type, CultureInfo.InvariantCulture)
        {

        }

        public CalendarSerializer(Type type, CultureInfo culture)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            this.type = type;
            this.culture = culture;

            integerStyles = NumberStyles.AllowLeadingSign 
                | NumberStyles.AllowLeadingWhite 
                | NumberStyles.AllowTrailingWhite;

            decimalStyles = NumberStyles.AllowLeadingSign 
                | NumberStyles.AllowDecimalPoint 
                | NumberStyles.AllowExponent;
        }

        protected CalendarSerializer(SerializationInfo info, StreamingContext context)
        {
            var otype = Type.GetType("System.Type");
            if (otype != null)type = (Type)info.GetValue(nameof(type), otype); 


        }

        protected virtual void SerializePrimitive(CalendarWriter writer, object o)
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

                case TypeCode.String:
                    writer.Write((string)o);
                    break;
                default:
                    if (otype == typeof(byte[]))
                        writer.Write(Convert.ToBase64String((byte[])o, Base64FormattingOptions.InsertLineBreaks));
                    if (otype == typeof(Guid)) writer.Write(((Guid)o).ToString());
                    else writer.Write(o);
                    break;
            }

        }

        public virtual void Serialize(CalendarWriter writer, object o)
        {
            if (o == null) return;
            try
            {
                var otype = o.GetType();
                if (otype.IsPrimitive
                    || otype == typeof (string)
                    || otype == (typeof (byte[]))
                    || otype == typeof (Guid))
                {
                    SerializePrimitive(writer, o);
                    return;
                }

                var serializable = o as ICalendarSerializable;
                if (serializable == null)
                    throw new InvalidOperationException("Cannot serialize object of type:" + otype.FullName);

                serializable.WriteCalendar(writer);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException) ex = ex.InnerException;
                throw new InvalidOperationException("Calendar Serialization Error", ex);
            }
            finally
            {
                writer.Flush();
            }
            
        }

        protected virtual object DeserializePrimitive(CalendarReader reader)
        {
            object o = null;
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    o = reader.Value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ? true : false;
                    break;
                case TypeCode.Char:
                    o = char.Parse(reader.Value);
                    break;
                case TypeCode.SByte:
                    o = sbyte.Parse(reader.Value,  integerStyles, culture);
                    break;
                case TypeCode.Byte:
                    o = byte.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.Int16:
                    o = short.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.UInt16:
                    o = ushort.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.Int32:
                    o = int.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.UInt32:
                    o = uint.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.Int64:
                    o = long.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.UInt64:
                    o = ulong.Parse(reader.Value, integerStyles, culture);
                    break;
                case TypeCode.Single:
                    o = float.Parse(reader.Value, decimalStyles, culture);
                    break;
                case TypeCode.Double:
                    o = double.Parse(reader.Value, decimalStyles, culture);
                    break;
                case TypeCode.Decimal:
                    o = decimal.Parse(reader.Value, decimalStyles, culture);
                    break;
                case TypeCode.String:
                    o = reader.Value;
                    break;
                default:
                    if(type == typeof(byte[]))  o = Convert.FromBase64String(reader.Value);
                    if (type == typeof(Guid)) o = new Guid(reader.Value);
                    break;
            }



            return o;
        }

        public virtual object Deserialize(CalendarReader reader)
        {
            if (type.IsPrimitive || type == (typeof(byte[])) || type == typeof(Guid))
                return DeserializePrimitive(reader);

            object o = null;
            if(typeof(ICalendarSerializable).IsAssignableFrom(type))
            {
                o = Activator.CreateInstance(type, true);
                var serializable = o as ICalendarSerializable;
                serializable?.ReadCalendar(reader);
            }
            return o;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(type), type);
        }
    }

    public class CalendarSerializer<TValue>: CalendarSerializer,  ICalendarSerializer<TValue>
    {
        public CalendarSerializer() : base(typeof(TValue))
        {
        }

        public void Serialize(TValue value, CalendarWriter writer)
        {
            Serialize(writer, value);
        }

        public new TValue Deserialize(CalendarReader reader) => (TValue)base.Deserialize(reader);
    }
}
