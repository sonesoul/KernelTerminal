using System.Threading.Tasks;

namespace KernelTerminal.Execution.Commands
{
    public struct Async : ICommand
    {
        public readonly void Execute(Instruction instruction)
        {
            Task.Run(() => Executor.Invoke(instruction.Raw));
        }
    }
}