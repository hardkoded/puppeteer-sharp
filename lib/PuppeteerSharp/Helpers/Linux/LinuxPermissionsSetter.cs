#if NETSTANDARD2_0

using Mono.Unix;

namespace PuppeteerSharp.Helpers.Linux
{
    internal static class LinuxPermissionsSetter
    {
        internal static readonly FileAccessPermissions ExecutableFilePermissions =
            FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.UserExecute |
            FileAccessPermissions.GroupRead |
            FileAccessPermissions.GroupExecute |
            FileAccessPermissions.OtherRead |
            FileAccessPermissions.OtherExecute;

        public static void SetExecutableFilePermissions(string revisionInfoExecutablePath)
        {
            var unixFileSystemInfo = UnixFileSystemInfo.GetFileSystemEntry(revisionInfoExecutablePath);

            unixFileSystemInfo.FileAccessPermissions = ExecutableFilePermissions;
        }
    }
}

#else

namespace PuppeteerSharp.Helpers.Linux
{
    internal static class LinuxPermissionsSetter
    {   
        public static void SetExecutableFilePermissions(string revisionInfoExecutablePath)
        {
        }
    }
}

#endif