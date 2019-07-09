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
            MaximumParameterCount = 1;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(RoomContext roomContext, params string[] parameters)
        {
            ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(parameters);

            var output = new List<(CommunicationChannel, string)>();

            if (parameters.Length == 0)
            {
                var availableCommands = BuildListOfAllAvailableCommands();

                foreach (var command in availableCommands)
                {
                    roomContext.CurrentCommandingPlayer.QueueMessage(command);
                }
            }
            else //parameters.Length == 1
            {
                if (commandDictionary.ContainsKey(parameters[0]))
                {
                    roomContext.CurrentCommandingPlayer.QueueMessage(commandDictionary[parameters[0]].SyntaxHelp);
                    roomContext.CurrentCommandingPlayer.QueueMessage(commandDictionary[parameters[0]].GeneralHelp);
                }
                else
                {
                    roomContext.CurrentCommandingPlayer.QueueMessage($"No help found for '{parameters[0]}'.");
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
    }
}