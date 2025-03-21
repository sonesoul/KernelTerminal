namespace KernelTerminal.Execution
{
    public abstract class Command
    {
        public static char OpenBracket { get; set; } = '(';
        public static char CloseBracket { get; set; } = ')';
        public static char Splitter { get; set; } = ',';

        public Instruction Instruction { get; set; }
        public string RawString => Instruction.RawString;

        protected Command(Instruction instruction) => Instruction = instruction;

        public abstract void Execute();
    }
}
