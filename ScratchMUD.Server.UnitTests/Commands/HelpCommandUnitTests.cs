using Moq;
using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class HelpCommandUnitTests
    {
        private readonly RoomContext roomContext;

        public HelpCommandUnitTests()
        {
            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter())
            };
        }

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

        [Fact(DisplayName = "ExecuteAsync => When provided with too many parameters, throws InvalidCommandSyntaxException")]
        public async void ExecuteAsyncWhenProvidedWithTooManyParametersThrowsInvalidCommandSyntaxException()
        {
            //Arrange
            var helpCommand = new HelpCommand(new Dictionary<string, ICommand>());

            var tooManyParameters = new string[2] { "one", "two" };

            //Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidCommandSyntaxException>(() => helpCommand.ExecuteAsync(roomContext, tooManyParameters));
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
            var result = await helpCommand.ExecuteAsync(roomContext);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 4);
            Assert.Contains("available commands", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
            Assert.Equal(ALPHABETICALLY_FIRST_COMMAND, roomContext.CurrentCommandingPlayer.DequeueMessage());
            Assert.Equal(ALPHABETICALLY_SECOND_COMMAND, roomContext.CurrentCommandingPlayer.DequeueMessage());
            Assert.Equal(ALPHABETICALLY_THIRD_COMMAND, roomContext.CurrentCommandingPlayer.DequeueMessage());
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
            var result = await helpCommand.ExecuteAsync(roomContext, "not" + COMMAND1_NAME);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 1);
            Assert.Contains("no help found", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
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
            var result = await helpCommand.ExecuteAsync(roomContext, COMMAND1_NAME);

            //Assert
            mockHelpCommand.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(roomContext.CurrentCommandingPlayer.MessageQueueCount == 2);
            Assert.False(string.IsNullOrEmpty(roomContext.CurrentCommandingPlayer.DequeueMessage()));
            Assert.False(string.IsNullOrEmpty(roomContext.CurrentCommandingPlayer.DequeueMessage()));
        }
    }
}