using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public interface IPlayerConnections
    {
        ConnectedPlayer GetConnectedPlayerByConnectionId(string signalRConnectionId);
        int GetAvailablePlayerCharacterId();
        void AddConnectedPlayer(string signalRConnectionId, ConnectedPlayer connectedPlayer);
        List<string> GetConnectionsInTheSameRoomAsAConnection(string connectionId);
        List<ConnectedPlayer> GetConnectedPlayersInTheSameRoomAsAConnection(string connectionId);
    }
}