using Mono.Unix;

namespace PuppeteerSharp.Helpers.Linux
{
    internal static class LinuxPermissionsSetter
    {
        internal static readonly FileAccessPermissions BrowserPermissionsInLinux =
            FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.UserExecute |
            FileAccessPermissions.GroupRead |
            FileAccessPermissions.GroupExecute |
            FileAccessPermissions.OtherRead |
            FileAccessPermissions.OtherExecute;

        public static void SetBrowserPermissionsPermissions(string revisionInfoExecutablePath)
        {
            //Extracted this code into a different class so it doesn't get loaded when in .net471.

            var unixFileSystemInfo = UnixFileSystemInfo.GetFileSystemEntry(revisionInfoExecutablePath);
            unixFileSystemInfo.FileAccessPermissions = BrowserPermissionsInLinux;
        }
    }
}
