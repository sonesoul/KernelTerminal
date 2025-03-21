using System;

namespace KernelTerminal.Execution.Commands
{
    public class SetSize : Command
    {
        public SetSize(Instruction instruction) : base(instruction) { }    
        public override void Execute()
        {
            var args = Instruction.Split();

            if (args.Count < 1)
            {
                Terminal.WriteLine($"{Console.WindowWidth}x{Console.WindowHeight}");

                return;
            }

            if (args.Count >= 1)
                Console.WindowWidth = int.Parse(args[0]);

            if (args.Count >= 2)
                Console.WindowHeight = int.Parse(args[1]);
        }
    }
}