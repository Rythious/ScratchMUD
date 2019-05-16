using Microsoft.Extensions.Configuration;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
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
        private readonly string connectionString;

        internal RoomEditCommand(
            EditingState editingState,
            PlayerContext playerContext,
            IConfiguration configuration
        )
        {
            this.editingState = editingState;
            this.playerContext = playerContext;

            connectionString = configuration.GetValue<string>("ConnectionStrings:ScratchMudServer");
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
                var response = string.Empty;

                switch (parameters[0].ToLower())
                {
                    case "title":
                        response = await UpdateTitleWithResponse(string.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    case "short-description":
                        response = await UpdateShortDescriptionWithResponse(string.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    case "full-description":
                        response = await UpdateFullDescriptionWithResponse(string.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    default:
                        break;
                }

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

        private async Task<string> UpdateTitleWithResponse(string title)
        {
            if (editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? _))
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        //TODO: This is terrible and needs to be removed once an ORM is set up.
                        using (var command = new SqlCommand($"UPDATE ScratchMUD.dbo.RoomTranslation SET Title = '{title}' WHERE RoomId = 1", connection))
                        {
                            await command.ExecuteNonQueryAsync();

                            return "Title updated."
;
                        }
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

        private async Task<string> UpdateShortDescriptionWithResponse(string shortDescription)
        {
            if (editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? _))
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        //TODO: This is terrible and needs to be removed once an ORM is set up.
                        using (var command = new SqlCommand($"UPDATE ScratchMUD.dbo.RoomTranslation SET ShortDescription = '{shortDescription}' WHERE RoomId = 1", connection))
                        {
                            await command.ExecuteNonQueryAsync();

                            return "Short description updated."
;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //TODO: Log this exception
                    return $"Error updating room short description. Exception: {ex.Message}";
                }
            }
            else
            {
                return "Cannot update room short description without entering room edit mode.";
            }
        }

        private async Task<string> UpdateFullDescriptionWithResponse(string fullDescriptionValue)
        {
            if (editingState.IsPlayerCurrentlyEditing(playerContext.Name, out EditType? _))
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        //TODO: This is terrible and needs to be removed once an ORM is set up.
                        using (var command = new SqlCommand($"UPDATE ScratchMUD.dbo.RoomTranslation SET FullDescription = '{fullDescriptionValue}' WHERE RoomId = 1", connection))
                        {
                            await command.ExecuteNonQueryAsync();

                            return "Full description updated."
;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //TODO: Log this exception
                    return $"Error updating room full description. Exception: {ex.Message}";
                }
            }
            else
            {
                return "Cannot update room full description without entering room edit mode.";
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