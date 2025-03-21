using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KernelTerminal.Execution.Commands;

namespace KernelTerminal.Execution
{
    public delegate Command CommandCreator(Instruction instruction);
    public class CommandFactory : Dictionary<string, CommandCreator> { }

    public static class Executor
    {
        private readonly static CommandFactory _creators = new CommandFactory();

        public static void SetupTerminal()
        {
            Terminal.HideFromTaskbar = false;
            Terminal.HideButtons = false;

            Terminal.Opened = HandleOpened;
            Terminal.UpdateHandler = Update;
            Terminal.ExceptionHandler = HandleException;

            Terminal.New();

            var factory = new CommandFactory()
            {
                ["size"] = i => new SetSize(i),
                ["color"] = i => new SetColor(i),
                ["write"] = i => new Write(i, false),
                ["writel"] = i => new Write(i, true),

                ["async"] = i => new Async(i),
                ["batch"] = i => new Batch(i),
                ["wait"] = i => new Wait(i),
            };

            RegisterFactory(factory);
        }

        public static Command Create(string stringCommand)
        {
            if (string.IsNullOrWhiteSpace(stringCommand))
                return null;

            stringCommand = stringCommand.Trim();

            int openBracket = stringCommand.IndexOf(Command.OpenBracket);
            int closeBracket = stringCommand.LastIndexOf(Command.CloseBracket);

            string keyword = stringCommand;
            Instruction args = new Instruction(string.Empty);

            if ((openBracket < 0) ^ (closeBracket < 0) || closeBracket < openBracket)
                throw new InvalidSyntaxException();

            if (openBracket > 0)
            {
                keyword = stringCommand.Substring(0, openBracket).Trim();
                args = new Instruction(stringCommand.Substring(openBracket + 1, closeBracket - openBracket - 1));
            }

            Command executable = null;

            if (_creators.TryGetValue(keyword, out CommandCreator creator))
            {
                executable = creator(args);
            }

            return executable;
        }

        public static void RegisterCommand(string name, CommandCreator creator)
        {
            if (_creators.ContainsKey(name))
                throw new Exception($"{name} is already registered");

            _creators[name] = creator;
        }
        public static void RegisterFactory(CommandFactory factory)
        {
            foreach (var item in factory)
            {
                RegisterCommand(item.Key, item.Value);
            }
        }
        public static void UnregisterCommand(string name)
        {
            if (!_creators.Remove(name))
                throw new Exception($"{name} is not registered");
        }
        public static void SetKey(string key, string newKey)
        {
            if (_creators.TryGetValue(key, out CommandCreator creator))
            {
                _creators.Remove(key);
                _creators[newKey] = creator;
            }

        }
        
        public static IEnumerable<string> GetRegisteredCommands() => _creators.Keys;

        private static void Update()
        {
            string input = Console.ReadLine();

            Command instance = Create(input);
            instance?.Execute();
        }
        private static void HandleException(Exception ex) => Terminal.WriteLine(ex.Message, ConsoleColor.Red).Wait();
        private static void HandleOpened()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder();
            var rnd = new Random();

            for (int i = 0; i < 5; i++)
            {
                int index = rnd.Next(chars.Length);
                sb.Append(chars[index]);
            }

            List<ConsoleColor> colors =
                Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>()
                .Where(c => c != ConsoleColor.Black)
                .ToList();

            int colorIndex = rnd.Next(colors.Count);

            Terminal.WriteLine($"| KernelTerminal [{sb}]\n", colors[colorIndex]);
            Console.SetWindowSize(60, 15);
        }
    }
}