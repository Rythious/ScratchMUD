using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly IConfiguration configuration;
        private readonly PlayerContext playerContext;
        private readonly IDictionary<string, ICommand> commandDictionary;

        public EventHub(
            IConfiguration configuration,
            PlayerContext playerContext,
            EditingState editingState
        )
        {
            this.configuration = configuration;
            this.playerContext = playerContext;
            commandDictionary = new Dictionary<string, ICommand>
            {
                [RoomEditCommand.NAME] = new RoomEditCommand(editingState, playerContext, configuration),
                [SayCommand.NAME] = new SayCommand(playerContext)
            };

            commandDictionary[HelpCommand.NAME] = new HelpCommand(commandDictionary);
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(() => SendMessage($"A new client has connected on {Context.ConnectionId}."));
            GetPlayerRoom();

            return base.OnConnectedAsync();
        }

        //TODO: This will be replaced with a method that sends some sort of event object.
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveServerCreatedMessage", message);
        }

        public async Task RelayClientMessage(string message)
        {
            playerContext.Name = Context.ConnectionId;

            await ExecuteClientCommand(message);
        }

        private async Task ExecuteClientCommand(string message)
        {
            var command = CommandParser.SplitCommandFromParameters(message, out string[] parameters);

            var outputMessages = new List<(CommunicationChannel CommChannel, string Message)>();

            if (commandDictionary.ContainsKey(command))
            {
                outputMessages = await commandDictionary[command].ExecuteAsync(parameters);
            }
            else
            {
                outputMessages.Add((CommunicationChannel.Self, $"'{command}' is not a valid command"));
            }

            await SendMessagesToProperChannels(outputMessages);
        }

        private async Task SendMessagesToProperChannels(List<(CommunicationChannel CommChannel, string Message)> outputMessages)
        {
            foreach (var outputItem in outputMessages)
            {
                if (outputItem.CommChannel == CommunicationChannel.Self)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveServerCreatedMessage", outputItem.Message);
                }
                else if (outputItem.CommChannel == CommunicationChannel.Everyone)
                {
                    await Clients.All.SendAsync("ReceiveServerCreatedMessage", outputItem.Message);
                }
            }
        }

        private void GetPlayerRoom()
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:ScratchMudServer");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT TOP 1 FullDescription FROM ScratchMUD.dbo.RoomTranslation", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomMessage", $"{reader.GetString(0)}");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomMessage", $"{ex.ToString()}");
            }
        }
    }
}