using System;

namespace KernelTerminal.Execution.Commands
{
    public struct Size : ICommand
    {
        public readonly void Execute(Instruction i)
        {
            var args = i.Args;

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