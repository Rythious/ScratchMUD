using Microsoft.AspNetCore.SignalR;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly PlayerContext playerContext;
        private readonly ICommandRepository commandRepository;

        public EventHub(
            PlayerContext playerContext,
            ICommandRepository commandRepository
        )
        {
            this.playerContext = playerContext;
            this.commandRepository = commandRepository;
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(() => SendMessage($"A new client has connected on {Context.ConnectionId}."));

            playerContext.Name = Context.ConnectionId;
            playerContext.CurrentRoomId = 1;

            ExecuteClientCommand(LookCommand.NAME).GetAwaiter().GetResult();

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

            string overrideClientReturnMethod = null;

            //TODO: I need a way for commands to return their own style.
            if (command == LookCommand.NAME)
            {
                overrideClientReturnMethod = "ReceiveRoomMessage";
            }

            try
            {
                var outputMessages = await commandRepository.ExecuteAsync(playerContext, command, parameters);

                if (string.IsNullOrEmpty(overrideClientReturnMethod))
                {
                    await SendMessagesToProperChannels(outputMessages);
                }
                else
                {
                    await SendMessagesToProperChannels(outputMessages, overrideClientReturnMethod);
                }
            }
            catch (ArgumentException ex)
            {
                var output = (CommunicationChannel.Self, $"{ex.Message}");

                await SendMessagesToProperChannels(new List<(CommunicationChannel CommChannel, string Message)> { output });
            }
        }

        private async Task SendMessagesToProperChannels(IEnumerable<(CommunicationChannel CommChannel, string Message)> outputMessages, string clientReturnMethod = "ReceiveServerCreatedMessage")
        {
            foreach (var outputItem in outputMessages)
            {
                if (outputItem.CommChannel == CommunicationChannel.Self)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(clientReturnMethod, outputItem.Message);
                }
                else if (outputItem.CommChannel == CommunicationChannel.Everyone)
                {
                    await Clients.All.SendAsync(clientReturnMethod, outputItem.Message);
                }
            }
        }
    }
}