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

        public SayCommandUnitTests()
        {
            sayCommand = new SayCommand();
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
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter());

            //Act
            var result = await sayCommand.ExecuteAsync(connectedPlayer, new List<ConnectedPlayer>());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.Contains("no words", connectedPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
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

            var playersInTheRoom = new List<ConnectedPlayer> { connectedPlayer, listeningPlayer };

            var firstParameter = "one";
            var secondParameter = "two";

            //Act
            var result = await sayCommand.ExecuteAsync(connectedPlayer, playersInTheRoom, firstParameter, secondParameter);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(connectedPlayer.MessageQueueCount == 1);
            Assert.True(listeningPlayer.MessageQueueCount == 1);
            var message = connectedPlayer.DequeueMessage();
            Assert.Contains(connectedPlayer.Name, message);
            Assert.Contains(firstParameter + " " + secondParameter, message, StringComparison.OrdinalIgnoreCase);
            var listeningPlayersMessage = listeningPlayer.DequeueMessage();
            Assert.Equal(listeningPlayersMessage, message);
        }
    }
}