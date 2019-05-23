using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class MoveCommand : Command, ICommand
    {
        public MoveCommand(Directions direction)
        {
            Name = direction.ToString();
            SyntaxHelp = direction.ToString().ToUpper();
            GeneralHelp = $"If there is an exit to the {direction.ToString().ToLower()}, moves you to that room.";
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(PlayerContext playerContext, params string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}