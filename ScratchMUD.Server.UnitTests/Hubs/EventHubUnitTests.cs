using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Hubs;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Repositories;

namespace ScratchMUD.Server.UnitTests.Hubs
{
    public class EventHubUnitTests
    {
        private readonly PlayerContext playerContext;
        private readonly Mock<IRoomRepository> mockRoomRepositiory;
        private readonly Mock<ICommandRepository> mockCommandRepository;
        private readonly EventHub eventHub;

        public EventHubUnitTests()
        {
            playerContext = new PlayerContext
            {
                Name = "Hub Tester"
            };

            mockRoomRepositiory = new Mock<IRoomRepository>(MockBehavior.Strict);
            mockCommandRepository = new Mock<ICommandRepository>(MockBehavior.Strict);

            eventHub = new EventHub(playerContext, mockRoomRepositiory.Object, mockCommandRepository.Object);
        }
    }
}