using System;
using System.Runtime.InteropServices;

namespace PuppeteerSharp.Helpers.Linux
{
    internal static class LinuxSysCall
    {
        [DllImport("libc", SetLastError = true, EntryPoint = "chmod")]
        internal static extern int Chmod(string path, uint mode);
    }
}