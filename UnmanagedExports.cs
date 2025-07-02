using System;
using System.Runtime.InteropServices;

namespace KernelTerminal
{
    public static class UnmanagedExports
    {
        [UnmanagedCallersOnly(EntryPoint = nameof(OpenTerminal))]
        public static bool OpenTerminal(int style) => Terminal.Open((WindowStyle)style);

        [UnmanagedCallersOnly(EntryPoint = nameof(CloseTerminal))]
        public static bool CloseTerminal() => Terminal.Close();


        [UnmanagedCallersOnly(EntryPoint = nameof(SetTerminalFontSize))]
        public static void SetTerminalFontSize(short width, short height) => Terminal.SetFontSize(width, height);
        [UnmanagedCallersOnly(EntryPoint = nameof(SetTerminalVisible))]
        public static void SetTerminalVisible(int visible) => Terminal.SetVisible(visible != 0);


        [UnmanagedCallersOnly(EntryPoint = nameof(GetWindowHandle))]
        public static IntPtr GetWindowHandle() => Terminal.WindowHandle ?? new IntPtr(-1);
    }
}