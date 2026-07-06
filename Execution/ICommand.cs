namespace KernelTerminal.Execution
{
    public interface ICommand
    {
        public void Execute(Instruction i);
    }
}