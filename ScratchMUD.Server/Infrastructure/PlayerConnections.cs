using System.Collections.Generic;

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
    }
}