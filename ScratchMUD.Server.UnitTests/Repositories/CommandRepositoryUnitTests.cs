using Moq;
using ScratchMUD.Server.Combat;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Repositories
{
    public class CommandRepositoryUnitTests
    {
        private readonly CommandRepository commandRespository;
        private readonly RoomContext roomContext;

        public CommandRepositoryUnitTests()
        {
            var mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            var mockPlayerRepository = new Mock<IPlayerRepository>(MockBehavior.Strict);
            var mockPlayerCombatHostedService = new Mock<IPlayerCombatHostedService>(MockBehavior.Strict);

            commandRespository = new CommandRepository(mockRoomRepository.Object, new EditingState(), mockPlayerRepository.Object, mockPlayerCombatHostedService.Object);

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter())
            };
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided a null command, throws an ArgumentNullException")]
        public async Task ExecuteCommandAsyncWhenProvidedANullCommandThrowsAnArgumentNullException()
        {
            //Arrange & Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => commandRespository.ExecuteCommandAsync(roomContext, null));
            Assert.Contains("cannot be null", exception.Message);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided an empty command, throws an ArgumentNullException")]
        public async Task ExecuteCommandAsyncWhenProvidedAnEmptyCommandThrowsAnArgumentNullException()
        {
            //Arrange & Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => commandRespository.ExecuteCommandAsync(roomContext, string.Empty));
            Assert.Contains("cannot be null", exception.Message);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided an invalid command, throws an ArgumentException")]
        public async Task ExecuteCommandAsyncWhenProvidedAnInvalidCommandThrowsAnArgumentException()
        {
            //Arrange
            const string NOT_A_VALID_COMMAND = "not_a_valid_command";

            //Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => commandRespository.ExecuteCommandAsync(roomContext, NOT_A_VALID_COMMAND));
            Assert.Contains("not a valid command", exception.Message);
            Assert.Contains(NOT_A_VALID_COMMAND, exception.Message);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided a valid command, that command is executed")]
        public async Task ExecuteCommandAsyncWhenProvidedAValidCommandThatCommandIsExecuted()
        {
            //Arrange
            const string VALID_COMMAND = "say";

            //Act
            var output = await commandRespository.ExecuteCommandAsync(roomContext, VALID_COMMAND);

            //Assert
            Assert.Empty(output);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided a valid command, and there is a queued command, both commands are executed")]
        public async Task ExecuteCommandAsyncWhenProvidedAValidCommandAndThereIsAQueuedCommandBothCommandsAreExecuted()
        {
            //Arrange
            const string VALID_COMMAND = "say";

            roomContext.CurrentCommandingPlayer.QueueCommand(VALID_COMMAND);

            //Act
            var output = await commandRespository.ExecuteCommandAsync(roomContext, VALID_COMMAND);

            //Assert
            Assert.Empty(output);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 2);
        }
    }
}