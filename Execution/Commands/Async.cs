using System.Threading.Tasks;

namespace KernelTerminal.Execution.Commands
{
    internal class Async : Command
    {
        public Async(Instruction instruction) : base(instruction) { }
        public override void Execute() => Task.Run(() => Executor.Create(RawString).Execute());
    }
}