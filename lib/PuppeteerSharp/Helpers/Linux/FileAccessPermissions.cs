using System;

namespace PuppeteerSharp.Helpers.Linux
{
    [Flags]
    internal enum FileAccessPermissions : uint
    {
        /// <summary>
        /// Other execution permission.
        /// </summary>
        OtherExecute = 1,

        /// <summary>
        /// Other write permission.
        /// </summary>
        OtherWrite = 2,

        /// <summary>
        /// Other read permission.
        /// </summary>
        OtherRead = 4,

        /// <summary>
        /// Group execution permission.
        /// </summary>
        GroupExecute = 8,

        /// <summary>
        /// Group write permission.
        /// </summary>
        GroupWrite = 16,

        /// <summary>
        /// Group read permission.
        /// </summary>
        GroupRead = 32,

        /// <summary>
        /// User execution permission.
        /// </summary>
        UserExecute = 64,

        /// <summary>
        /// User writer permission.
        /// </summary>
        UserWrite = 128,

        /// <summary>
        /// User read permission.
        /// </summary>
        UserRead = 256,

        /// <summary>
        /// Other read, write and execution permissions.
        /// </summary>
        OtherReadWriteExecute = OtherRead | OtherWrite | OtherExecute,

        /// <summary>
        /// Group read, write and execution permissions.
        /// </summary>
        GroupReadWriteExecute = GroupRead | GroupWrite | GroupExecute,

        /// <summary>
        /// User read, write and execution permissions.
        /// </summary>
        UserReadWriteExecute = UserRead | UserWrite | UserExecute,

        /// <summary>
        /// Default permissions.
        /// </summary>
        DefaultPermissions = OtherWrite | OtherRead | GroupWrite | GroupRead | UserWrite | UserRead,

        /// <summary>
        /// All permissions.
        /// </summary>
        AllPermissions = DefaultPermissions | OtherExecute | GroupExecute | UserExecute,
    }
}
