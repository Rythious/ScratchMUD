using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public interface IPlayerConnections
    {
        ConnectedPlayer GetConnectedPlayerByConnectionId(string signalRConnectionId);
        int GetAvailablePlayerCharacterId();
        void AddConnectedPlayer(ConnectedPlayer connectedPlayer);
        List<ConnectedPlayer> GetConnectedPlayersInARoom(int roomId);
    }
}