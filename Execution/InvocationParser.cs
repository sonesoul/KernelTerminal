using System;
using System.Collections.Generic;
using System.Linq;

namespace KernelTerminal.Execution
{
    public class InvocationParser
    {
        public char OpenBracket { get; set; } = '(';
        public char CloseBracket { get; set; } = ')';
        public char Splitter { get; set; } = ',';

        public virtual InvocationData Parse(string invocation)
        {
            invocation = invocation.Trim();

            int openBracket = invocation.IndexOf(OpenBracket);
            int closeBracket = invocation.LastIndexOf(CloseBracket);

            string keyword = invocation;

            if ((openBracket < 0) ^ (closeBracket < 0) || closeBracket < openBracket)
                throw new InvalidSyntaxException();

            Instruction instruction;

            if (openBracket > 0)
            {
                keyword = invocation[..openBracket].Trim();
                string raw = invocation.Substring(openBracket + 1, closeBracket - openBracket - 1);

                instruction = new Instruction(
                    raw,
                    GetArgs(raw));
            }
            else
            {
                instruction = new(string.Empty, new());
            }

            return new InvocationData(keyword, instruction);
        }

        public virtual List<string> GetArgs(string raw)
        {
            List<string> args = new(); 

            if (raw.Length < 1)
                return args;

            int depth = 0;
            int startIndex = 0;

            for (int i = 0; i < raw.Length; i++)
            {
                char c = raw[i];

                if (c == CloseBracket)
                {
                    if (depth < 1)
                        throw new InvalidSyntaxException();

                    depth--;
                    continue;
                }

                if (c == OpenBracket)
                {
                    depth++;
                    continue;
                }

                if (c == Splitter && depth == 0)
                {
                    args.Add(raw[startIndex..i]);
                    startIndex = i + 1;
                }
            }

            args.Add(raw[startIndex..]);
            return args;
        }
    }
}