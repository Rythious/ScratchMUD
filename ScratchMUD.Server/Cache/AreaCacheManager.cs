using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Repositories;
using System.Collections.Generic;

namespace ScratchMUD.Server.Cache
{
    public class AreaCacheManager : IAreaCacheManager
    {
        private readonly IAreaCache areaCache;
        private readonly IRoomRepository roomRepository;
        private readonly INpcRepository npcRepository;

        public AreaCacheManager(
            IAreaCache areaCache,
            IRoomRepository roomRepository,
            INpcRepository npcRepository
        )
        {
            this.areaCache = areaCache;
            this.roomRepository = roomRepository;
            this.npcRepository = npcRepository;
        }

        public void LoadArea(int areaId)
        {
            IEnumerable<int> roomIds = roomRepository.GetRoomIdsByAreaId(areaId);

            var npcsInTheArea = new List<Npc>();

            foreach (var id in roomIds)
            {
                npcsInTheArea.AddRange(npcRepository.GetNpcsByRoomId(id));
            }

            areaCache.SpawnedNpcs = npcsInTheArea;
        }
    }
}