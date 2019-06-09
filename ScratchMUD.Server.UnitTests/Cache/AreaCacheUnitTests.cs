using ScratchMUD.Server.Cache;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Cache
{
    public class AreaCacheUnitTests
    {
        private readonly AreaCache areaCache;

        public AreaCacheUnitTests()
        {
            areaCache = new AreaCache();
        }

        [Fact(DisplayName = "GetNpcsInRoom => When NPCs exist in multiple room ids, only ones in the specified room id are returned")]
        public void GetNpcsInRoomWhenNpcsExistInMultipleRoomIdsOnlyOnesInTheSpecifiedRoomIdAreReturned()
        {
            //Arrange
            const int ROOM_ID_FOR_TEST = 1;

            areaCache.SpawnedNpcs = new List<Models.Npc>
            {
                new Models.Npc
                {
                    RoomId = ROOM_ID_FOR_TEST,
                    Id = 1
                },
                new Models.Npc
                {
                    RoomId = 2,
                    Id = 2
                }
            };

            //Act
            var npcInRoom = areaCache.GetNpcsInRoom(ROOM_ID_FOR_TEST);

            //Assert
            Assert.Empty(npcInRoom.Where(n => n.RoomId != ROOM_ID_FOR_TEST));
            Assert.Single(npcInRoom);
        }

        [Fact(DisplayName = "GetNpcsInRoom => When there are no spawned NPCs, an empty list is returned")]
        public void GetNpcsInRoomWhenThereAreNoSpawnedNpcsAnEmptyListIsReturned()
        {
            //Arrange
            const int ROOM_ID_FOR_TEST = 1;

            areaCache.SpawnedNpcs = new List<Models.Npc>();

            //Act
            var npcInRoom = areaCache.GetNpcsInRoom(ROOM_ID_FOR_TEST);

            //Assert
            Assert.Empty(npcInRoom);
        }
    }
}