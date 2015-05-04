using System.IO;
using System.Security.AccessControl;

namespace reexjungle.crosscut.security.extensions.concretes
{
    /// <summary>
    /// Helper class that provides common security-related extended features
    /// </summary>
    public static class FileSecurityExtensions
    {
        /// <summary>
        /// Sets the security on a file
        /// </summary>
        /// <param name="path">The path to a file</param>
        /// <param name="account">The user account to authorize the file security</param>
        /// <param name="rights">The rights of the user. Rights must be sufficient to grant permission</param>
        /// <param name="access">The type of control to allow access to the secured file/param>
        public static void SetFileSecurity(this string path, string account, FileSystemRights rights, AccessControlType access)
        {
            var security = File.GetAccessControl(path);
            security.AddAccessRule(new FileSystemAccessRule(account, rights, access));
            File.SetAccessControl(path, security);
        }

        /// <summary>
        /// Sets the security on a directory
        /// </summary>
        /// <param name="directory">The path to the directory</param>
        /// <param name="account">The provided user account to grant permissions </param>
        /// <param name="rights">The rights of the user. Rights must be sufficient to grant permission</param>
        /// <param name="access">The type of control to allow acceess to a secure directory</param>
        public static void SetFolderSecurity(this string path, string account, FileSystemRights rights, AccessControlType access)
        {
            var security = Directory.GetAccessControl(path);
            security.AddAccessRule(new FileSystemAccessRule(account, rights, access));
            Directory.SetAccessControl(path, security);
        }
    }
}