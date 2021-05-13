using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using OSECore.Text;

namespace OSECore.IO
{
    /// <summary>
    /// This class contains a number of static methods that implement file system operations.
    /// </summary>
    public class FileSupport
    {
        /// <summary>
        /// This method determines whether the file indicated by the path argument is writable. To be writable in this context
        /// means that the file can be either modified, appended to, overwritten or deleted.
        /// </summary>
        /// <param name="path">The path to the file to test for writability.  The path may be either relative or absolute.</param>
        /// <returns>True, if the file is writable.</returns>
        public static bool IsFileWritable(string path)
        {
            if (IsValidPath(path))
            {
                string fullpath = GetFullPath(path);
                string folderpath = Path.GetDirectoryName(fullpath);
                if (DoesFileExist(fullpath))
                {
                    return IsNormalFile(fullpath) && !IsReadOnly(fullpath) && HasFilePermission(fullpath, FileSystemRights.Write);
                }
                else if (DoesFolderExist(folderpath))
                {
                    return HasFolderPermission(folderpath, FileSystemRights.Write);
                }
            }
            return false;
        }

        public static bool IsFolderWritable(string path)
        {
            if (IsValidPath(path))
            {
                string fullpath = GetFullPath(path);
                if (DoesFolderExist(fullpath))
                {
                    return HasFolderPermission(fullpath, FileSystemRights.Modify);
                }
            }

            return false;
        }
        /// <summary>
        /// Determines wheter the file indicated by the path argument is a normal file.  To be a normal file in this context
        /// means that the file is not a directory and is also not encrypted, hidden, temporary or a system file.
        /// </summary>
        /// <param name="fullpath">full path to the file to be tested.</param>
        /// <returns>True, if the indicated file is a normal file.</returns>
        public static bool IsNormalFile(string fullpath)
        {
            FileAttributes fa = File.GetAttributes(fullpath);
            FileAttributes specialFile = FileAttributes.Directory | FileAttributes.Encrypted | FileAttributes.Hidden | FileAttributes.System | FileAttributes.Temporary;
            return (fa & specialFile) == 0;
        }
        /// <summary>
        /// Determines whether the the file indicated by the path is readable.  To be readable means that the file can be opened to an input stream and bytes can be
        /// read from the stream.
        /// </summary>
        /// <param name="path">The relative or absolute path to the file to be tested.</param>
        /// <returns>True, if the file is readable.</returns>
        public static bool IsFileReadable(string path)
        {
            if (IsValidPath(path))
            {
                string fullpath = GetFullPath(path);
                if (DoesFileExist(fullpath))
                {
                    return HasFilePermission(fullpath, FileSystemRights.Read);
                }
            }
            return false;
        }

