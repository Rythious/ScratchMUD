using Microsoft.EntityFrameworkCore;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public class RoomEditCommand : ICommand
    {
        internal const string NAME = "roomedit";
        private readonly string[] VALID_ACTIONS = new string[3] { "title", "short-description", "full-description" };
        private readonly EditingState editingState;
        private readonly PlayerContext playerContext;
        private readonly ScratchMUDContext scratchMudContext;

        internal RoomEditCommand(
            EditingState editingState,
            PlayerContext playerContext,
            ScratchMUDContext scratchMudContext
        )
        {
            this.editingState = editingState;
            this.playerContext = playerContext;
            this.scratchMudContext = scratchMudContext;
        }

        #region Syntax, Help, and Name
        public string Name { get; } = NAME;

        public string GeneralHelp => "If the user has sufficient editing permissions for the current area, they will enter editing mode of their current room.  The Exit subcommand will exit this mode.";

        public string SyntaxHelp => "ROOMEDIT or ROOMEDIT EXIT, ROOMEDIT TITLE <VALUE>, ROOMEDIT SHORT-DESCRIPTION <VALUE>, ROOMEDIT FULL-DESCRIPTION <VALUE>";
        #endregion

        public async Task<List<(CommunicationChannel, string)>> ExecuteAsync(params string[] parameters)
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
            else //parameters.Length > 1
            {
                var response = await UpdateRoomDetailWithResponse(parameters);

                if (!string.IsNullOrEmpty(response))
                {
                    output.Add((CommunicationChannel.Self, response));
                }
            }

            if (output.Count == 0)
            {
                output.Add((CommunicationChannel.Self, $"Invalid syntax of {Name.ToUpper()} command: " + SyntaxHelp));
            }

            return output;
        }

        private async Task<string> UpdateRoomDetailWithResponse(string[] parameters)
        {
            if (VALID_ACTIONS.Contains(parameters[0].ToLower()))
            {
                if (!editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? editType) || editType != EditType.Room)
                {
                    return "Must be in room edit mode.";
                }
                else
                {
                    var valuePortionOfCommand = string.Join(" ", parameters, 1, parameters.Length - 1);

                    try
                    {
                        switch (parameters[0].ToLower())
                        {
                            case "title":
                                await UpdateTitle(valuePortionOfCommand);
                                break;
                            case "short-description":
                                await UpdateShortDescription(valuePortionOfCommand);
                                break;
                            case "full-description":
                                await UpdateFullDescription(valuePortionOfCommand);
                                break;
                            default:
                                break;
                        }

                        return "Room updated.";
                    }
                    catch (DbUpdateException ex)
                    {
                        return $"Error updating room. Exception: {ex.Message}";
                    }
                }
            }

            return null;
        }

        private async Task UpdateTitle(string title)
        {
            var room = scratchMudContext.RoomTranslation.First(rt => rt.RoomId == 1);
            room.Title = title;
            await scratchMudContext.SaveChangesAsync();
        }

        private async Task UpdateShortDescription(string shortDescription)
        {
            var room = scratchMudContext.RoomTranslation.First(rt => rt.RoomId == 1);
            room.ShortDescription = shortDescription;
            await scratchMudContext.SaveChangesAsync();
        }

        private async Task UpdateFullDescription(string fullDescription)
        {
            var room = scratchMudContext.RoomTranslation.First(rt => rt.RoomId == 1);
            room.FullDescription = fullDescription;
            await scratchMudContext.SaveChangesAsync();
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
    }
}