using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.io
{

    public static class FileExtensions
    {
        /// <summary>
        /// Browse a direction specified by a path and return a collection of files found inside the directory
        /// </summary>
        /// <param name="path">The directory path</param>
        /// <returns>The collection of files from the directory</returns>
        public static IEnumerable<FileInfo> GetFiles(this string path)
        {
            IEnumerable<FileInfo> finfos = null;

            try
            {
                var dinfo = new DirectoryInfo(path);
                finfos = dinfo.GetFiles();
            }
            catch (DirectoryNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return finfos;
        }

        /// <summary>
        /// Browse a direction specified by a path and return a collection of files found inside the directory.
        /// The returned files are filtered according to a specified extension
        /// </summary>
        /// <param name="path">The directory path</param>
        /// <param name="filter">An extension filter that specifies that only files with the extension are selected</param>
        /// <returns>The collection of files in the specified directory that are filtered by the extension</returns>
        public static IEnumerable<FileInfo> GetFiles(this string path, string filter)
        {
            IEnumerable<FileInfo> finfos = null;

            try
            {
                var dinfo = new DirectoryInfo(path);
                var files = dinfo.GetFiles().Where(f => f.Extension == filter).Select(f => f);
            }
            catch (DirectoryNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return finfos;
        }

        /// <summary>
        /// Browse a direction specified by a path and return a collection of files found inside the directory.
        /// The returned files are filtered by specified multiple extensions
        /// </summary>
        /// <param name="path">The directory path</param>
        /// <param name="filters">A collection of extensions to filter the file retrieval result </param>
        /// <returns>The collection of files in the specified directory that have been filtered by the collection of extensions</returns>
        public static IEnumerable<FileInfo> GetFiles(this string path, IEnumerable<string> filters)
        {
            IEnumerable<FileInfo> finfos = null;

            try
            {
                var dinfo = new DirectoryInfo(path);
                var files = dinfo.GetFiles().Where(f => f.Extension == filters.Select(l => l).FirstOrDefault()).Select(f => f);
            }
            catch (DirectoryNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return finfos;
        }

        /// <summary>
        /// Appends a path to a root path 
        /// </summary>
        /// <param name="root">The root path</param>
        /// <param name="path">The path to be appended to the root path</param>
        /// <returns>A new path consisting of the root path and the appendix</returns>
        public static string AppendPath(this string root, string path)
        {
            StringBuilder sb = new StringBuilder(root);
            sb.AppendFormat("\\{0}", path);
            return sb.ToString();
        }
    }

}
