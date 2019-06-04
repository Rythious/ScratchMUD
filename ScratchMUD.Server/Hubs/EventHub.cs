using Microsoft.AspNetCore.SignalR;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly ICommandRepository commandRepository;
        private readonly IPlayerConnections playerConnections;
        private readonly IPlayerRepository playerRepository;

        public EventHub(
            ICommandRepository commandRepository,
            IPlayerConnections playerConnections,
            IPlayerRepository playerRepository
        )
        {
            this.commandRepository = commandRepository;
            this.playerConnections = playerConnections;
            this.playerRepository = playerRepository;
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(() => SendMessageToProperChannel((CommunicationChannel.Everyone, $"A new client has connected on {Context.ConnectionId}.")));

            var availableCharacterId = playerConnections.GetAvailablePlayerCharacterId();
            var playerCharacter = playerRepository.GetPlayerCharacter(availableCharacterId);
            playerConnections.AddConnectedPlayer(Context.ConnectionId, new ConnectedPlayer(playerCharacter));

            Task.Run(() => SendMessageToProperChannel((CommunicationChannel.Self, $"You are playing as {playerCharacter.Name}.")));

            ExecuteClientCommand(LookCommand.NAME).GetAwaiter().GetResult();

            return base.OnConnectedAsync();
        }

        public async Task RelayClientMessage(string message)
        {
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
                var player = playerConnections.GetConnectedPlayerByConnectionId(Context.ConnectionId);

                var outputMessages = await commandRepository.ExecuteCommandAsync(player, command, parameters);

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
                var output = (CommunicationChannel.Self, ex.Message);

                await SendMessagesToProperChannels(new List<(CommunicationChannel CommChannel, string Message)> { output });
            }
            catch (InvalidCommandSyntaxException ex)
            {
                var output = (CommunicationChannel.Self, ex.Message);

                await SendMessagesToProperChannels(new List<(CommunicationChannel CommChannel, string Message)> { output });
            }
        }

        public async Task SendMessageToProperChannel((CommunicationChannel CommChannel, string Message) channeledMessage)
        {
            await SendMessagesToProperChannels(new List<(CommunicationChannel, string)> { channeledMessage });
        }

        private async Task SendMessagesToProperChannels(IEnumerable<(CommunicationChannel CommChannel, string Message)> outputMessages, string clientReturnMethod = "ReceiveServerCreatedMessage")
        {
            foreach (var outputItem in outputMessages)
            {
                if (outputItem.CommChannel == CommunicationChannel.Self)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(clientReturnMethod, outputItem.Message);
                }
                else if (outputItem.CommChannel == CommunicationChannel.Room)
                {
                    List<string> connectionsInSameRoom = playerConnections.GetConnectedPlayersInTheSameRoomAsAConnection(Context.ConnectionId);

                    await Clients.Clients(connectionsInSameRoom).SendAsync(clientReturnMethod, outputItem.Message);
                }
                else if (outputItem.CommChannel == CommunicationChannel.Everyone)
                {
                    await Clients.All.SendAsync(clientReturnMethod, outputItem.Message);
                }
            }
        }
    }
}