using Microsoft.EntityFrameworkCore;
using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class RoomEditCommandUnitTests
    {
        private readonly Mock<EditingState> mockEditingState;
        private readonly Mock<IRoomRepository> mockRoomRepository;
        private readonly ConnectedPlayer connectedPlayer;
        private readonly RoomEditCommand roomEditCommand;

        public RoomEditCommandUnitTests()
        {
            mockEditingState = new Mock<EditingState>(MockBehavior.Strict);
            mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);

            connectedPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                Name = "Tester Jones"
            });

            roomEditCommand = new RoomEditCommand(mockEditingState.Object, mockRoomRepository.Object);
        }

        [Fact(DisplayName = "Name => Returns RoomEdit")]
        public void NameReturnsRoomEdit()
        {
            //Arrange & Act
            var result = roomEditCommand.Name;

            //Assert
            Assert.Equal("roomedit", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes RoomEdit")]
        public void SyntaxHelpReturnsAStringThatIncludesRoomEdit()
        {
            //Arrange & Act
            var result = roomEditCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("roomedit", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange & Act
            var result = roomEditCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => When passed no parameters and the player is not currently editing, they are put in editing state with a message")]
        public async Task ExecuteAsyncWhenPassedNoParametersAndThePlayerIsNotCurrentlyEditingTheyArePutInEditingStateWithAMessage()
        {
            //Arrange
            EditType? editType = null;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(false);

            mockEditingState.Setup(es => es.AddPlayerEditor(connectedPlayer.Name, EditType.Room)).Verifiable();

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer });

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("you are editing", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed no parameters and the player is already editing, a message is returned stating they are already editing")]
        public async Task ExecuteAsyncWhenPassedNoParametersAndThePlayerIsAlreadyEditingAMessageIsReturnedStatingTheyAreAlreadyEditing()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(true);

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer });

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("already editing", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed one Exit parameter, editing state is called to remove the player and a message is returned")]
        public async Task ExecuteAsyncWhenPassedOneExitParameterEditingStateIsCalledToRemoveThePlayerAndAMessageIsReturned()
        {
            //Arrange
            mockEditingState.Setup(es => es.RemovePlayerEditor(connectedPlayer.Name)).Verifiable();

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, "exit");

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("no longer editing", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed one parameter that does not match a handled case, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedOneParameterThatDoesNotMatchAHandledCaseAnErrorMessageIsReturned()
        {
            //Arrange & Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, "purple");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("invalid syntax", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is not a valid action, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsNotAValidActionAnErrorMessageIsReturned()
        {
            //Arrange & Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, "one", "two");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("invalid syntax", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a valid action but the player is not editing, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsAValidActionButThePlayerIsNotEditingAnErrorMessageIsReturned()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(false);

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, "title", "two");

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("room edit mode", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a title action the correct room repository method is called with the correct value")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsATitleActionTheCorrectRoomRepositoryMethodIsCalledWithTheCorrectValue()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "title", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateTitle(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a short description action the correct room repository method is called with the correct value")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsAShortDescriptionActionTheCorrectRoomRepositoryMethodIsCalledWithTheCorrectValue()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "short-description", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateShortDescription(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a full description action the correct room repository method is called with the correct value")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsAFullDescriptionActionTheCorrectRoomRepositoryMethodIsCalledWithTheCorrectValue()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "full-description", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateFullDescription(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the room repository is called for an update but throws an exception, an error is returned with that exception text")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndRoomRepositoryIsCalledForAnUpdateButThrowsAnExceptionAnErrorMessageIsReturnedWithThatExceptionText()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(connectedPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "full-description", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateFullDescription(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Throws(new DbUpdateException("thrown from database", (Exception)null));

            //Act
            var result = await roomEditCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer> { connectedPlayer }, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("exception", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }
    }
}