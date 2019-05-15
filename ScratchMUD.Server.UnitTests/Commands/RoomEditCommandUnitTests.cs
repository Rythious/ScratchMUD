using Microsoft.Extensions.Configuration;
using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class RoomEditCommandUnitTests
    {
        private readonly IConfiguration configuration;

        public RoomEditCommandUnitTests()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configuration = configurationBuilder.Build();
        }

        [Fact(DisplayName = "Name => Returns RoomEdit")]
        public void NameReturnsRoomEdit()
        {
            //Arrange


            var roomEditCommand = new RoomEditCommand(new EditingState(), new PlayerContext(), configuration);

            //Act
            var result = roomEditCommand.Name;

            //Assert
            Assert.Equal("roomedit", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes RoomEdit")]
        public void SyntaxHelpReturnsAStringThatIncludesRoomEdit()
        {
            //Arrange
            var roomEditCommand = new RoomEditCommand(new EditingState(), new PlayerContext(), configuration);

            //Act
            var result = roomEditCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("roomedit", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange
            var roomEditCommand = new RoomEditCommand(new EditingState(), new PlayerContext(), configuration);

            //Act
            var result = roomEditCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => When passed no parameters and the player is not currently editing, they are put in editing state with a message")]
        public async Task ExecuteAsyncWhenPassedNoParametersAndThePlayerIsNotCurrentlyEditingTheyArePutInEditingStateWithAMessage()
        {
            //Arrange
            var playerContext = new PlayerContext
            {
                Name = "Rodrigo"
            };

            var mockEditingState = new Mock<EditingState>(MockBehavior.Strict);

            EditType? editType = null;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(playerContext.Name, out editType)).Returns(false);

            mockEditingState.Setup(es => es.AddPlayerEditor(playerContext.Name, EditType.Room)).Verifiable();

            var roomEditCommand = new RoomEditCommand(mockEditingState.Object, playerContext, configuration);

            //Act
            var result = await roomEditCommand.ExecuteAsync();

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("you are editing", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed no parameters and the player is already editing, a message is returned stating they are already editing")]
        public async Task ExecuteAsyncWhenPassedNoParametersAndThePlayerIsAlreadyEditingAMessageIsReturnedStatingTheyAreAlreadyEditing()
        {
            //Arrange
            var playerContext = new PlayerContext
            {
                Name = "Estavo"
            };

            var mockEditingState = new Mock<EditingState>(MockBehavior.Strict);

            EditType? editType = EditType.Room;
            mockEditingState.Setup(es => es.IsPlayerCurrentlyEditing(playerContext.Name, out editType)).Returns(true);

            var roomEditCommand = new RoomEditCommand(mockEditingState.Object, playerContext, configuration);

            //Act
            var result = await roomEditCommand.ExecuteAsync();

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("already editing", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed one Exit parameter, editing state is called to remove the player and a message is returned")]
        public async Task ExecuteAsyncWhenPassedOneExitParameterEditingStateIsCalledToRemoveThePlayerAndAMessageIsReturned()
        {
            //Arrange
            var playerContext = new PlayerContext
            {
                Name = "Chimmy"
            };

            var mockEditingState = new Mock<EditingState>(MockBehavior.Strict);

            mockEditingState.Setup(es => es.RemovePlayerEditor(playerContext.Name)).Verifiable();

            var roomEditCommand = new RoomEditCommand(mockEditingState.Object, playerContext, configuration);

            //Act
            var result = await roomEditCommand.ExecuteAsync("exit");

            //Assert
            mockEditingState.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("no longer editing", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed one parameter that does not match a handled case, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedOneParameterThatDoesNotMatchAHandledCaseAnErrorMessageIsReturned()
        {
            //Arrange
            var roomEditCommand = new RoomEditCommand(new EditingState(), new PlayerContext(), configuration);

            //Act
            var result = await roomEditCommand.ExecuteAsync("purple");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("invalid syntax", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When passed more than one parameter, an error message is returned")]
        public async Task ExecuteAsyncWhenPassedMoreThanOneParameterAnErrorMessageIsReturned()
        {
            //Arrange
            var roomEditCommand = new RoomEditCommand(new EditingState(), new PlayerContext(), configuration);

            //Act
            var result = await roomEditCommand.ExecuteAsync("one", "two");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("invalid syntax", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }
    }
}