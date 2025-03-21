namespace KernelTerminal.Execution.Commands
{
    internal class Exit : Command
    {
        public Exit(Instruction instruction) : base(instruction) { }
        public override void Execute() => Terminal.Close();
    }
}
