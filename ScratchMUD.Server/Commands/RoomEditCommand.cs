using Microsoft.EntityFrameworkCore;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class RoomEditCommand : Command, ICommand
    {
        internal const string NAME = "roomedit";
        private readonly string[] VALID_ACTIONS = new string[9]
        {
            "title",
            "short-description",
            "full-description",
            "create-north",
            "create-east",
            "create-south",
            "create-west",
            "create-up",
            "create-down"
        };
        private readonly EditingState editingState;
        private readonly IRoomRepository roomRepository;

        internal RoomEditCommand(
            EditingState editingState,
            IRoomRepository roomRepository
        )
        {
            this.editingState = editingState;
            this.roomRepository = roomRepository;

            Name = NAME;
            SyntaxHelp = "ROOMEDIT or ROOMEDIT EXIT, ROOMEDIT TITLE <VALUE>, ROOMEDIT SHORT-DESCRIPTION <VALUE>, ROOMEDIT FULL-DESCRIPTION <VALUE>, ROOMEDIT CREATE-NORTH, ROOMEDIT CREATE-EAST, ROOMEDIT CREATE-SOUTH, ROOMEDIT CREATE-WEST, ROOMEDIT CREATE-UP, ROOMEDIT CREATE-DOWN";
            GeneralHelp = "If the user has sufficient editing permissions for the current area, they will enter editing mode of their current room.  The Exit subcommand will exit this mode.";
        }

        public async Task<List<(CommunicationChannel, string)>> ExecuteAsync(RoomContext roomContext, params string[] parameters)
        {
            var output = new List<string>();

            if (parameters.Length == 0)
            {
                output.Add(EnterEditingModeWithResponse(roomContext.CurrentCommandingPlayer.Name));
            }
            else if (parameters.Length == 1)
            {
                switch (parameters[0].ToLower())
                {
                    case "exit":
                        output.Add(ExitEditingModeWithResponse(roomContext.CurrentCommandingPlayer.Name));
                        break;
                    case string command when command.StartsWith("create-"):
                        output.Add(await CreateRoomWithResponse(roomContext.CurrentCommandingPlayer, parameters));
                        break;
                    default:
                        output.Add(InvalidSyntaxErrorText);
                        break;
                }
            }
            else //parameters.Length > 1
            {
                output.Add(await UpdateRoomDetailWithResponse(roomContext.CurrentCommandingPlayer, parameters));
            }

            foreach (var message in output)
            {
                roomContext.CurrentCommandingPlayer.QueueMessage(message);
            }

            return new List<(CommunicationChannel, string)>();
        }

        private async Task<string> CreateRoomWithResponse(ConnectedPlayer connectedPlayer, string[] parameters)
        {
            var wasCommandRecognized = true;
            switch (parameters[0].ToLower())
            {
                case "create-north": await CreateNewRoom(connectedPlayer, Directions.North); break;
                case "create-south": await CreateNewRoom(connectedPlayer, Directions.South); break;
                case "create-east": await CreateNewRoom(connectedPlayer, Directions.East); break;
                case "create-west": await CreateNewRoom(connectedPlayer, Directions.West); break;
                case "create-up": await CreateNewRoom(connectedPlayer, Directions.Up); break;
                case "create-down": await CreateNewRoom(connectedPlayer, Directions.Down); break;
                default:
                    wasCommandRecognized = false;
                    break;
            }

            if (wasCommandRecognized)
            {
                return "Room updated.";
            }

            return InvalidSyntaxErrorText;
        }

        private async Task CreateNewRoom(ConnectedPlayer connectedPlayer, Directions direction)
        {
            await roomRepository.CreateNewRoom(connectedPlayer.RoomId, direction);
            connectedPlayer.QueueCommand(direction.ToString());
        }

        private async Task<string> UpdateRoomDetailWithResponse(ConnectedPlayer connectedPlayer, string[] parameters)
        {
            if (VALID_ACTIONS.Contains(parameters[0].ToLower()))
            {
                if (!editingState.IsPlayerCurrentlyEditing(connectedPlayer.Name, out EditType? editType) || editType != EditType.Room)
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
                                await roomRepository.UpdateTitle(connectedPlayer.RoomId, valuePortionOfCommand);
                                break;
                            case "short-description":
                                await roomRepository.UpdateShortDescription(connectedPlayer.RoomId, valuePortionOfCommand);
                                break;
                            case "full-description":
                                await roomRepository.UpdateFullDescription(connectedPlayer.RoomId, valuePortionOfCommand);
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

            return InvalidSyntaxErrorText;
        }

        private string EnterEditingModeWithResponse(string playerName)
        {
            //TODO: See if the current player is listed as an editor for the current area.

            if (editingState.IsPlayerCurrentlyEditing(playerName, out EditType? editType))
            {
                return $"Player is already editing a { editType }!";
            }
            else
            {
                editingState.AddPlayerEditor(playerName, EditType.Room);

                return "You are editing the room.";
            }
        }

        private string ExitEditingModeWithResponse(string playerName)
        {
            editingState.RemovePlayerEditor(playerName);

            return "You are no longer editing the room.";
        }
    }
}