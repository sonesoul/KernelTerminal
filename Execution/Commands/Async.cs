using System.Threading.Tasks;

namespace KernelTerminal.Execution.Commands
{
    internal struct Async : ICommand
    {
        public readonly void Execute(Instruction instruction)
        {
            Task.Run(() => Executor.Execute(instruction.Raw));
        }
    }
}