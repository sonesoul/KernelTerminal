namespace KernelTerminal.Execution.Commands
{
    internal class Batch : Command
    {
        public Batch(Instruction instruction) : base(instruction) { }
        public override void Execute()
        {
            foreach (var item in Instruction.Split())
            {
                Executor.Create(item).Execute();
            }
        }
    }
}