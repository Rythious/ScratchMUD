﻿using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class SayCommand : ICommand
    {
        internal const string NAME = "say";
        private readonly PlayerContext playerContext;

        public string Name { get; } = NAME;

        internal SayCommand(PlayerContext playerContext)
        {
            this.playerContext = playerContext;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(params string[] parameters)
        {
            var output = (CommunicationChannel.Self, string.Empty);

            if (parameters.Length > 0)
            {
                output = (CommunicationChannel.Everyone, $"{playerContext.Name} says \"{string.Join(" ", parameters)}\"");
            }

            return Task.Run(() => new List<(CommunicationChannel, string)> { output });
        }

        public string SyntaxHelp()
        {
            return "SAY <VALUE>";
        }

        public string GeneralHelp()
        {
            return "Your character speaks to the other characters in the room.";
        }
    }
}