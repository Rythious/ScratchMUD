using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System;
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
            //Arrange & Act
            var result = await sayCommand.ExecuteAsync(new ConnectedPlayer(new PlayerCharacter()));

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("no words", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When two parameters are passed in, the parametrs and player name are included in the outgoing message to everyone")]
        public async Task ExecuteAsyncWhenTwoParametersArePassedInTheyAreBothIncludedInTheOutgoingMessageToEveryone()
        {
            //Arrange
            var connectedPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                Name = "Trouble"
            });
            
            var firstParameter = "one";
            var secondParameter = "two";

            //Act
            var result = await sayCommand.ExecuteAsync(connectedPlayer, firstParameter, secondParameter);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Everyone, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains(connectedPlayer.Name, result[0].Item2);
            Assert.Contains(firstParameter + " " + secondParameter, result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }
    }
}