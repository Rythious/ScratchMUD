using Microsoft.AspNetCore.SignalR;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var connectedPlayer = new ConnectedPlayer(playerCharacter);

            playerConnections.AddConnectedPlayer(Context.ConnectionId, connectedPlayer);

            Task.Run(() => connectedPlayer.QueueMessage($"You are playing as {playerCharacter.Name}."));

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

            var player = playerConnections.GetConnectedPlayerByConnectionId(Context.ConnectionId);
            var playersInRoom = playerConnections.GetConnectedPlayersInTheSameRoomAsAConnection(Context.ConnectionId);

            var roomContext = new RoomContext
            {
                CurrentCommandingPlayer = player,
                OtherPlayersInTheRoom = playersInRoom.Except(new List<ConnectedPlayer> { player })
            };

            try
            {
                var outputMessages = await commandRepository.ExecuteCommandAsync(roomContext, command, parameters);

                if (string.IsNullOrEmpty(overrideClientReturnMethod))
                {
                    await SendMessagesToProperChannels(outputMessages);
                }
                else
                {
                    await SendMessagesToProperChannels(outputMessages, overrideClientReturnMethod);
                }

                roomContext.AllPlayersInTheRoom.ToList().ForEach(async p => await SendAllQueuedMessages(p));
            }
            catch (ArgumentException ex)
            {
                roomContext.CurrentCommandingPlayer.QueueMessage(ex.Message);

                await SendAllQueuedMessages(roomContext.CurrentCommandingPlayer);
            }
            catch (InvalidCommandSyntaxException ex)
            {
                roomContext.CurrentCommandingPlayer.QueueMessage(ex.Message);

                await SendAllQueuedMessages(roomContext.CurrentCommandingPlayer);
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
                if (outputItem.CommChannel == CommunicationChannel.Everyone)
                {
                    await Clients.All.SendAsync(clientReturnMethod, outputItem.Message);
                }
            }
        }

        private async Task SendAllQueuedMessages(ConnectedPlayer connectedPlayer)
        {
            var connectionId = playerConnections.GetConnectionOfConnectedPlayer(connectedPlayer);

            while (connectedPlayer.MessageQueueCount > 0)
            {
                var message = connectedPlayer.DequeueMessage();

                await Clients.Client(connectionId).SendAsync("ReceiveServerCreatedMessage", message);
            }
        }
    }
}