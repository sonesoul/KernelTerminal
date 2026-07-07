using System;
using System.Text;

namespace KernelTerminal
{
    /// <summary>
    /// Provides methods and properties to manage a simple console window.
    /// </summary>
    public static class Terminal
    {
        #region Fields

        public static IntPtr? WindowHandle { get; private set; } = null;

        public static bool IsOpened { get; private set; } = false;

        public static string Title { get => _title; set => SetTitle(value); } 
        public static WindowStyle WindowStyle { get => _currentStyle; set => SetStyle(value); }
        public static ITerminalIO IO { get; set; } = new SystemConsoleIO();

        public static int InitialWidth { get; set; } = 400;
        public static int InitialHeight { get; set; } = 250;
        public static int InitialX { get; set; } = 150;
        public static int InitialY { get; set; } = 150;

        public static Action Opened { get; set; }
        public static Action Closed { get; set; }

        private static WindowStyle _currentStyle;
        private static string _title = "Terminal";

        #endregion

        public static bool Open(WindowStyle style = WindowStyle.Default)
        {
            if (IsOpened)
                return false;

            if (!NativeInterop.AllocConsole())
                return false;

            WindowHandle = NativeInterop.GetNewWindow();
            
            Move(
                InitialX, 
                InitialY, 
                InitialWidth, 
                InitialHeight
            );
            
            WindowStyle = style;
            
            Console.Title = Title;
            Console.OutputEncoding = Encoding.UTF8;
            
            IsOpened = true;
            Opened?.Invoke();

            return true;
        }
        public static bool Close()
        {
            if (!IsOpened)
                return false;

            if (!NativeInterop.FreeConsole())
                return false;
            
            NativeInterop.ResetConsole();

            _currentStyle = WindowStyle.Default;
            WindowHandle = null;
            IsOpened = false;

            Closed?.Invoke();

            return true;
        }

        public static void New()
        {
            WindowStyle style = _currentStyle;
         
            if (IsOpened)
                Close();

            Open(style);
        }

        public static void SetFontSize(short width, short height) => NativeInterop.SetFontSize(width, height);
        public static void SetVisible(bool visible)
        {
            if (!WindowHandle.HasValue)
                return;

            NativeInterop.SetVisible(WindowHandle.Value, visible);
        }

        public static void Resize(int w, int h)
        {
            if (!WindowHandle.HasValue)
                return;

            Console.SetWindowSize(w, h);
            Console.SetBufferSize(w, h);
        }
        public static void Move(int x, int y, int w, int h)
        {
            if (!WindowHandle.HasValue)
                return;

            NativeInterop.MoveWindow(x, y, w, h, true);
        }

        #region IO Proxy

        public static void Write(string value) => IO.Write(value);
        public static void WriteLine(string value) => IO.WriteLine(value);
        public static void WriteLine() => IO.WriteLine();
        public static string ReadLine() => IO.ReadLine();
        public static void Clear() => IO.Clear();

        #endregion

        private static void SetStyle(WindowStyle style)
        {
            if (!WindowHandle.HasValue)
                return;

            _currentStyle = style;

            NativeInterop.SetButtonsVisible(
                WindowHandle.Value,
                !style.HasFlag(WindowStyle.ButtonsHidden));

            NativeInterop.SetTabVisible(
                WindowHandle.Value, 
                !style.HasFlag(WindowStyle.TabHidden));
        }
        private static void SetTitle(string title)
        {
            _title = title;

            if (!WindowHandle.HasValue)
                return;

            Console.Title = title;
        }
    }
}