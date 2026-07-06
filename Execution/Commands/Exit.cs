namespace KernelTerminal.Execution.Commands
{
    public struct Exit : ICommand
    {
        public readonly void Execute(Instruction i) => Terminal.Close();
    }
}
