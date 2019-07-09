using ScratchMUD.Server.Infrastructure;
using System.Collections.Generic;

namespace ScratchMUD.Server.Repositories
{
    public interface INpcRepository
    {
        IEnumerable<Npc> GetNpcsByRoomId(int roomId);
    }
}