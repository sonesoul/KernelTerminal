namespace KernelTerminal
{
    public enum WindowStyle : byte
    {
        Default = 0,
        ButtonsHidden = 1 << 0,
        ProcessHidden= 1 << 1,
    }
}