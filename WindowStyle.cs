namespace KernelTerminal
{
    public enum WindowStyle : byte
    {
        None = 0,
        ButtonsHidden = 1 << 0,
        ProcessHidden= 1 << 1,
    }
}