        public static bool IsFolderReadable(string path)
        {
            if(IsValidPath(path))
            {
                string fullpath = GetFullPath(path);
                if (DoesFolderExist(fullpath))
                {
                    return HasFolderPermission(fullpath, FileSystemRights.Read);
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether a file has the indicated file system right.
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="right">The file system right.</param>
        /// <returns>True, if the file has the indicated right.</returns>
        public static bool HasFilePermission(string path, FileSystemRights right)
        {
            FileSecurity fs = new FileSecurity(path,AccessControlSections.Access);
            return HasPermission(fs, right);
        }
        /// <summary>
        /// Determines whether a directory has the indicated file system right.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="right">The file system right.</param>
        /// <returns>True, if the indicated directory has the file system right.</returns>
        public static bool HasFolderPermission(string path, FileSystemRights right)
        {
            DirectorySecurity ds = new DirectorySecurity(path, AccessControlSections.Access);
            return HasPermission(ds, right);
        }
        /// <summary>
        /// Determines whether the indicated file system security object has the indicated file system right.
        /// </summary>
        /// <param name="fss">The file system security object.</param>
        /// <param name="right">The file system right.</param>
        /// <returns>True, if the indicated file system security object has the indicated file system right.</returns>
        /// <remarks>The current Windows user identity is used to search the security object's ACL for 
        /// relevent allow or deny rules.  To have permission for the indicated right, the object's ACL
        /// list must contain an explicit allow rule and no deny rules for either the user identity or a group to which
        /// the user belongs.</remarks>
        private static bool HasPermission(FileSystemSecurity fss, FileSystemRights right)
        {
            AuthorizationRuleCollection rules = fss.GetAccessRules(true, true, typeof(SecurityIdentifier));
            var groups = WindowsIdentity.GetCurrent().Groups;
            SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
            bool explicitAllow = false;
            bool explicitDeny = false;
            foreach (FileSystemAccessRule rule in rules.OfType<FileSystemAccessRule>())
            {
                if ((rule.FileSystemRights & right) == right)
                {
                    if (rule.IdentityReference == user || (groups != null && groups.Contains(rule.IdentityReference)))
                    {
                        if (rule.AccessControlType == AccessControlType.Allow)
                        {
                            explicitAllow = true;
                        }
                        else if (rule.AccessControlType == AccessControlType.Deny)
                        {
                            explicitDeny = true;
                        }
                    }
                }
            }
            return explicitAllow && !explicitDeny;
        }
        /// <summary>
        /// Determines if the read-only file attribute is set.
        /// </summary>
        /// <param name="fullpath">The full path to the file</param>
        /// <returns>True, if the file has the read only attribute set.</returns>
        public static bool IsReadOnly(string fullpath)
        {
            FileAttributes fa = File.GetAttributes(fullpath);
            return (fa & FileAttributes.ReadOnly) != 0;
        }
        /// <summary>
        /// Determines whether a directory exists.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>True, if the directory exists.</returns>
        public static bool DoesFolderExist(string path)
        {
            return Directory.Exists(path);
        }
        /// <summary>
        /// Determines whether a file exists.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>True, if the file exists.</returns>
        public static bool DoesFileExist(string path)
        {
            return File.Exists(path);
        }
        /// <summary>
        /// Determine whether the path is valid.
        /// </summary>
        /// <param name="path">The path to be validated.</param>
        /// <returns>True, if the path is valid.</returns>
        /// <remarks>To be valid the path must represent a well formed absolute or relative path.  The path
        /// cannot contain any illegal path characters and cannot be empty or start or end in a white space character character.</remarks>
        public static bool IsValidPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return false;
            else if (char.IsWhiteSpace(path.First()) || char.IsWhiteSpace(path.Last()))
                return false;
            else if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;
            else if (!IsValidFilename(Path.GetFileName(path)))
                return false;
            else
                return true;
        }
        public static bool IsValidFilename(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return false;
            else if (filename.Trim() != filename)
                return false;
            else if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Get the full path for the indicated relative path.
        /// </summary>
        /// <param name="path">the relative path</param>
        /// <returns>An absolute path created from the relative path.</returns>
        /// <remarks>An absolute path is rooted, that is begins with a '\'. If the supplied path
        /// is already rooted, it is simply returned.  Otherwise the current working directory path is
        /// prepended to the path and returned.</remarks>
        public static string GetFullPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                return Path.Combine(Directory.GetCurrentDirectory(), path);
            }
        }
        /// <summary>
        /// Create a valid file name out of a string.
        /// </summary>
        /// <param name="name">The provided name</param>
        /// <param name="substitute">The character or characters used to substitue for illegal characters.  If empty string, then the
        ///                          illegal characters are simply removed.</param>
        /// <param name="defaultName">The name returned if the provided name is null or empty.</param>
        /// <returns>The name converted into a valid file name.</returns>
        /// <remarks>If the name contains invalid characters, these characters are replaced with '-' characters. Invalid
        /// characters include any control character such as newline, path characters (\\,/,:), wildcard characters
        /// (?,*), pipe characters (&lt;,&gt;,|) or double quote.</remarks>
        public static string MakeValidFileName(string name, string substitute = "-", string defaultName = "untitled")
        {
            if (!String.IsNullOrEmpty(name))
            {
                char[] invalidChars = Path.GetInvalidFileNameChars();
                StringBuilder sb = new StringBuilder();
                int i = 0;
                while (i < name.Length)
                {
                    int j = name.IndexOfAny(invalidChars, i);
                    if (j < 0)
                    {
                        sb.Append(name.Substring(i));
                        break;
                    }
                    else if (j > i)
                    {
                        sb.Append(name.Substring(i, j - i));
                        sb.Append(substitute.SafeString());
                        i = j + 1;
                    }
                    else
                    {
                        sb.Append(substitute.SafeString());
                        i = j + 1;
                    }
                }
                return sb.ToString();
            }
            else
            {
                return defaultName.SafeString();
            }
        }
    }
}
