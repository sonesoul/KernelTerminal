# Monoconsole
Monoconsole is a lightweight library for opening and managing a console window in non-console applications.
It allows interaction with the console, handles user input, displays messages with customizable colors, and executes commands asynchronously.


The console integrates with `System.Console` in C#, meaning that once it's open, you can use `Console.WriteLine` or any other Console methods/properties seamlessly.
However, it's recommended to use `Terminal.Write` and `Terminal.WriteLine` primarily

### Installation
Reference the compiled .dll file in your project:
In Visual Studio, right-click on your project in the Solution Explorer, choose "Add Reference...", and locate the .dll file.

Add the using directive in the files where you want to use the library:
```csharp
using Monoconsole;
```

### Open/Close
You can open the console by calling:
```csharp
Terminal.Open();
```
To close the console:
```csharp
Terminal.Close();
```
You can toggle the console open/close state:
```csharp
Terminal.Toggle();
```
And you can recreate console window:
```csharp
Terminal.New();
```

### Logic
You can assign an input handler for processing input in a basic console logic and the `Execute`/`ExecuteAsync` methods:
```csharp
Terminal.Handler = (input) => 
{
    Terminal.WriteLine($"Received: {input}");
};
```

To execute commands either synchronously or asynchronously:

```csharp
Terminal.Execute("your command"); //blocks the thread on which it was called
await Terminal.ExecuteAsync("your async command");
```

### Colors
You can modify text colors by setting:
```csharp
Monoconsole.ForeColor = ConsoleColor.Green; //sets the color of characters
Monoconsole.BackColor = ConsoleColor.White; //sets the background color of characters 
Monoconsole.InfoColor = ConsoleColor.Blue; //sets the color that used in the Monoconsole.WriteInfo
Monoconsole.ErrorColor = ConsoleColor.Magenta; //sets the color that used in the Monoconsole.WriteError
```
And you can pass the color as an argument:
```csharp
Monoconsole.WriteLine("Red color", ConsoleColor.Red);
//writes "Red color" using a red color

Monoconsole.Write("Blue color", ConsoleColor.Blue);
//writes "Blue color" using a blue color
```

### Events
Terminal provides several events, such as:

`Opened` – Triggered when the console opens.

`Closed` – Triggered when the console closes.

## Known Issue
When the console window is closed manually or the process is terminated (e.g., through Task Manager), 
the **main application thread is also terminated**. This happens because the console process is linked to the main application thread, and closing it forces the entire application to stop.
To avoid this, **do not close the console manually** — use `Monoconsole.Close()` instead. By default, the console window has no buttons 
(such as close or fullscreen) and does not appear in the taskbar.  You can customize button visibility or show the console window in the taskbar, but do so at your own risk:
```csharp
Terminal.HideFromTaskbar = false; //enables showing in taskbar
Terminal.HideButtons = false; //enables buttons for the window
```
