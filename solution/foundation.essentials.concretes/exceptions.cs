using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.foundation.essentials.concretes
{
    #region extensions

    public static class ExceptionExtensions
    {
        public static void ThrowIfNull<TValue>(this TValue source, string paramName = null, string message = null)
        {
            if (source == null) throw new ArgumentNullException(paramName, message);
        }

        public static void ThrowIfNull<TValue>(this TValue source, string message, Exception inner = null)
        {
            if (source == null) throw new ArgumentNullException(message, inner);
        }

        public static void ThrowIfNullOrEmpty<TValue>(this IEnumerable<TValue> source, string message, Exception inner = null)
        {
            if (source.NullOrEmpty()) throw new ArgumentException(message, inner);
        }
    }
 
    #endregion

    #region helpers

    /// <summary>
    /// Provides generic exception services
    /// </summary>
    /// <typeparam name="TValue">The type parameter for the generic exception</typeparam>
    public class TException<TValue> : Exception
    {
        private Type type = null;

        /// <summary>
        /// Gets the data type of the generic operator, T
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TException()
            : base()
        {
            type = typeof(TValue);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        public TException(string message)
            : base(message)
        {
            type = typeof(TValue);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="inner">The inner exception of this execption</param>
        public TException(string message, Exception inner)
            : base(message, inner)
        {
            type = typeof(TValue);
        }
    } 
    #endregion
}
