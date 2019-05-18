using Microsoft.EntityFrameworkCore;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
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
        private readonly IRoomRepository roomRepository;

        internal RoomEditCommand(
            EditingState editingState,
            IRoomRepository roomRepository
        )
        {
            this.editingState = editingState;
            this.roomRepository = roomRepository;
        }

        #region Syntax, Help, and Name
        public string Name { get; } = NAME;

        public string GeneralHelp => "If the user has sufficient editing permissions for the current area, they will enter editing mode of their current room.  The Exit subcommand will exit this mode.";

        public string SyntaxHelp => "ROOMEDIT or ROOMEDIT EXIT, ROOMEDIT TITLE <VALUE>, ROOMEDIT SHORT-DESCRIPTION <VALUE>, ROOMEDIT FULL-DESCRIPTION <VALUE>";
        #endregion

        public async Task<List<(CommunicationChannel, string)>> ExecuteAsync(PlayerContext playerContext, params string[] parameters)
        {
            var output = new List<(CommunicationChannel, string)>();

            if (parameters.Length == 0)
            {
                output.Add((CommunicationChannel.Self, EnterEditingModeWithResponse(playerContext)));
            }
            else if (parameters.Length == 1)
            {
                if (parameters[0].ToLower() == "exit")
                {
                    output.Add((CommunicationChannel.Self, ExitEditingModeWithResponse(playerContext)));
                }
            }
            else //parameters.Length > 1
            {
                var response = await UpdateRoomDetailWithResponse(playerContext, parameters);

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

        private async Task<string> UpdateRoomDetailWithResponse(PlayerContext playerContext, string[] parameters)
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
                                await roomRepository.UpdateTitle(valuePortionOfCommand);
                                break;
                            case "short-description":
                                await roomRepository.UpdateShortDescription(valuePortionOfCommand);
                                break;
                            case "full-description":
                                await roomRepository.UpdateFullDescription(valuePortionOfCommand);
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

        private string EnterEditingModeWithResponse(PlayerContext playerContext)
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

        private string ExitEditingModeWithResponse(PlayerContext playerContext)
        {
            editingState.RemovePlayerEditor(playerContext.Name);

            return "You are no longer editing the room.";
        }
    }
}