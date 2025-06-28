using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KernelTerminal
{
    internal static class NativeInterop
    {
        #region ConsoleConstants
        public const uint STD_INPUT_HANDLE = 0xFFFFFFF6;
        public const uint STD_OUTPUT_HANDLE = 0xFFFFFFF5;
        public const uint STD_ERROR_HANDLE = 0xFFFFFFF4;

        public const int LF_FACESIZE = 32;
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);
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

        #region Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public int cbSize;
            public int nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
            public string FaceName;
        }
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
            SafeFileHandle GetHandle(uint std) => new SafeFileHandle(GetStdHandle(std), ownsHandle: false);

            Console.SetIn(
                new StreamReader(
                    new FileStream(
                        GetHandle(STD_INPUT_HANDLE), FileAccess.Read)));
            
            Console.SetOut(
                new StreamWriter(
                    new FileStream(
                        GetHandle(STD_OUTPUT_HANDLE), FileAccess.Write))
                { AutoFlush = true });

            Console.SetError(
                new StreamWriter(
                    new FileStream(
                        GetHandle(STD_ERROR_HANDLE), FileAccess.Write))
                { AutoFlush = true });

            return GetConsoleWindow();
        }

        public static void SetFontSize(short width, short height)
        {
            IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);

            var cfi = new CONSOLE_FONT_INFO_EX();
            cfi.cbSize = Marshal.SizeOf(cfi);

            GetCurrentConsoleFontEx(hnd, false, ref cfi);

            cfi.dwFontSize.X = width;
            cfi.dwFontSize.Y = height;

            SetCurrentConsoleFontEx(hnd, false, ref cfi);
        }
    }
}