﻿using ScratchMUD.Server.Models;
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

        public Task<List<string>> ExecuteAsync(params string[] parameters)
        {
            string output = string.Empty;

            if (parameters.Length > 0)
            {
                output = $"{playerContext.Name} says \"{parameters[0]}\"";
            }

            return Task.Run(() => new List<string> { output });
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