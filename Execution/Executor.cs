using System.Collections.Generic;

namespace KernelTerminal.Execution
{
    public static class Executor
    {
        public static InvocationParser Parser { get; set; } = new();

        public static Dictionary<string, ICommand> Commands { get; } = new()
        {
        };

        public static void Execute(string invocation)
        {
            var data = Parser.Parse(invocation);

            if (!Commands.TryGetValue(data.Keyword, out var command))
                throw new KeyNotFoundException($"No command '{data.Keyword}' found.");

            command.Execute(data.Instruction);
        }
    }
}