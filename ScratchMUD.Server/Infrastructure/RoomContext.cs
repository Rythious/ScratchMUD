using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Infrastructure
{
    public class RoomContext
    {
        public ConnectedPlayer CurrentCommandingPlayer { get; set; }
        public IEnumerable<ConnectedPlayer> OtherPlayersInTheRoom { get; set; } = new List<ConnectedPlayer>();
        public IEnumerable<Npc> NpcsInTheRoom { get; set; } = new List<Npc>();

        public IEnumerable<ConnectedPlayer> AllPlayersInTheRoom => OtherPlayersInTheRoom.Union(new List<ConnectedPlayer> { CurrentCommandingPlayer }).Distinct();
    }
}