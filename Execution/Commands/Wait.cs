using System.Threading.Tasks;

namespace KernelTerminal.Execution.Commands
{
    public struct Wait : ICommand
    {
        public readonly void Execute(Instruction i) => Task.Delay(int.Parse(i.Raw)).Wait();
    }
}