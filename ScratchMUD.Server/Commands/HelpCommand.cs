using ScratchMUD.Server.Models.Constants;
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

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(params string[] parameters)
        {
            var output = new List<(CommunicationChannel, string)>();

            if (parameters.Length == 0)
            {
                var availableCommands = BuildListOfAllAvailableCommands();

                foreach (var command in availableCommands)
                {
                    output.Add((CommunicationChannel.Self, command));
                }
            }
            else if (parameters.Length == 1)
            {
                if (commandDictionary.ContainsKey(parameters[0]))
                {
                    output.Add((CommunicationChannel.Self, commandDictionary[parameters[0]].SyntaxHelp()));
                    output.Add((CommunicationChannel.Self, commandDictionary[parameters[0]].GeneralHelp()));
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
