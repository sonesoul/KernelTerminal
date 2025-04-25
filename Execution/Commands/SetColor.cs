using System;

namespace KernelTerminal.Execution.Commands
{
    internal class SetColor : Command
    {
        public SetColor(Instruction instruction) : base(instruction) { }
        public override void Execute()
        {
            if (RawString.Length == 0)
            {
                Terminal.WriteLine(Terminal.ForegroundColor.ToString());
            }
            else
            {
                var args = Instruction.Split();
                if (args.Count == 2)
                {
                    var type = args[0];
                    ConsoleColor color = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), args[1], true);

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