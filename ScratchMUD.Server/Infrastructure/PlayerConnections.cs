﻿using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Infrastructure
{
    internal class PlayerConnections : IPlayerConnections
    {
        private Queue<int> AvailablePlayerCharacterIds { get; } = new Queue<int>();
        private Dictionary<string, ConnectedPlayer> ConnectedPlayers { get; set; } = new Dictionary<string, ConnectedPlayer>();

        public PlayerConnections()
        {
            AvailablePlayerCharacterIds.Enqueue(1);
            AvailablePlayerCharacterIds.Enqueue(2);
            AvailablePlayerCharacterIds.Enqueue(3);
            AvailablePlayerCharacterIds.Enqueue(4);
            AvailablePlayerCharacterIds.Enqueue(5);
        }

        public ConnectedPlayer GetConnectedPlayerByConnectionId(string signalRConnectionId)
        {
            if (ConnectedPlayers.ContainsKey(signalRConnectionId))
            {
                return ConnectedPlayers[signalRConnectionId];
            }

            return null;
        }

        public void AddConnectedPlayer(string signalRConnectionId, ConnectedPlayer connectedPlayer)
        {
            ConnectedPlayers[signalRConnectionId] = connectedPlayer;
        }

        public int GetAvailablePlayerCharacterId()
        {
            return AvailablePlayerCharacterIds.Dequeue();
        }

        public string GetConnectionOfConnectedPlayer(ConnectedPlayer connectedPlayer)
        {
            var connectionId = ConnectedPlayers.Single(cp => cp.Value == connectedPlayer).Key;

            return connectionId;
        }

        public List<ConnectedPlayer> GetConnectedPlayersInTheSameRoomAsAConnection(string connectionId)
        {
            var connectedPlayer = GetConnectedPlayerByConnectionId(connectionId);

            var allConnectedPlayersInSameRoom = ConnectedPlayers.Where(cp => cp.Value.RoomId == connectedPlayer.RoomId).Select(cp => cp.Value).ToList();

            return allConnectedPlayersInSameRoom;
        }
    }
}