using Microsoft.AspNetCore.SignalR;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly PlayerContext playerContext;
        private readonly IRoomRepository roomRepository;
        private readonly ICommandRepository commandRepository;

        public EventHub(
            PlayerContext playerContext,
            IRoomRepository roomRepository,
            ICommandRepository commandRepository
        )
        {
            this.playerContext = playerContext;
            this.roomRepository = roomRepository;
            this.commandRepository = commandRepository;
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
            playerContext.CurrentRoomId = 1;

            await ExecuteClientCommand(message);
        }

        private async Task ExecuteClientCommand(string message)
        {
            var command = CommandParser.SplitCommandFromParameters(message, out string[] parameters);

            try
            {
                var outputMessages = await commandRepository.ExecuteAsync(playerContext, command, parameters);

                await SendMessagesToProperChannels(outputMessages);
            }
            catch (ArgumentException ex)
            {
                var output = (CommunicationChannel.Self, $"{ex.Message}");

                await SendMessagesToProperChannels(new List<(CommunicationChannel CommChannel, string Message)> { output });
            }  
        }

        private async Task SendMessagesToProperChannels(IEnumerable<(CommunicationChannel CommChannel, string Message)> outputMessages)
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
            var roomDescription = roomRepository.GetRoomFullDescription(1);

            Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomMessage", $"{roomDescription}");
        }
    }
}