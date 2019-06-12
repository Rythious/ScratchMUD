using Moq;
using ScratchMUD.Server.Cache;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Cache
{
    public class AreaCacheManagerUnitTests
    {
        private readonly Mock<IAreaCache> mockAreaCache;
        private readonly Mock<IRoomRepository> mockRoomRepository;
        private readonly Mock<INpcRepository> mockNpcRepository;
        private readonly AreaCacheManager areaCacheManager;

        public AreaCacheManagerUnitTests()
        {
            mockAreaCache = new Mock<IAreaCache>(MockBehavior.Strict);
            mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            mockNpcRepository = new Mock<INpcRepository>(MockBehavior.Strict);

            areaCacheManager = new AreaCacheManager(mockAreaCache.Object, mockRoomRepository.Object, mockNpcRepository.Object);
        }

        [Fact(DisplayName = "LoadArea => When there are no rooms for an areaId it does not try to retrieve NPCs")]
        public void LoadAreaWhenThereAreNoRoomsForAnAreaIdItDoesNotTryToRetrieveNpcs()
        {
            //Arrange
            mockRoomRepository.Setup(rr => rr.GetRoomIdsByAreaId(It.IsAny<int>())).Returns(new List<int>());

            mockAreaCache.SetupSet(ac => ac.SpawnedNpcs = new List<Npc>()).Verifiable();

            //Act
            areaCacheManager.LoadArea(1);

            //Assert
            mockRoomRepository.VerifyAll();
            mockNpcRepository.Verify(nr => nr.GetNpcsByRoomId(It.IsAny<int>()), Times.Never);
            mockAreaCache.VerifyAll();
        }

        [Fact(DisplayName = "LoadArea => When there are rooms for an areaId the NPCs are retrieved for each room")]
        public void LoadAreaWhenThereAreRoomsForAnAreaIdTheNpcsAreRetievedForEachRoom()
        {
            //Arrange
            var roomIdsList = new List<int> { 1, 2 };

            mockRoomRepository.Setup(rr => rr.GetRoomIdsByAreaId(It.IsAny<int>())).Returns(roomIdsList);

            var npcInRoom1 = new Npc { Id = 10 };
            var npcInRoom2 = new Npc { Id = 20 };

            mockNpcRepository.Setup(nr => nr.GetNpcsByRoomId(roomIdsList.First())).Returns(new List<Npc> { npcInRoom1 });
            mockNpcRepository.Setup(nr => nr.GetNpcsByRoomId(roomIdsList.Last())).Returns(new List<Npc> { npcInRoom2 });

            mockAreaCache.SetupSet(ac => ac.SpawnedNpcs = new List<Npc> { npcInRoom1, npcInRoom2 }).Verifiable();

            //Act
            areaCacheManager.LoadArea(1);

            //Assert
            mockRoomRepository.VerifyAll();
            mockNpcRepository.VerifyAll();
            mockAreaCache.VerifyAll();
        }
    }
}
