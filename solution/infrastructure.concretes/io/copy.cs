using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace reexmonkey.infrastructure.io.concretes
{
    /// <summary>
    /// Provides extended object copying functionalities
    /// </summary>
    public static class CopyExtensions
    {
        /// <summary>
        /// Creates a deep clone of an object specified by a generic type parameter
        /// </summary>
        /// <typeparam name="TValue">The type parameter of the object to be cloned </typeparam>
        /// <param name="value">The object of a specified generic type parameter</param>
        /// <returns></returns>
        public static TValue Clone<TValue>(this TValue value)
            where TValue: ISerializable, new()
        {
            if (value == null) throw new ArgumentNullException("value");

            object copy = null;
            using (var ms = new MemoryStream())
            {
                try
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(ms, value);
                    ms.Position = 0;
                    copy = bf.Deserialize(ms);
                }
                catch (SerializationException ){throw;}
                catch (Exception){throw;}
            }
            return (TValue) copy;
        }
    }
}
