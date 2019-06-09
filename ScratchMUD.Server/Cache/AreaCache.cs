using ScratchMUD.Server.Models;
using System.Collections.Generic;

namespace ScratchMUD.Server.Cache
{
    public class AreaCache : IAreaCache
    {
        public int AreaId { get; private set; } = 1;
        public List<Npc> SpawnedNpcs { private get; set; } = new List<Npc>();

        public IEnumerable<Npc> GetNpcsInRoom(int roomId)
        {
            return SpawnedNpcs.FindAll(n => n.RoomId == roomId);
        }
    }
}
