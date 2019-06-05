using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class LookCommandUnitTests
    {
        private readonly Mock<IRoomRepository> mockRoomRepository;
        private readonly LookCommand lookCommand;

        public LookCommandUnitTests()
        {
            mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);

            lookCommand = new LookCommand(mockRoomRepository.Object);
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

            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            //Act
            var result = await lookCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer });

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 3);
            Assert.Equal(room.Title, connectedPlayer.DequeueMessage());
            Assert.Equal(room.FullDescription, connectedPlayer.DequeueMessage());
            var exits = connectedPlayer.DequeueMessage();
            Assert.Contains("Exits", exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.East.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.West.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.Down.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.North.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.South.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.Up.ToString(), exits, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When there are non player characters in the room, returns a line for each with its short description")]
        public async void ExecuteAsyncWhenThereAreNonPlayerCharactersInTheRoomReturnsALineForEachWithItsShortDescription()
        {
            //Arrange
            var npc1 = new Models.Npc
            {
                ShortDescription = "Test Npc1"
            };

            var npc2 = new Models.Npc
            {
                ShortDescription = "Test Npc2"
            };

            var room = new Models.Room
            {
                Exits = new HashSet<(Directions, int)>
                {
                    (Directions.East, 3)
                },
                Title = "Room title",
                FullDescription = "Full description of room",
                Npcs = new List<Models.Npc> { npc1, npc2 }
            };

            mockRoomRepository.Setup(rr => rr.GetRoomWithTranslatedValues(It.IsAny<int>())).Returns(room);

            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            //Act
            var result = await lookCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer>());

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 5);
            Assert.Equal(room.Title, connectedPlayer.DequeueMessage());
            Assert.Equal(room.FullDescription, connectedPlayer.DequeueMessage());
            var exits = connectedPlayer.DequeueMessage();
            Assert.Contains("Exits", exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(Directions.East.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.West.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.Down.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.North.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.South.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(Directions.Up.ToString(), exits, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(npc1.ShortDescription, connectedPlayer.DequeueMessage());
            Assert.Contains(npc2.ShortDescription, connectedPlayer.DequeueMessage());
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

            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            //Act
            var result = await lookCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer });

            //Assert
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 3);
            connectedPlayer.DequeueMessage();
            connectedPlayer.DequeueMessage();
            var exits = connectedPlayer.DequeueMessage();
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
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            var tooManyParameters = new string[1] { "one" };

            //Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidCommandSyntaxException>(() => lookCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer>(), tooManyParameters));
        }
    }
}