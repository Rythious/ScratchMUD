using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class MoveCommandUnitTests
    {
        const Directions DIRECTION = Directions.North;

        private readonly Mock<IRoomRepository> mockRoomRepository;
        private readonly Mock<IPlayerRepository> mockPlayerRepository;
        private readonly MoveCommand northMoveCommand;
        private readonly RoomContext roomContext;

        public MoveCommandUnitTests()
        {
            mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            mockPlayerRepository = new Mock<IPlayerRepository>(MockBehavior.Strict);

            northMoveCommand = new MoveCommand(DIRECTION, mockRoomRepository.Object, mockPlayerRepository.Object);

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter())
            };
        }

        [Fact(DisplayName = "Name => Returns the name of the direction")]
        public void NameReturnsTheNameOfTheDirection()
        {
            //Arrange & Act
            var result = northMoveCommand.Name;

            //Assert
            Assert.Equal(DIRECTION.ToString(), result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes the name of the direction")]
        public void SyntaxHelpReturnsAStringThatIncludesTheNameOfTheDirection()
        {
            //Arrange & Act
            var result = northMoveCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains(DIRECTION.ToString(), result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange & Act
            var result = northMoveCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => When provided with any parameters, throws InvalidCommandSyntaxException")]
        public async void ExecuteAsyncWhenProvidedWithAnyParametersThrowsInvalidCommandSyntaxException()
        {
            //Arrange
            var tooManyParameters = new string[1] { "one" };

            //Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidCommandSyntaxException>(() => northMoveCommand.ExecuteAsync(roomContext, tooManyParameters));
        }

        [Fact(DisplayName = "ExecuteAsync => Returns an error message when a room does not exist in that direction")]
        public async void ExecuteAsyncReturnsAnErrorMessageWhenAroomDoesNotExistInThatDirection()
        {
            //Arrange
            var room = new Models.Room
            {
                Exits = new HashSet<(Directions, int)>()
            };

            mockRoomRepository.Setup(rr => rr.GetRoomWithTranslatedValues(It.IsAny<int>())).Returns(room);

            //Act
            var result = await northMoveCommand.ExecuteAsync(roomContext);

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            var message = roomContext.CurrentCommandingPlayer.DequeueMessage();
            Assert.Contains("no room", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(DIRECTION.ToString(), message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => Updates the room of the player, updates the player repository record, and queues a command")]
        public async void ExecuteAsyncUpdatesTheRoomOfThePlayerUpdatesThePlayerRepositoryRecordAndQueuesACommand()
        {
            //Arrange
            var room = new Models.Room
            {
                Id = 123,
                AreaId = 4,
                Exits = new HashSet<(Directions, int)>
                {
                    (Directions.North, 45)
                }
            };

            mockRoomRepository.Setup(rr => rr.GetRoomWithTranslatedValues(It.IsAny<int>())).Returns(room);

            var newRoom = new Models.Room
            {
                Id = 125,
                AreaId = 4,
                Exits = new HashSet<(Directions, int)>
                {
                    (Directions.South, 12)
                }
            };

            mockRoomRepository.Setup(rr => rr.GetRoomIdByAreaAndVirtualNumber(room.AreaId, room.Exits.First().Item2)).Returns(newRoom.Id);

            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                RoomId = 4444,
                PlayerCharacterId = 887
            });

            mockPlayerRepository.Setup(pr => pr.UpdateRoomId(connectedPlayer.PlayerCharacterId, newRoom.Id)).Returns(Task.CompletedTask);

            mockPlayerRepository.Setup(pr => pr.GetPlayerCharacter(connectedPlayer.PlayerCharacterId)).Returns(new PlayerCharacter());
            
            //Act
            var result = await northMoveCommand.ExecuteAsync(new RoomContext { CurrentCommandingPlayer = connectedPlayer });

            //Assert
            mockRoomRepository.VerifyAll();
            mockPlayerRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.Equal(1, connectedPlayer.CommandQueueCount);
        }
    }
}
