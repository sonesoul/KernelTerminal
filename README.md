KernelTerminal is a lightweight library that uses native interop to manage a console window.\
\
It can be useful when you need a console in a non-console application, or if you want to get around any wrappers (like Windows Terminal) or make the opened console a main application. To do it, you need to create a console project, then hop into the project properties -> Output type and select "Windows application" instead of "Console application". Then simply call `Terminal.Open` at the beginning of the Main method.\
\
The console integrates with `System.Console` in C#, meaning that once it's open, you can use `Console.WriteLine` or any other Console methods/properties seamlessly.

## Known Issue
If you opened the console window as seconadry window of an application and the console is closed manually or its process is terminated (e.g., through Task Manager), 
the **main application thread is also terminated**. This happens because the console process is linked to the main application thread, and closing it forces the entire application to stop.
To avoid this, **do not close the console manually** — use `Terminal.Close()` instead. You can also hide the buttons and the process of the console — there is WindowStyle enum to manage it:
```cs
//the buttons and the process are visible
Terminal.Open(WindowStyle.Default);

//the buttons are hidden but the process still visible
Terminal.Open(WindowStyle.ButtonsHidden); 

//the process is hidden and three different buttons replaced by single X
Terminal.Open(WindowStyle.ProcessHidden);

// no buttons and no visible window in taskbar, Alt+Tab or the Applications tab in Task Manager
Terminal.Open(WindowStyle.ButtonsHidden | WindowStyle.ProcessHidden);
```
You also can use the | operator to combine flags.
