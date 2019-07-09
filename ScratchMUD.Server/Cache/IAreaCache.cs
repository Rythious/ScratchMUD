using System.Collections.Generic;
using ScratchMUD.Server.Models;

namespace ScratchMUD.Server.Cache
{
    public interface IAreaCache
    {
        List<Npc> SpawnedNpcs { set; }
        IEnumerable<Npc> GetNpcsInRoom(int roomId);
    }
}