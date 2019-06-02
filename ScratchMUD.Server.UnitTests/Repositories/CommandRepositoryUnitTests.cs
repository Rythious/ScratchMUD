using Moq;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
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

        public CommandRepositoryUnitTests()
        {
            var mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            var mockPlayerRepository = new Mock<IPlayerRepository>(MockBehavior.Strict);

            commandRespository = new CommandRepository(mockRoomRepository.Object, new EditingState(), mockPlayerRepository.Object);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided a null command, throws an ArgumentNullException")]
        public async Task ExecuteCommandAsyncWhenProvidedANullCommandThrowsAnArgumentNullException()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            //Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => commandRespository.ExecuteCommandAsync(connectedPlayer, null));
            Assert.Contains("cannot be null", exception.Message);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided an empty command, throws an ArgumentNullException")]
        public async Task ExecuteCommandAsyncWhenProvidedAnEmptyCommandThrowsAnArgumentNullException()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            //Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => commandRespository.ExecuteCommandAsync(connectedPlayer, string.Empty));
            Assert.Contains("cannot be null", exception.Message);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided an invalid command, throws an ArgumentException")]
        public async Task ExecuteCommandAsyncWhenProvidedAnInvalidCommandThrowsAnArgumentException()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            const string NOT_A_VALID_COMMAND = "not_a_valid_command";

            //Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => commandRespository.ExecuteCommandAsync(connectedPlayer, NOT_A_VALID_COMMAND));
            Assert.Contains("not a valid command", exception.Message);
            Assert.Contains(NOT_A_VALID_COMMAND, exception.Message);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided a valid command, that command is executed")]
        public async Task ExecuteCommandAsyncWhenProvidedAValidCommandThatCommandIsExecuted()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            const string VALID_COMMAND = "say";

            //Act
            var output = await commandRespository.ExecuteCommandAsync(connectedPlayer, VALID_COMMAND);

            //Assert
            Assert.NotNull(output);
            Assert.Single(output);
        }

        [Fact(DisplayName = "ExecuteCommandAsync => When provided a valid command, and there is a queued command, both commands are executed")]
        public async Task ExecuteCommandAsyncWhenProvidedAValidCommandAndThereIsAQueuedCommandBothCommandsAreExecuted()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            const string VALID_COMMAND = "say";

            connectedPlayer.QueueCommand(VALID_COMMAND);

            //Act
            var output = await commandRespository.ExecuteCommandAsync(connectedPlayer, VALID_COMMAND);

            //Assert
            Assert.NotNull(output);
            Assert.True(new List<(CommunicationChannel, string)>(output).Count == 2);
        }
    }
}