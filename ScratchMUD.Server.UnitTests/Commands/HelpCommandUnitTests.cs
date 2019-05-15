using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class HelpCommandUnitTests
    {
        [Fact(DisplayName = "Name => Returns Help")]
        public void NameReturnsHelp()
        {
            //Arrange
            var helpCommand = new HelpCommand(new Dictionary<string, ICommand>());

            //Act
            var result = helpCommand.Name;

            //Assert
            Assert.Equal("help", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes Help")]
        public void SyntaxHelpReturnsAStringThatIncludesHelp()
        {
            //Arrange
            var helpCommand = new HelpCommand(new Dictionary<string, ICommand>());

            //Act
            var result = helpCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("help", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange
            var helpCommand = new HelpCommand(new Dictionary<string, ICommand>());

            //Act
            var result = helpCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => When no parameters are sent in, the available commands are listed in alphabetical order")]
        public async Task ExecuteAsyncWhenNoParametersAreSentInTheAvailableCommandsAreListedInAlphabeticalOrder()
        {
            //Arrange
            const string ALPHABETICALLY_FIRST_COMMAND = "apple";
            const string ALPHABETICALLY_SECOND_COMMAND = "banana";
            const string ALPHABETICALLY_THIRD_COMMAND = "cranberry";

            var commandDictionary = new Dictionary<string, ICommand>
            {
                [ALPHABETICALLY_SECOND_COMMAND] = null,
                [ALPHABETICALLY_THIRD_COMMAND] = null,
                [ALPHABETICALLY_FIRST_COMMAND] = null
            };

            var helpCommand = new HelpCommand(commandDictionary);

            //Act
            var result = await helpCommand.ExecuteAsync();

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 4);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[1].Item1);
            Assert.Equal(CommunicationChannel.Self, result[2].Item1);
            Assert.Equal(CommunicationChannel.Self, result[3].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("available commands", result[0].Item2, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(ALPHABETICALLY_FIRST_COMMAND, result[1].Item2);
            Assert.Equal(ALPHABETICALLY_SECOND_COMMAND, result[2].Item2);
            Assert.Equal(ALPHABETICALLY_THIRD_COMMAND, result[3].Item2);
        }

        [Fact(DisplayName = "ExecuteAsync => When a parameter is passed in but does not match any available commands, an error message is returned")]
        public async Task ExecuteAsyncWhenAParameterIsPassedInButDoesNotMatchAnyAvailableCommandsAnErrorMessageIsReturned()
        {
            //Arrange
            const string COMMAND1_NAME = "command1";

            var commandDictionary = new Dictionary<string, ICommand>
            {
                [COMMAND1_NAME] = null
            };

            var helpCommand = new HelpCommand(commandDictionary);

            //Act
            var result = await helpCommand.ExecuteAsync("not" + COMMAND1_NAME);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.Contains("no help found", result[0].Item2, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When a parameter is passed that matches an available command, two messages are returned")]
        public async Task ExecuteAsyncWhenAParameterIsPassedThatMatchesAnAvailableCommandTwoMessagesAreReturned()
        {
            //Arrange
            const string COMMAND1_NAME = "command1";

            var mockHelpCommand = new Mock<ICommand>(MockBehavior.Strict);

            mockHelpCommand.Setup(h => h.SyntaxHelp).Returns("syntax statement");
            mockHelpCommand.Setup(h => h.GeneralHelp).Returns("help statement");

            var commandDictionary = new Dictionary<string, ICommand>
            {
                [COMMAND1_NAME] = mockHelpCommand.Object
            };

            var helpCommand = new HelpCommand(commandDictionary);

            //Act
            var result = await helpCommand.ExecuteAsync(COMMAND1_NAME);

            //Assert
            mockHelpCommand.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 2);
            Assert.IsAssignableFrom<CommunicationChannel>(result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[0].Item1);
            Assert.Equal(CommunicationChannel.Self, result[1].Item1);
            Assert.IsAssignableFrom<string>(result[0].Item2);
            Assert.False(string.IsNullOrEmpty(result[0].Item2));
            Assert.False(string.IsNullOrEmpty(result[1].Item2));
        }

        [Fact(DisplayName = "ExecuteAsync => When two parameters are passed in, an error message is returned")]
        public async Task ExecuteAsyncWhenTwoParametersArePassedInAnErrorMessageIsReturned()
        {
            //Arrange
            var commandDictionary = new Dictionary<string, ICommand>();

            var helpCommand = new HelpCommand(commandDictionary);

            //Act
            var result = await helpCommand.ExecuteAsync("one", "two");

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