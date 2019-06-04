using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class HelpCommand : Command, ICommand
    {
        internal const string NAME = "help";
        private readonly IDictionary<string, ICommand> commandDictionary;

        internal HelpCommand(IDictionary<string, ICommand> commandDictionary)
        {
            this.commandDictionary = commandDictionary;

            Name = NAME;
            SyntaxHelp = "HELP or HELP <COMMAND>";
            GeneralHelp = "Returns helpful information about available commands.";
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(ConnectedPlayer connectedPlayer, IEnumerable<ConnectedPlayer> playersInTheRoom, params string[] parameters)
        {
            var output = new List<(CommunicationChannel, string)>();

            if (parameters.Length == 0)
            {
                var availableCommands = BuildListOfAllAvailableCommands();

                foreach (var command in availableCommands)
                {
                    connectedPlayer.QueueMessage(command);
                }
            }
            else if (parameters.Length == 1)
            {
                if (commandDictionary.ContainsKey(parameters[0]))
                {
                    connectedPlayer.QueueMessage(commandDictionary[parameters[0]].SyntaxHelp);
                    connectedPlayer.QueueMessage(commandDictionary[parameters[0]].GeneralHelp);
                }
                else
                {
                    connectedPlayer.QueueMessage($"No help found for '{parameters[0]}'.");
                }
            }
            else
            {
                connectedPlayer.QueueMessage(InvalidSyntaxErrorText);
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
    }
}