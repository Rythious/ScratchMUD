using System;
using System.Collections.Generic;
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

        public void AddConnectedPlayer(ConnectedPlayer connectedPlayer)
        {
            if (connectedPlayer.SignalRConnectionId == null)
            {
                throw new ArgumentException($"ConnectedPlayer {connectedPlayer.Name} is missing a SignalR connection id.");
            }

            ConnectedPlayers[connectedPlayer.SignalRConnectionId] = connectedPlayer;
        }

        public int GetAvailablePlayerCharacterId()
        {
            return AvailablePlayerCharacterIds.Dequeue();
        }

        public List<ConnectedPlayer> GetConnectedPlayersInARoom(int roomId)
        {
            var allConnectedPlayersInSameRoom = ConnectedPlayers.Where(cp => cp.Value.RoomId == roomId).Select(cp => cp.Value).ToList();

            return allConnectedPlayersInSameRoom;
        }
    }
}