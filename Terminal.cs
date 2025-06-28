using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KernelTerminal.Execution;

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
        private readonly static object _writeLock = new object();
        
        #endregion

        public static bool Open(WindowStyle style)
        {
            if (IsOpened)
                return false;

            if (!NativeInterop.AllocConsole())
                return false;

            IntPtr windowHandle;
            WindowHandle = windowHandle = NativeInterop.GetNewWindow();

            if (style.HasFlag(WindowStyle.ButtonsHidden))
            {
                NativeInterop.HideButtons(windowHandle);
            }
            if (style.HasFlag(WindowStyle.ProcessHidden))
            {
                NativeInterop.HideFromTaskbar(windowHandle);
            }

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

            _currentStyle = WindowStyle.None;
            WindowHandle = null;
            IsOpened = false;

            Closed?.Invoke();

            return true;
        }

        public static void New()
        {
            WindowStyle style = _currentStyle;


        }

        public static WindowStyle GetCurrentStyle() => _currentStyle;

        #region Output

        /// <summary>
        /// Writes the specified value to the console without a newline, using the specified foreground color.
        /// </summary>
        public static void Write(string value, ConsoleColor foreColor = ConsoleColor.Gray, ConsoleColor backColor = ConsoleColor.Black)
        {
            WriteColored(() => Console.Write(value), foreColor, backColor);
        }
        /// <summary>
        /// Writes the specified value to the console followed by a newline, using the specified foreground color.
        /// </summary>
        public static void WriteLine(string value, ConsoleColor foreColor = ConsoleColor.Gray, ConsoleColor backColor = ConsoleColor.Black)
        {
            WriteColored(() => Console.WriteLine(value), foreColor, backColor);
        }
        /// <summary>
        /// Writes a newline symbol to the console.
        /// </summary>
        public static void WriteLine() => Write(Environment.NewLine);

        private static void WriteColored(Action writeAction, ConsoleColor foreColor, ConsoleColor backColor)
        {
            lock (_writeLock)
            {
                if (IsOpened)
                {
                    var tempFore = ForegroundColor;
                    var tempBack = BackgroundColor;

                    ForegroundColor = foreColor;
                    BackgroundColor = backColor;

                    try
                    {
                        writeAction();
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        ForegroundColor = tempFore;
                        BackgroundColor = tempBack;
                    }
                }
            }
        }
        #endregion
    }
}