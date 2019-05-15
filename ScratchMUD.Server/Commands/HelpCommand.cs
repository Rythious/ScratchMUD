﻿using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public class HelpCommand : ICommand
    {
        internal const string NAME = "help";
        private readonly IDictionary<string, ICommand> commandDictionary;

        internal HelpCommand(IDictionary<string, ICommand> commandDictionary)
        {
            this.commandDictionary = commandDictionary;
        }

        #region Syntax, Help, and Name
        public string Name { get; } = NAME;

        public string SyntaxHelp => "HELP or HELP <COMMAND>";

        public string GeneralHelp => "Returns helpful information about available commands.";
        #endregion

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
                    output.Add((CommunicationChannel.Self, commandDictionary[parameters[0]].SyntaxHelp));
                    output.Add((CommunicationChannel.Self, commandDictionary[parameters[0]].GeneralHelp));
                }
                else
                {
                    output.Add((CommunicationChannel.Self, $"No help found for '{parameters[0]}'."));
                }
            }
            else
            {
                output.Add((CommunicationChannel.Self, $"Invalid syntax of {Name.ToUpper()} command: " + SyntaxHelp));
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