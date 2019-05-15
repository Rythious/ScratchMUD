using Microsoft.Extensions.Configuration;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public class RoomEditCommand : ICommand
    {
        internal const string NAME = "roomedit";
        private readonly EditingState editingState;
        private readonly PlayerContext playerContext;
        private readonly IConfiguration configuration;

        internal RoomEditCommand(
            EditingState editingState,
            PlayerContext playerContext,
            IConfiguration configuration
        )
        {
            this.editingState = editingState;
            this.playerContext = playerContext;
            this.configuration = configuration;
        }

        #region Syntax, Help, and Name
        public string Name { get; } = NAME;

        public string GeneralHelp => "If the user has sufficient editing permissions for the current area, they will enter editing mode of their current room.  The Exit subcommand will exit this mode.";

        public string SyntaxHelp => "ROOMEDIT or ROOMEDIT EXIT, ROOMEDIT TITLE <VALUE>";
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
                switch (parameters[0].ToLower())
                {
                    case "title":
                        var response = await UpdateTitleWithResponse(string.Join(" ", parameters, 1, parameters.Length - 1));
                        output.Add((CommunicationChannel.Self, response));
                        break;
                    default:
                        break;
                }
            }

            if (output.Count == 0)
            {
                output.Add((CommunicationChannel.Self, $"Invalid syntax of {Name.ToUpper()} command: " + SyntaxHelp));
            }

            return output;
        }

        private async Task<string> UpdateTitleWithResponse(string newTitleValue)
        {
            if (editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? editType))
            {
                var connectionString = configuration.GetValue<string>("ConnectionStrings:ScratchMudServer");

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        //TODO: This is terrible and needs to be removed once an ORM is set up.
                        using (var command = new SqlCommand($"UPDATE ScratchMUD.dbo.RoomTranslation SET Title = '{newTitleValue}' WHERE RoomId = 1", connection))
                        {
                            await command.ExecuteNonQueryAsync();

                            return "Title updated."
;                        }
                    }
                }
                catch (SqlException ex)
                {
                    //TODO: Log this exception
                    return $"Error updating room title. Exception: {ex.Message}";
                }
            }
            else
            {
                return "Cannot update room title without entering room edit mode.";
            }
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