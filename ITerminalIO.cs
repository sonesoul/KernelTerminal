using System;

namespace KernelTerminal
{
    public interface ITerminalIO
    {
        void Write(string value);
        void WriteLine(string value);
        void WriteLine() => Write(Environment.NewLine);

        string ReadLine();
        ConsoleKeyInfo ReadKey(bool intercept = false);
     
        void Clear();
    }
}