using Microsoft.AspNetCore.SignalR;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly PlayerContext playerContext;
        private readonly ScratchMUDContext scratchMUDContext;
        private readonly IDictionary<string, ICommand> commandDictionary;

        public EventHub(
            PlayerContext playerContext,
            EditingState editingState,
            ScratchMUDContext scratchMUDContext
        )
        {
            this.playerContext = playerContext;
            this.scratchMUDContext = scratchMUDContext;
            commandDictionary = new Dictionary<string, ICommand>
            {
                [RoomEditCommand.NAME] = new RoomEditCommand(editingState, playerContext, scratchMUDContext),
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
            var room = scratchMUDContext.RoomTranslation.First();

            Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomMessage", $"{room.FullDescription}");
        }
    }
}