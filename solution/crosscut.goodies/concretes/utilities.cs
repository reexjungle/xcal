using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace reexmonkey.crosscut.goodies.concretes
{
    /// <summary>
    /// Specifies a helper class providing common IO functionalities
    /// </summary>
    public static class Utilities
    {

        #region assembly helpers

        /// <summary>
        /// Gets the assembly for a specified type
        /// </summary>
        /// <param name="type">The specified type, whose assembly is being searched</param>
        /// <returns></returns>
        public static Assembly GetAssembly(this Type type)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.GetAssembly(type);
            }
            catch (System.ArgumentNullException)
            {
                throw;
            }

            return assembly;
        }

        /// <summary>
        /// Gets the name of assembly for the given object type
        /// </summary>
        /// <param name="type">The type of the object</param>
        /// <returns>The full path to the assembly</returns>
        public static string GetAssemblyPath(this Type type)
        {
            string path = null;
            try
            {
                path = Assembly.GetAssembly(type).Location;
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }
            return path;
        }

        /// <summary>
        /// Gets the name of assembly for the given object
        /// </summary>
        /// <param name="target">The object, whose assembly name is being sought</param>
        /// <returns>The full path to the assembly</returns>
        public static string GetAssemblyPath(this object target)
        {
            string path = string.Empty;
            try
            {
                path = Assembly.GetAssembly(target.GetType()).Location;
            }
            catch (System.ArgumentNullException)
            {
                throw;
            }
            return path;
        }

        public static string GetAssemblyPath<TValue>(this TValue value)
        {
            string path = string.Empty;
            try
            {
                path = Assembly.GetAssembly(typeof(TValue)).Location;
            }
            catch (System.ArgumentNullException)
            {
                throw;
            }
            return path;
        }

        public static string GetAssemblyPath<T>()
        {
            string path = string.Empty;
            try
            {
                path = Assembly.GetAssembly(typeof(T)).Location;
            }
            catch (System.ArgumentNullException)
            {
                throw;
            }
            return path;
        }


        public static string[] GetAssembyPaths(this Type[] types)
        {
            IEnumerable<string> paths = null;
            try
            {
                paths = types.Select(x => Assembly.GetAssembly(x).Location);

            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }

            return paths.ToArray();
        }

        public static string[] GetAssembyPaths(this object[] objects)
        {
            IEnumerable<string> paths = null;
            try
            {
                paths = objects.Select(x => Assembly.GetAssembly(x.GetType()).Location);

            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }

            return paths.ToArray();
        }

        /// <summary>
        /// Gets the application path of current executing application
        /// </summary>
        /// <returns>The application path</returns>
        public static string GetApplicationPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        #endregion

        #region generators

        /// <summary>
        /// Generates a time stamp from the current UTC date time object.
        /// Default format is ddMMyyyy_HHmmssff
        /// </summary>
        /// <returns>A time stamp value</returns>
        public static string GenerateTimeStamp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:ddMMyyyy_HHmmssff}", System.DateTime.UtcNow);
            return sb.ToString();
        }

        /// <summary>
        /// Generates a time stamp from a date time source
        /// </summary>
        /// <param name="source">The date time object providing the temporal value for the time stamp</param>
        /// <param name="format">The format of the generated time stamp</param>
        /// <returns>A time stamp value</returns>
        public static string GenerateTimeStamp(this DateTime source, string format)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(format, source);
            return sb.ToString();
        }

        /// <summary>
        /// Generates a new global unique identifier
        /// </summary>
        /// <returns>The string representation of the Guid</returns>
        public static string GenerateGUID()
        {
            var guid = Guid.NewGuid();
            return guid.ToString();
        }

        #endregion

        #region encoding and decoding helpers

        /// <summary>
        /// Converts a Base64 string to its equivalent raw binary value.
        /// </summary>
        /// <param name="base64">The base64 text (encoded) that is to be decoded</param>
        /// <returns>Raw binary data decoded from the Base64 text</returns>
        /// <exception cref="ArgumentNullException">Thrown when the plain text argument is null</exception>
        /// <exception cref="ArgumentException">Thrown when conversion from Base64 to raw binary data fails</exception>
        /// <exception cref="FormatException">Thrown when conversion from Base64 to raw binary data fails</exception>
        public static IEnumerable<byte> DecodeToBytes(this string base64)
        {
            IEnumerable<byte> bytes = null;
            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch (ArgumentNullException ex) { throw; }
            catch (FormatException ex) { throw; }
            return bytes;

        }

        #endregion
        
        #region miscellaneous

        /// <summary>
        /// Extracts hexadecimal digits from a string
        /// </summary>
        /// <param name="source">The hexadecimal string from which the digits are extracted</param>
        /// <returns>An array of extracted hexadecimal digits</returns>
        public static char[] ExtractHexDigits(this string source)
        {
            var regex = new Regex("[abcdefABECDEF\\d]+", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            var extracted = from hex in source
                            where regex.IsMatch(hex.ToString())
                            select hex;

            return extracted.ToArray();
        }

        #endregion

    }
}
