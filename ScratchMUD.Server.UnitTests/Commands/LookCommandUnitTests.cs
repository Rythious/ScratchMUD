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
using Xunit;
using Npc = ScratchMUD.Server.Infrastructure.Npc;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class LookCommandUnitTests
    {
        private readonly Mock<IRoomRepository> mockRoomRepository;
        private readonly LookCommand lookCommand;
        private readonly RoomContext roomContext;

        public LookCommandUnitTests()
        {
            mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);

            lookCommand = new LookCommand(mockRoomRepository.Object);

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter()),
                NpcsInTheRoom = null
            };
        }

        [Fact(DisplayName = "Name => Returns Look")]
        public void NameReturnsLook()
        {
            //Arrange & Act
            var result = lookCommand.Name;

            //Assert
            Assert.Equal("look", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes Look")]
        public void SyntaxHelpReturnsAStringThatIncludesLook()
        {
            //Arrange & Act
            var result = lookCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("look", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange & Act
            var result = lookCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => Returns the Title, Full Description, and Exits")]
        public async void ExecuteAsyncReturnsTheTitleFullDescriptionAndExits()
        {
            //Arrange
            var room = new Models.Room
            {
                Exits = new HashSet<(Directions, int)>
                {
                    (Directions.East, 3),
                    (Directions.West, 5),
                    (Directions.Down, 7)
                },
                Title = "Room title",
                FullDescription = "Full description of room"
            };

            mockRoomRepository.Setup(rr => rr.GetRoomWithTranslatedValues(It.IsAny<int>())).Returns(room);

            //Act
            var result = await lookCommand.ExecuteAsync(roomContext);

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 3);
            Assert.Equal(room.Title, roomContext.CurrentCommandingPlayer.DequeueMessage());
            Assert.Equal(room.FullDescription, roomContext.CurrentCommandingPlayer.DequeueMessage());
            var exits = roomContext.CurrentCommandingPlayer.DequeueMessage();
            Assert.Contains("Exits", exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.East.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.West.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.Down.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.North.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.South.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.Up.ToString(), exits, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When a room has no exits, the Exits string has none")]
        public async void ExecuteAsyncWhenARoomHasNoExitsTheExitsStringHasNone()
        {
            //Arrange
            var room = new Models.Room
            {
                Exits = new HashSet<(Directions, int)>()
            };

            mockRoomRepository.Setup(rr => rr.GetRoomWithTranslatedValues(It.IsAny<int>())).Returns(room);

            //Act
            var result = await lookCommand.ExecuteAsync(roomContext);

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 3);
            roomContext.CurrentCommandingPlayer.DequeueMessage();
            roomContext.CurrentCommandingPlayer.DequeueMessage();
            var exits = roomContext.CurrentCommandingPlayer.DequeueMessage();
            Assert.Contains("Exits", exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("none", exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.East.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.West.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.Down.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.North.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.South.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.Up.ToString(), exits, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When provided with any parameters, throws InvalidCommandSyntaxException")]
        public async void ExecuteAsyncWhenProvidedWithAnyParametersThrowsInvalidCommandSyntaxException()
        {
            //Arrange
            var tooManyParameters = new string[1] { "one" };

            //Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidCommandSyntaxException>(() => lookCommand.ExecuteAsync(roomContext, tooManyParameters));
        }

        [Fact(DisplayName = "ExecuteAsync => When a room contains Npcs, there is a description string for each")]
        public async void ExecuteAsyncWhenARoomContainsNpcsThereIsADescriptionStringForEach()
        {
            //Arrange
            var room = new Models.Room
            {
                Exits = new HashSet<(Directions, int)>()
            };

            mockRoomRepository.Setup(rr => rr.GetRoomWithTranslatedValues(It.IsAny<int>())).Returns(room);

            var roomContextWithNpcs = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter()),
                NpcsInTheRoom = new List<Npc>
                {
                    new Npc { ShortDescription = "FirstNpc" },
                    new Npc { ShortDescription = "SecondNpc" }
                }
            };

            //Act
            var result = await lookCommand.ExecuteAsync(roomContextWithNpcs);

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContextWithNpcs.CurrentCommandingPlayer.MessageQueueCount == 5);
            roomContextWithNpcs.CurrentCommandingPlayer.DequeueMessage();
            roomContextWithNpcs.CurrentCommandingPlayer.DequeueMessage();
            roomContextWithNpcs.CurrentCommandingPlayer.DequeueMessage();
            var firstNpcMessage = roomContextWithNpcs.CurrentCommandingPlayer.DequeueMessage();
            Assert.Contains(roomContextWithNpcs.NpcsInTheRoom.ElementAt(0).ShortDescription, firstNpcMessage, StringComparison.OrdinalIgnoreCase);
            var secondNpcMessage = roomContextWithNpcs.CurrentCommandingPlayer.DequeueMessage();
            Assert.Contains(roomContextWithNpcs.NpcsInTheRoom.ElementAt(1).ShortDescription, secondNpcMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}