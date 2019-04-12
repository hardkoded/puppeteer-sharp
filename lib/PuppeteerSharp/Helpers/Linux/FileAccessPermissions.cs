using System;

namespace PuppeteerSharp.Helpers.Linux
{
    [Flags]
    internal enum FileAccessPermissions : uint
    {
        OtherExecute = 1,
        OtherWrite = 2,
        OtherRead = 4,
        
        GroupExecute = 8,
        GroupWrite = 16,
        GroupRead = 32,
        
        UserExecute = 64,
        UserWrite = 128,
        UserRead = 256,

        OtherReadWriteExecute = OtherRead | OtherWrite | OtherExecute,
        GroupReadWriteExecute = GroupRead | GroupWrite | GroupExecute,
        UserReadWriteExecute = UserRead | UserWrite | UserExecute,

        DefaultPermissions = OtherWrite | OtherRead | GroupWrite | GroupRead | UserWrite | UserRead,
        AllPermissions = DefaultPermissions | OtherExecute | GroupExecute | UserExecute
    }
}