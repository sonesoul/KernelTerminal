using System;

namespace KernelTerminal
{
    internal class SystemConsoleIO : ITerminalIO
    {
        private readonly object _readLock = new();
        private readonly object _writeLock = new();

        public void Write(string value)
        {
            lock (_writeLock) 
                Console.Write(value);
        }
        public void WriteLine(string value)
        {
            lock (_writeLock) 
                Console.WriteLine(value);
        }
        public void Clear()
        {
            lock(_writeLock) 
                Console.Clear();
        }

        public string ReadLine()
        {
            lock (_readLock) 
                return Console.ReadLine();
        }
        public ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            lock (_readLock) 
                return Console.ReadKey(intercept);
        }
    }
}