﻿using Microsoft.AspNetCore.SignalR;
using ScratchMUD.Server.Cache;
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
        private readonly IAreaCache areaCache;

        public EventHub(
            ICommandRepository commandRepository,
            IPlayerConnections playerConnections,
            IPlayerRepository playerRepository,
            IAreaCache areaCache
        )
        {
            this.commandRepository = commandRepository;
            this.playerConnections = playerConnections;
            this.playerRepository = playerRepository;
            this.areaCache = areaCache;
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(() => SendMessageToProperChannel((CommunicationChannel.Everyone, $"A new client has connected on {Context.ConnectionId}.")));

            ConnectedPlayer connectedPlayer = GetAvailablePlayerAsConnectedPlayer();

            connectedPlayer.SignalRConnectionId = Context.ConnectionId;

            Task.Run(() => connectedPlayer.QueueMessage($"You are playing as {connectedPlayer.Name}."));

            ExecuteClientCommand(LookCommand.NAME).GetAwaiter().GetResult();

            return base.OnConnectedAsync();
        }

        private ConnectedPlayer GetAvailablePlayerAsConnectedPlayer()
        {
            var availableCharacterId = playerConnections.GetAvailablePlayerCharacterId();

            var playerCharacter = playerRepository.GetPlayerCharacter(availableCharacterId);

            var connectedPlayer = new ConnectedPlayer(playerCharacter);

            connectedPlayer.SignalRConnectionId = Context.ConnectionId;

            playerConnections.AddConnectedPlayer(connectedPlayer);

            return connectedPlayer;
        }

        public async Task ExecuteClientCommand(string message)
        {
            var command = CommandParser.SplitCommandFromParameters(message, out string[] parameters);

            string overrideClientReturnMethod = null;

            //TODO: I need a way for commands to return their own style.
            if (command == LookCommand.NAME)
            {
                overrideClientReturnMethod = "ReceiveRoomMessage";
            }

            var player = playerConnections.GetConnectedPlayerByConnectionId(Context.ConnectionId);
            var playersInRoom = playerConnections.GetConnectedPlayersInARoom(player.RoomId);

            var roomContext = new RoomContext
            {
                CurrentCommandingPlayer = player,
                OtherPlayersInTheRoom = playersInRoom.Except(new List<ConnectedPlayer> { player }),
                NpcsInTheRoom = areaCache.GetNpcsInRoom(player.RoomId)
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

        private async Task SendMessageToProperChannel((CommunicationChannel CommChannel, string Message) channeledMessage)
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
            while (connectedPlayer.MessageQueueCount > 0)
            {
                var message = connectedPlayer.DequeueMessage();

                await Clients.Client(connectedPlayer.SignalRConnectionId).SendAsync("ReceiveServerCreatedMessage", message);
            }
        }
    }
}