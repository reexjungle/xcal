using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace reexmonkey.infrastructure.operations.concretes
{
    public enum StorageType
    { 
        rdbms = 0x1,
        nosql = 0x2,
        unknown = 0xf
    }

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

        public static Assembly GetAssembly<TValue>(this TValue value)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.GetAssembly(typeof(TValue));
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }
            return assembly;
        }

        public static Assembly GetAssembly<TValue>()
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.GetAssembly(typeof(TValue));
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }
            return assembly;
        }

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

        public static string[] GetAssembyPaths<TValue>(this TValue[] objects)
        {
            IEnumerable<string> paths = null;
            try
            {
                paths = objects.Select(x => Assembly.GetAssembly(typeof(TValue)).Location);

            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }

            return paths.ToArray();
        }

        public static string[] GetReferencedAssemblyNames(this Assembly assembly)
        {
            string[] references = null;
            try
            {
                references = assembly.GetReferencedAssemblies().Select(x => string.Format("{0}.dll", x.Name)).ToArray();
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }
            return references;
        }

        public static string[] GetReferencedAssemblyNamesFromEntryAssembly()
        {
            string[] references = null;
            try
            {
                references = Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(x => string.Format("{0}.dll", x.Name)).ToArray();
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }
            return references;
        }

        public static string[] GetReferencedAssemblyNamesFromExecutingAssembly()
        {
            string[] references = null;
            try
            {
                references = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(x => string.Format("{0}.dll", x.Name)).ToArray();
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception) { throw; }
            return references;
        }

        public static FileVersionInfo GetFileVersionInfo(this Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location); 
        }

        public static Version GetVersionInfo(this Assembly assembly)
        {
            return assembly.GetName().Version;
        }
        /// <summary>
        /// Gets the application path of current executing application
        /// </summary>
        /// <returns>The application path</returns>
        public static string GetExecutingAssemblyApplicationPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// Gets the application path of first executable
        /// </summary>
        /// <returns>The application path</returns>
        public static string GetEntryAssemblyApplicationPath()
        {
            return Assembly.GetEntryAssembly().Location;
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
        
    }
}
