using Microsoft.EntityFrameworkCore;
using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class RoomEditCommandUnitTests
    {
        private readonly Mock<EditingState> mockEditingState;
        private readonly Mock<IRoomRepository> mockRoomRepository;
        private readonly RoomContext roomContext;
        private readonly RoomEditCommand roomEditCommand;

        public RoomEditCommandUnitTests()
        {
            mockEditingState = new Mock<EditingState>(MockBehavior.Strict);
            mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter
                {
                    Name = "Tester Jones"
                })
            };

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
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(false);

            mockEditingState.Setup(es => es.AddPlayerEditor(roomContext.CurrentCommandingPlayer.Name, EditType.Room)).Verifiable();

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext);

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("you are editing", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed no parameters and the player is already editing, a message is returned stating they are already editing")]
        public async Task ExecuteAsyncWhenPassedNoParametersAndThePlayerIsAlreadyEditingAMessageIsReturnedStatingTheyAreAlreadyEditing()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext);

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("already editing", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed one Exit parameter, editing state is called to remove the player and a message is returned")]
        public async Task ExecuteAsyncWhenPassedOneExitParameterEditingStateIsCalledToRemoveThePlayerAndAMessageIsReturned()
        {
            //Arrange
            mockEditingState.Setup(es => es.RemovePlayerEditor(roomContext.CurrentCommandingPlayer.Name)).Verifiable();

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, "exit");

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("no longer editing", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed one parameter that does not match a handled case, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedOneParameterThatDoesNotMatchAHandledCaseAnErrorMessageIsReturned()
        {
            //Arrange & Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, "purple");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("invalid syntax", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is not a valid action, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsNotAValidActionAnErrorMessageIsReturned()
        {
            //Arrange & Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, "one", "two");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("invalid syntax", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a valid action but the player is not editing, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsAValidActionButThePlayerIsNotEditingAnErrorMessageIsReturned()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(false);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, "title", "two");

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room edit mode", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a title action the correct room repository method is called with the correct value")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsATitleActionTheCorrectRoomRepositoryMethodIsCalledWithTheCorrectValue()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "title", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateTitle(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a short description action the correct room repository method is called with the correct value")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsAShortDescriptionActionTheCorrectRoomRepositoryMethodIsCalledWithTheCorrectValue()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "short-description", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateShortDescription(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the first is a full description action the correct room repository method is called with the correct value")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndTheFirstIsAFullDescriptionActionTheCorrectRoomRepositoryMethodIsCalledWithTheCorrectValue()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "full-description", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateFullDescription(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter and the room repository is called for an update but throws an exception, an error is returned with that exception text")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAndRoomRepositoryIsCalledForAnUpdateButThrowsAnExceptionAnErrorMessageIsReturnedWithThatExceptionText()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[3] { "full-description", "new", "string" };

            mockRoomRepository.Setup(r => r.UpdateFullDescription(It.IsAny<int>(), It.Is<string>(s => s == string.Join(" ", testParameters[1], testParameters[2]))))
                .Throws(new DbUpdateException("thrown from database", (Exception)null));

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("exception", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed an invalid create parameter, the invalid syntax message is returned")]
        public async Task ExecuteAsynWhenPassedAnInvalidCreateParameterTheInvalidSyntaxMessageIsReturned()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-notreal" };

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Equal(roomEditCommand.InvalidSyntaxErrorText, roomContext.CurrentCommandingPlayer.DequeueMessage());
        }

        [Fact(DisplayName = "ExecuteAsync => When passed the create-north parameter, a new room is created to the north")]
        public async Task ExecuteAsynWhenPassedTheCreateNorthParameterANewRoomIsCreatedToTheNorth()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-north" };

            mockRoomRepository.Setup(r => r.CreateNewRoom(It.IsAny<int>(), It.Is<Directions>(d => d == Directions.North)))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.True(roomContext.CurrentCommandingPlayer.CommandQueueCount == 1);
            var command = roomContext.CurrentCommandingPlayer.DequeueCommand();
            Assert.Contains(Directions.North.ToString(), command, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed the create-east parameter, a new room is created to the east")]
        public async Task ExecuteAsynWhenPassedTheCreateEastParameterANewRoomIsCreatedToTheEast()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-east" };

            mockRoomRepository.Setup(r => r.CreateNewRoom(It.IsAny<int>(), It.Is<Directions>(d => d == Directions.East)))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.True(roomContext.CurrentCommandingPlayer.CommandQueueCount == 1);
            var command = roomContext.CurrentCommandingPlayer.DequeueCommand();
            Assert.Contains(Directions.East.ToString(), command, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed the create-south parameter, a new room is created to the south")]
        public async Task ExecuteAsynWhenPassedTheCreateSouthParameterANewRoomIsCreatedToTheSouth()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-south" };

            mockRoomRepository.Setup(r => r.CreateNewRoom(It.IsAny<int>(), It.Is<Directions>(d => d == Directions.South)))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.True(roomContext.CurrentCommandingPlayer.CommandQueueCount == 1);
            var command = roomContext.CurrentCommandingPlayer.DequeueCommand();
            Assert.Contains(Directions.South.ToString(), command, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed the create-west parameter, a new room is created to the west")]
        public async Task ExecuteAsynWhenPassedTheCreateWestParameterANewRoomIsCreatedToTheWest()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-west" };

            mockRoomRepository.Setup(r => r.CreateNewRoom(It.IsAny<int>(), It.Is<Directions>(d => d == Directions.West)))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.True(roomContext.CurrentCommandingPlayer.CommandQueueCount == 1);
            var command = roomContext.CurrentCommandingPlayer.DequeueCommand();
            Assert.Contains(Directions.West.ToString(), command, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed the create-up parameter, a new room is created to the up")]
        public async Task ExecuteAsynWhenPassedTheCreateUpParameterANewRoomIsCreatedToTheUp()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-up" };

            mockRoomRepository.Setup(r => r.CreateNewRoom(It.IsAny<int>(), It.Is<Directions>(d => d == Directions.Up)))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.True(roomContext.CurrentCommandingPlayer.CommandQueueCount == 1);
            var command = roomContext.CurrentCommandingPlayer.DequeueCommand();
            Assert.Contains(Directions.Up.ToString(), command, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed the create-down parameter, a new room is created to the down")]
        public async Task ExecuteAsynWhenPassedTheCreateDownParameterANewRoomIsCreatedToTheDown()
        {
            //Arrange
            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(roomContext.CurrentCommandingPlayer.Name, out editType)).Returns(true);

            string[] testParameters = new string[1] { "create-down" };

            mockRoomRepository.Setup(r => r.CreateNewRoom(It.IsAny<int>(), It.Is<Directions>(d => d == Directions.Down)))
                .Returns(Task.CompletedTask);

            //Act
            var result = await roomEditCommand.ExecuteAsync(roomContext, testParameters);

            //Assert
            mockEditingState.VerifyAll();
            mockRoomRepository.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("room updated", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.True(roomContext.CurrentCommandingPlayer.CommandQueueCount == 1);
            var command = roomContext.CurrentCommandingPlayer.DequeueCommand();
            Assert.Contains(Directions.Down.ToString(), command, StringComparison.OrdinalIgnoreCase);
        }
    }
}