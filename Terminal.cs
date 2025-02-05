using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public static ConsoleColor ForeColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
        
        /// <summary>
        /// Gets a value indicating whether the console is currently opened.
        /// </summary>
        /// <returns>true if the console is opened; otherwise, false.</returns>
        public static bool IsOpened { get; private set; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable writing commands that have been executed.
        /// </summary>
        public static bool WriteExecuted { get; set; } = true;
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
        public static string Title { get; set; } = "monoconsole";

        /// <summary>
        /// Gets or sets handler that receives input from console window. Also receives input from the <see cref="Execute(string)"/> or <see cref="ExecuteAsync(string)"/>
        /// </summary>
        public static Action<string> Handler { get; set; }
        /// <summary>
        /// Gets or sets the handler that receives exceptions thrown in <see cref="Execute(string)"/> or <see cref="ExecuteAsync"/> methods.
        /// </summary>
        public static Action<Exception> ExceptionHandler { get; set; }
       
        /// <summary>
        /// Gets a value indicating the current handle of the console window.
        /// </summary>
        ///<returns>Console window handle of the console if it is opened; otherwise, null.</returns>
        public static IntPtr? WindowHandle { get; private set; } = null;


        /// <summary>
        /// Occurs when the console is opened.
        /// </summary>
        public static event Action Opened;
        /// <summary>
        /// Occurs when the console is closed.
        /// </summary>
        public static event Action Closed;

        private readonly static object _writeLock = new object();

        #endregion

        static Terminal() => Task.Run(ConsoleRead);

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

        /// <summary>
        /// Pushes a command to the <see cref="Handler"/>
        /// </summary>
        public static void Execute(string command)
        {
            try
            {
                Task.Run(() =>
                {
                    if (WriteExecuted)
                        WriteLine("> " + command, ConsoleColor.DarkYellow).Wait();

                    Handler?.Invoke(command);
                }).Wait();
            }
            catch (Exception ex)
            {
                WriteError($"{ex.Message} [sync]").Wait();
                ExceptionHandler?.Invoke(ex);
            }

        }
        /// <summary>
        /// Asynchronously pushes a command to the <see cref="Handler"/>
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static async Task ExecuteAsync(string command)
        {
            try
            {
                if (WriteExecuted)
                    await WriteLine("> " + command, ConsoleColor.Blue);

                await Task.Run(() => Handler?.Invoke(command));
            }
            catch (Exception ex)
            {
                await WriteError($"{ex.Message} [async]");
                ExceptionHandler?.Invoke(ex);
            }
        }
        /// <summary>
        /// Executes a command on the specified synchronization context, handling exceptions if they occur.
        /// </summary>
        
        private static void ConsoleRead()
        {
            while (true)
            {
                try
                {
                    string input = Console.ReadLine();

                    if (input == null)
                        continue;

                    Handler?.Invoke(input);
                }
                catch (Exception ex)
                {
                    WriteError($"{ex.Message} [read]").Wait(CancellationToken.None);
                }
            }
        }
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

            ForeColor = ConsoleColor.Yellow;

            Console.Title = Title;
            Console.OutputEncoding = Encoding.UTF8;
        }

        #region Output

        /// <summary>
        /// Writes the specified value to the console without a newline, using the specified text color.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static Task Write<T>(T value, ConsoleColor color = ConsoleColor.Gray)
        {
            void Write() => Console.Write(value.ToString());

            return WriteColoredAsync(Write, color);
        }
        /// <summary>
        /// Writes the specified value to the console followed by a newline, using the specified text color.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static Task WriteLine<T>(T value, ConsoleColor color = ConsoleColor.Gray)
        {
            void Write() => Console.WriteLine(value.ToString());

            return WriteColoredAsync(Write, color);
        }
        /// <summary>
        /// Works like <see cref="WriteLine"/> but uses <see cref="ConsoleColor.Red"/> as second parameter.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        public static Task WriteError(string message) => WriteLine(message, ConsoleColor.Red);

        private static async Task WriteColoredAsync(Action writeAction, ConsoleColor color)
        {
            try
            {
                await Task.Run(() =>
                {
                    lock (_writeLock)
                    {
                        if (IsOpened)
                        {
                            var temp = ForeColor;
                            ForeColor = color;

                            writeAction();

                            ForeColor = temp;
                        }
                    }
                });
            }
            catch (IOException)
            {
                //Invalid handle exceptions
            }
        }
        #endregion
    }
}