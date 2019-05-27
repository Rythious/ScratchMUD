using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Repositories;

namespace ScratchMUD.Server.Infrastructure
{
    public interface IPlayerConnections
    {
        ConnectedPlayer GetConnectedPlayerByConnectionId(string signalRConnectionId);
        int GetAvailablePlayerCharacterId();
        void AddConnectedPlayer(string signalRConnectionId, ConnectedPlayer connectedPlayer);
    }
}