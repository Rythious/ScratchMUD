using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class SayCommandUnitTests
    {
        private readonly SayCommand sayCommand;
        private readonly RoomContext roomContext;

        public SayCommandUnitTests()
        {
            sayCommand = new SayCommand();

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter())
            };
        }

        [Fact(DisplayName = "Name => Returns Say")]
        public void NameReturnsSay()
        {
            //Arrange & Act
            var result = sayCommand.Name;

            //Assert
            Assert.Equal("say", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes Say")]
        public void SyntaxHelpReturnsAStringThatIncludesSay()
        {
            //Arrange & Act
            var result = sayCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("say", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange & Act
            var result = sayCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => When no parameters are sent in, a message to the player is returned indicating no words")]
        public async Task ExecuteAsyncWhenNoParametersAreSentInAMessageToThePlayerIsReturnedIndicatingNoWords()
        {
            //Arrange & Act
            var result = await sayCommand.ExecuteAsync(roomContext);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("no words", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When two parameters are passed in, the parameters and player name are included in the outgoing message to everyone")]
        public async Task ExecuteAsyncWhenTwoParametersArePassedInTheyAreBothIncludedInTheOutgoingMessageToEveryone()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                Name = "Trouble"
            });

            var listeningPlayer = new ConnectedPlayer(new PlayerCharacter());

            var witnessingPlayers = new List<ConnectedPlayer> { listeningPlayer };

            var specialRoomContext = new RoomContext
            {
                CurrentCommandingPlayer = connectedPlayer,
                OtherPlayersInTheRoom = witnessingPlayers
            };

            var firstParameter = "one";
            var secondParameter = "two";

            //Act
            var result = await sayCommand.ExecuteAsync(specialRoomContext, firstParameter, secondParameter);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.True(listeningPlayer.MessageQueueCount == 1);
            var message = specialRoomContext.CurrentCommandingPlayer.DequeueMessage();
            Assert.Contains("you", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(firstParameter + " " + secondParameter, message, StringComparison.OrdinalIgnoreCase);
            var listeningPlayersMessage = listeningPlayer.DequeueMessage();
            Assert.Contains(specialRoomContext.CurrentCommandingPlayer.Name, listeningPlayersMessage);
        }
    }
}