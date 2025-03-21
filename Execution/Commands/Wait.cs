using System.Threading.Tasks;

namespace KernelTerminal.Execution.Commands
{
    public class Wait : Command
    {
        public Wait(Instruction instruction) : base(instruction) { }
        public override void Execute() => Task.Delay(int.Parse(RawString)).Wait();
    }
}