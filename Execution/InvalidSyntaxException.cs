using System;

namespace KernelTerminal.Execution
{
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException() : base("Invalid syntax.") { }
        public InvalidSyntaxException(string expected) : base($"Invalid syntax. Expected: {expected}") { }
    }
}