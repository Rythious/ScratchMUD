using System.Collections.Generic;

namespace ScratchMUD.Server.Repositories
{
    public interface INpcRepository
    {
        IEnumerable<Models.Npc> GetNpcsByRoomId(int roomId);
    }
}