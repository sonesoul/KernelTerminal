namespace KernelTerminal.Execution
{
    public readonly struct InvocationData
    {
        public string Keyword { get; }
        public Instruction Instruction { get; }

        public InvocationData(string keyword, Instruction instruction)
        {
            Keyword = keyword;
            Instruction = instruction;
        }
    }
}