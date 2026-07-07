namespace KernelTerminal.Execution.Commands
{
    public struct Batch : ICommand
    {
        public readonly void Execute(Instruction i)
        {
            foreach (var item in i.Args)
            {
                Executor.Invoke(item);
            }
        }
    }
}