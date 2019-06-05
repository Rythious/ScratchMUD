using ScratchMUD.Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Infrastructure
{
    public class RoomContext
    {
        public ConnectedPlayer CurrentCommandingPlayer { get; set; }
        public IEnumerable<ConnectedPlayer> OtherPlayersInTheRoom { get; set; }
        public IEnumerable<Npc> NpcsInTheRoom { get; set; }

        public IEnumerable<ConnectedPlayer> AllPlayersInTheRoom => OtherPlayersInTheRoom.Union(new List<ConnectedPlayer> { CurrentCommandingPlayer }).Distinct();
    }
}