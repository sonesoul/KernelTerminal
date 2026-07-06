using System;

namespace KernelTerminal.Execution.Commands
{
    internal struct SetColor : ICommand
    {
        public readonly void Execute(Instruction i)
        {
            if (i.Raw.Length == 0)
            {
                Terminal.WriteLine(Terminal.ForegroundColor.ToString());
            }
            else
            {
                if (i.Args.Count == 2)
                {
                    var type = i.Args[0];
                    ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), i.Args[1], true);

                    if (type == "fore")
                        Terminal.ForegroundColor = color;
                    else if (type == "back")
                        Terminal.BackgroundColor = color;
                    else 
                        throw new InvalidSyntaxException();
                }
            }
        }
    }
}