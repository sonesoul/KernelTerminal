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

        /// <summary>
        /// Gets a value indicating whether the console is currently opened.
        /// </summary>
        /// <returns>true if the console is opened; otherwise, false.</returns>
        public static bool IsOpened { get; private set; } = false;
        
        /// <summary>
        /// Gets or sets a value indicating whether to hide the console's system buttons.
        /// </summary>
        public static bool HideButtons { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether to hide the console from the taskbar.
        /// </summary>
        public static bool HideFromTaskbar { get; set; } = true;

        /// <summary>
        /// Gets or sets the title of the console window.
        /// </summary>
        public static string Title { get; set; } = "Terminal";

        /// <summary>
        /// Occurs when the console is opened.
        /// </summary>
        public static Action Opened { get; set; }
        /// <summary>
        /// Occurs when the console is closed.
        /// </summary>
        public static Action Closed { get; set; }

        /// <summary>
        /// Gets a value indicating the current handle of the console window.
        /// </summary>
        ///<returns>Console window handle of the console if it is opened; otherwise, null.</returns>
        public static IntPtr? WindowHandle { get; private set; } = null;

        private readonly static object _writeLock = new object();
        
        #endregion

        #region Open/Close
        /// <summary>
        /// Opens the console if it is not already open.
        /// </summary>
        /// <returns>true if the console was successfully opened; otherwise, false.</returns>
        public static bool Open()
        {
            if (IsOpened)
                return false;

            bool opened = NativeInterop.AllocConsole();

            if (!opened)
                return false;

            CreateWindow();

            IsOpened = true;

            Opened?.Invoke();

            return true;
        }
        /// <summary>
        /// Closes the console if open.
        /// </summary>
        /// <returns>true if the console was successfully closed; otherwise, false.</returns>
        public static bool Close()
        {
            if (!IsOpened)
                return false;

            bool closed = NativeInterop.FreeConsole();

            if (!closed)
                return false;
            
            NativeInterop.ResetConsole();

            WindowHandle = null;

            IsOpened = false;
            Closed?.Invoke();

            return true;
        }
        /// <summary>
        /// Toggles the console state (opens or closes it).
        /// </summary>
        public static void Toggle()
        {
            if (IsOpened)
                Close();
            else
                Open();
        }
        /// <summary>
        /// Closes the console if it is open and opens a new one.
        /// </summary>
        public static void New()
        {
            if (IsOpened)
                Close();

            Open();
        }
        #endregion

        private static void CreateWindow()
        {
            IntPtr windowHandle;
            WindowHandle = windowHandle = NativeInterop.GetNewWindow();

            if (HideButtons)
            {
                NativeInterop.HideButtons(windowHandle);
            }
            if (HideFromTaskbar)
            {
                NativeInterop.HideFromTaskbar(windowHandle);
            }

            ForegroundColor = ConsoleColor.Yellow;

            Console.Title = Title;
            Console.OutputEncoding = Encoding.UTF8;
        }

        #region Output

        /// <summary>
        /// Writes the specified value to the console without a newline, using the specified foreground color.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static void Write(string value, ConsoleColor foreColor = ConsoleColor.Gray, ConsoleColor backColor = ConsoleColor.Black)
        {
            WriteColored(() => Console.Write(value), foreColor, backColor);
        }
        /// <summary>
        /// Writes the specified value to the console followed by a newline, using the specified foreground color.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static void WriteLine(string value, ConsoleColor foreColor = ConsoleColor.Gray, ConsoleColor backColor = ConsoleColor.Black)
        {
            WriteColored(() => Console.WriteLine(value), foreColor, backColor);
        }
        /// <summary>
        /// Writes a newline symbol to the console.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
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