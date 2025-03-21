using System.Collections.Generic;

namespace KernelTerminal.Execution
{
    public readonly struct Instruction
    {
        public string RawString { get; }

        public Instruction(string rawString)
        {
            RawString = rawString;
        }

        public List<string> Split()
        {
            var args = new List<string>();

            if (RawString.Length > 0)
            {
                var invalidSyntax = new InvalidSyntaxException();

                int depth = 0;
                int startIndex = 0;

                for (int i = 0; i < RawString.Length; i++)
                {
                    char c = RawString[i];

                    if (c == Command.CloseBracket)
                    {
                        if (depth < 1 || (depth > 0 && i == (RawString.Length - 1)))
                            throw invalidSyntax;

                        depth--;
                        continue;
                    }

                    if (c == Command.OpenBracket)
                    {
                        depth++;
                        continue;
                    }

                    if (c == Command.Splitter && depth == 0)
                    {
                        args.Add(RawString.Substring(startIndex, i - startIndex));
                        startIndex = i + 1;
                    }
                }

                args.Add(RawString.Substring(startIndex));
            }

            return args;
        }
    }
}