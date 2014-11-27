using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace reexmonkey.infrastructure.io.concretes
{
    /// <summary>
    /// Represents an enumeration of cyptographic hash algorithms
    /// </summary>
    public enum HashAlgorithmMode
    {
        SHA1 = 0x0001,
        SHA256 = 0x0002,
        SHA384 = 0x0004,
        SHA512 = 0x0008,
        MD5 = 0x0010
    }

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
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="origin">Contains the directory that defines the start of the relative path.</param>
        /// <param name="destination">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>Acknowledgements to: http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path</remarks>  
        public static string GetRelativePath(this string origin, string destination)
        {
            if (origin == destination) return Path.GetFileName(origin);
            Uri relativeUri = new Uri(origin).MakeRelativeUri(new Uri(destination));
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            return relativePath.Replace('/', Path.DirectorySeparatorChar);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string GetCheckSum(this string path, HashAlgorithmMode mode)
        {
            byte[] checksum = null;
            using (var sr = new StreamReader(path))
            {
                switch (mode)
                {
                    case HashAlgorithmMode.MD5:
                        var md5h = new MD5CryptoServiceProvider();
                        checksum = md5h.ComputeHash(sr.BaseStream);
                        break;
                    case HashAlgorithmMode.SHA1:
                        var sha1h = new SHA1CryptoServiceProvider();
                        checksum = sha1h.ComputeHash(sr.BaseStream);
                        break;
                    case HashAlgorithmMode.SHA256:
                        var sha2h = new SHA256CryptoServiceProvider();
                        checksum = sha2h.ComputeHash(sr.BaseStream);
                        break;
                    case HashAlgorithmMode.SHA384:
                        var sha3h = new SHA384CryptoServiceProvider();
                        checksum = sha3h.ComputeHash(sr.BaseStream);
                        break;
                    case HashAlgorithmMode.SHA512:
                        var sha5h = new SHA512CryptoServiceProvider();
                        checksum = sha5h.ComputeHash(sr.BaseStream);
                        break;
                    default:
                        var defh = new SHA1CryptoServiceProvider();
                        checksum = defh.ComputeHash(sr.BaseStream);
                        break;

                }

            }

            return BitConverter.ToString(checksum);
        }

        public static string ReadTextLinesFromFile(this string path)
        {
            var sb = new StringBuilder();
            using (var sr = File.OpenText(path))
            {
                var line = string.Empty;
                while ((line = sr.ReadLine()) != null) sb.Append(line); 
            }
            return sb.ToString();
        }


        public static string ReadText(this Stream stream)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(stream))
            {
                var line = string.Empty;
                while ((line = sr.ReadLine()) != null) sb.Append(line);
            }
            return sb.ToString();
        }

        public static byte[] ReadBytes(System.IO.Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new System.IO.MemoryStream())
            {
                int read = 0;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }


        public static IEnumerable<byte[]> SplitToLines(this byte[] bytes, int len)
        {
            List<byte[]> lines = null;
            try
            {
                int offset = 0;
                int count = bytes.Length / len;
                int rem = bytes.Length % len;
                lines = (rem == 0) ? new List<byte[]>(count) : new List<byte[]>(count + 1);
                while (offset < bytes.Length)
                {
                    var buffer = new byte[len];
                    Buffer.BlockCopy(bytes, offset, buffer, 0, len);
                    lines.Add(buffer);
                    offset += len;
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (DivideByZeroException) { throw; }
            catch (Exception) { throw; }
            return lines;
        }
    }

}
