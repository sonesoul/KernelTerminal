namespace KernelTerminal.Execution.Commands
{
    internal class Write : Command
    {
        public bool AppendNewLine { get; set; }

        public Write(Instruction instruction, bool appendNewLine) : base(instruction) => AppendNewLine = appendNewLine;
        public override void Execute()
        {
            if (AppendNewLine)
                Terminal.WriteLine(RawString);
            else 
                Terminal.Write(RawString);
        }
    }
}