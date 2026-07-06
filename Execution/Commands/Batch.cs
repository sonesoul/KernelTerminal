namespace KernelTerminal.Execution.Commands
{
    internal struct Batch : ICommand
    {
        public readonly void Execute(Instruction i)
        {
            foreach (var item in i.Args)
            {
                Executor.Execute(item);
            }
        }
    }
}