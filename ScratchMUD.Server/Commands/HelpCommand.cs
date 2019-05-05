using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public class HelpCommand : ICommand
    {
        internal const string NAME = "help";
        private readonly IDictionary<string, ICommand> commandDictionary;

        public string Name { get; } = NAME;

        internal HelpCommand(IDictionary<string, ICommand> commandDictionary)
        {
            this.commandDictionary = commandDictionary;
        }

        public Task<List<string>> ExecuteAsync(params string[] parameters)
        {
            var output = new List<string>();

            if (parameters.Length == 0)
            {
                var availableCommands = BuildListOfAllAvailableCommands();

                output = availableCommands;
            }
            else if (parameters.Length == 1)
            {
                if (commandDictionary.ContainsKey(parameters[0]))
                {
                    output.Add(commandDictionary[parameters[0]].SyntaxHelp());
                    output.Add(commandDictionary[parameters[0]].GeneralHelp());
                }
            }

            return Task.Run(() => output);
        }

        private List<string> BuildListOfAllAvailableCommands()
        {
            var commandNames = new List<string> { "Available commands list" };

            foreach (var item in commandDictionary.OrderBy(d => d.Key))
            {
                commandNames.Add(item.Key);
            }

            return commandNames;
        }

        public string SyntaxHelp()
        {
            return "HELP or HELP <COMMAND>";
        }

        public string GeneralHelp()
        {
            return "Returns helpful information about available commands.";
        }
    }
}
