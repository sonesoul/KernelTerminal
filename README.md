KernelTerminal is a lightweight library that uses native interop to manage a console window. It can be useful when you need a console in a non-console application.
The console integrates with `System.Console` in C#, meaning that once it's open, you can use `Console.WriteLine` or any other Console methods/properties seamlessly.
However, it's recommended to use `Terminal.Write` and `Terminal.WriteLine` primarily.

## Known Issue
When the console window is closed manually or the process is terminated (e.g., through Task Manager), 
the **main application thread is also terminated**. This happens because the console process is linked to the main application thread, and closing it forces the entire application to stop.
To avoid this, **do not close the console manually** — use `Terminal.Close()` instead. By default, the console window has no buttons 
(such as close or fullscreen) and does not appear in the taskbar.  You can customize button visibility or show the console window in the taskbar, but do so at your own risk:
```csharp
Terminal.HideFromTaskbar = false; //enables showing in taskbar
Terminal.HideButtons = false; //enables buttons for the window
```
