using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public class RoomEditCommand : ICommand
    {
        internal const string NAME = "roomedit";
        private readonly EditingState editingState;
        private readonly PlayerContext playerContext;

        public string Name { get; } = NAME;

        internal RoomEditCommand(
            EditingState editingState,
            PlayerContext playerContext
        )
        {
            this.editingState = editingState;
            this.playerContext = playerContext;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(params string[] parameters)
        {
            var output = new List<(CommunicationChannel, string)>();

            if (parameters.Length == 0)
            {
                output.Add((CommunicationChannel.Self, EnterEditingModeWithResponse()));
            }
            else if (parameters.Length == 1)
            {
                if (parameters[0].ToLower() == "exit")
                {
                    output.Add((CommunicationChannel.Self, ExitEditingModeWithResponse()));
                }
            }

            if (output.Count == 0)
            {
                output.Add((CommunicationChannel.Self, $"Invalid syntax of {Name.ToUpper()} command: " + SyntaxHelp()));
            }

            return Task.Run(() => output);
        }

        private string EnterEditingModeWithResponse()
        {
            //TODO: See if the current player is listed as an editor for the current area.

            if (editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? editType))
            {
                return $"Player is already editing a { editType }!";
            }
            else
            {
                editingState.AddPlayerEditor(playerContext.Name, EditType.Room);

                return "You are editing the room.";
            }
        }

        private string ExitEditingModeWithResponse()
        {
            editingState.RemovePlayerEditor(playerContext.Name);

            return "You are no longer editing the room.";
        }

        public string GeneralHelp()
        {
            return "If the user has sufficient editing permissions for the current area, they will enter editing mode of their current room.  The Exit subcommand will exit this mode.";
        }

        public string SyntaxHelp()
        {
            return "ROOMEDIT or ROOMEDIT EXIT";
        }
    }
}
