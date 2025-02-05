using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KernelTerminal
{
    internal class NativeInterop
    {
        #region ConsoleConstants
        public const uint STD_INPUT_HANDLE = 0xFFFFFFF6;
        public const uint STD_OUTPUT_HANDLE = 0xFFFFFFF5;
        public const uint STD_ERROR_HANDLE = 0xFFFFFFF4;
        #endregion

        #region WindowConstants
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;
        public const int GWL_EXSTYLE = -20;

        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_APPWINDOW = 0x00040000;

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        #endregion

        #region Kernel32
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, uint wAttributes);
        #endregion

        #region User32
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion

        private readonly static TextReader _originalIn = Console.In;
        private readonly static TextWriter _originalOut = Console.Out;
        private readonly static TextWriter _originalErr = Console.Error;

        public static void HideButtons(IntPtr handle)
        {
            int style = GetWindowLong(handle, GWL_STYLE);
            style &= ~WS_SYSMENU;
            _ = SetWindowLong(handle, GWL_STYLE, style);
        }
        public static void HideFromTaskbar(IntPtr handle) 
        {
            int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
            _ = SetWindowLong(handle, GWL_EXSTYLE, (exStyle & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW);
        }

        public static void ResetConsole()
        {
            Console.SetOut(_originalOut);
            Console.SetError(_originalErr);
            Console.SetIn(_originalIn);
        }
        public static IntPtr GetNewWindow()
        {
            Console.SetIn(
                new StreamReader(
                    new FileStream(
                        new SafeFileHandle(GetStdHandle(STD_INPUT_HANDLE), false), FileAccess.Read)));
            
            Console.SetOut(
                new StreamWriter(
                    new FileStream(
                        new SafeFileHandle(GetStdHandle(STD_OUTPUT_HANDLE), false), FileAccess.Write))
                { AutoFlush = true });

            Console.SetError(
                new StreamWriter(
                    new FileStream(
                        new SafeFileHandle(GetStdHandle(STD_ERROR_HANDLE), false), FileAccess.Write))
                { AutoFlush = true });

            return GetConsoleWindow();
        }
    }
}