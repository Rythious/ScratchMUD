using ScratchMUD.Server.Models;
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

        public Task<List<string>> ExecuteAsync(params string[] parameters)
        {
            var output = new List<string>();

            if (parameters.Length == 0)
            {
                EnterEditingMode(output);
            }
            else if (parameters.Length == 1)
            {
                if (parameters[0].ToLower() == "exit")
                {
                    ExitEditingMode(output);
                }
            }

            return Task.Run(() => output);
        }

        private void EnterEditingMode(List<string> output)
        {
            //TODO: See if the current player is listed as an editor for the current area.

            if (editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? editType))
            {
                output.Add($"Player is already editing a { editType }!");
            }
            else
            {
                editingState.AddPlayerEditor(playerContext.Name, EditType.Room);

                output.Add("You are editing the room.");
            }
        }

        private void ExitEditingMode(List<string> output)
        {
            editingState.RemovePlayerEditor(playerContext.Name);

            output.Add("You are no longer editing the room.");
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
