namespace KernelTerminal.Execution.Commands
{
    internal struct Exit : ICommand
    {
        public readonly void Execute(Instruction i) => Terminal.Close();
    }
}
