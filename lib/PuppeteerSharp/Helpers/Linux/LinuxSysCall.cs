using System;
using System.Runtime.InteropServices;

namespace PuppeteerSharp.Helpers.Linux
{
    internal static class LinuxSysCall
    {
        [DllImport("libc", SetLastError = true, EntryPoint = "chmod")]
        internal static extern int Chmod(string path, uint mode);

        [DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromFilePermissions")]
        internal static extern int FromFilePermissions(FilePermissions value, out uint rval);

        [DllImport("MonoPosixHelper", EntryPoint = "helper_Mono_Posix_Stat")]
        internal extern static int GetStats(
            string filename,
            bool dereference,
            out int device,
            out int inode,
            out int mode,
            out int nlinks,
            out int uid,
            out int gid,
            out int rdev,
            out long size,
            out long blksize,
            out long blocks,
            out long atime,
            out long mtime,
            out long ctime);

        internal static void SetPermissions(string path, FilePermissions filePermissions)
        {
            FromFilePermissions(filePermissions, out var permissions);
            Chmod(path, permissions);
        }

        internal static FilePermissions GetFileMode(string filename)
        {
            GetStats(
                filename,
                false,
                out var device,
                out var inode,
                out var mode,
                out var nlinks,
                out var uid,
                out var gid,
                out var rdev,
                out var size,
                out var blksize,
                out var blocks,
                out var atime,
                out var mtime,
                out var ctime);

            return (FilePermissions)mode;
        }
    }
}