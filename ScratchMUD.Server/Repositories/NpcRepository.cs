using ScratchMUD.Server.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Repositories
{
    public class NpcRepository : INpcRepository
    {
        private readonly ScratchMUDContext context;

        public NpcRepository(ScratchMUDContext context)
        {
            this.context = context;
        }

        public IEnumerable<Models.Npc> GetNpcsByRoomId(int roomId)
        {
            IQueryable<Models.Npc> npcsInTheRoom = from rn in context.RoomNpc
                                where rn.RoomId == roomId
                                join nt in context.NpcTranslation on rn.NpcId equals nt.NpcId
                                select new Models.Npc
                                {
                                    Id = rn.NpcId,
                                    RoomId = rn.RoomId,
                                    FullDescription = nt.FullDescription,
                                    ShortDescription = nt.ShortDescription
                                };

            return npcsInTheRoom;
        }
    }
}