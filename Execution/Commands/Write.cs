namespace KernelTerminal.Execution.Commands
{
    public struct Write : ICommand
    {
        public bool AppendNewLine { get; set; }

        public Write(bool appendNewLine) => AppendNewLine = appendNewLine;
        public readonly void Execute(Instruction i)
        {
            if (AppendNewLine)
                Terminal.WriteLine(i.Raw);
            else
                Terminal.Write(i.Raw);
        }
    }
}