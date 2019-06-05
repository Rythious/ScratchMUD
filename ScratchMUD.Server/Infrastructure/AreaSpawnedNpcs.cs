using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class AreaSpawnedNpcs
    {
        public IEnumerable<Models.Npc> Npcs { get; set; }
    }
}
