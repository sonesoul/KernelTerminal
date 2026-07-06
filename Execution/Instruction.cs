using System;
using System.Collections.Generic;

namespace KernelTerminal.Execution
{
    public readonly struct Instruction
    {
        public string Raw { get; }
        public List<string> Args { get; }
        
        public Instruction(string rawString, List<string> args)
        {
            Raw = rawString;
            Args = args;
        }
    }
}