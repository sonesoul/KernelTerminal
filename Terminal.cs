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

        /// <summary>
        /// Gets or sets the foreground color of the text in the console.
        /// </summary>
        public static ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
        /// <summary>
        /// Gets or sets the background color of the text in the console.
        /// </summary>
        public static ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }

        public static bool IsOpened { get; private set; } = false;
        public static string Title { get; set; } = "Terminal";

        public static Action Opened { get; set; }
        public static Action Closed { get; set; }

        /// <summary>
        /// Gets a value indicating the current handle of the console window.
        /// </summary>
        ///<returns>Console window handle of the console if it is opened; otherwise, null.</returns>
        public static IntPtr? WindowHandle { get; private set; } = null;

        private static WindowStyle _currentStyle;

        #endregion

        public static bool Open(WindowStyle style = WindowStyle.Default)
        {
            if (IsOpened)
                return false;

            if (!NativeInterop.AllocConsole())
                return false;

            IntPtr windowHandle;
            WindowHandle = windowHandle = NativeInterop.GetNewWindow();

            SetStyle(style);

            _currentStyle = style;
            
            ForegroundColor = ConsoleColor.Yellow;

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

        public static WindowStyle GetCurrentStyle() => _currentStyle;

        public static void SetStyle(WindowStyle style)
        {
            if (!WindowHandle.HasValue)
                return;

            NativeInterop.SetButtonsVisible(
                WindowHandle.Value,
                !style.HasFlag(WindowStyle.ButtonsHidden));

            NativeInterop.SetTabVisible(
                WindowHandle.Value, 
                !style.HasFlag(WindowStyle.TabHidden));
        }

        public static void SetFontSize(short width, short height) => NativeInterop.SetFontSize(width, height);
        public static void SetVisible(bool visible)
        {
            if (!WindowHandle.HasValue)
                return;

            NativeInterop.SetVisible(WindowHandle.Value, visible);
        }
    }
}