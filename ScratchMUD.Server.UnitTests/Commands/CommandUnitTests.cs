using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Exceptions;
using System;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class CommandUnitTests
    {
        const string BASIC_COMMAND_NAME = "basic";
        const string BASIC_COMMAND_SYNTAX_HELP = "BASIC syntax";
        const string BASIC_COMMAND_GENERAL_HELP = "Basic general help";
        const int BASIC_COMMAND_MAXIMUM_PARAMETER_COUNT = 2;

        private readonly BasicCommand basicCommand;

        class BasicCommand : Command
        {
            public BasicCommand()
            {
                Name = BASIC_COMMAND_NAME;
                SyntaxHelp = BASIC_COMMAND_SYNTAX_HELP;
                GeneralHelp = BASIC_COMMAND_GENERAL_HELP;
                MaximumParameterCount = BASIC_COMMAND_MAXIMUM_PARAMETER_COUNT;
            }

            public void CheckForTooManyParameters(params string[] passthroughParameters)
            {
                ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(passthroughParameters);
            }
        }

        public CommandUnitTests()
        {
            basicCommand = new BasicCommand();
        }

        [Fact(DisplayName = "Name => Returns corrent string")]
        public void NameReturnsCorrectString()
        {
            //Arrange & Act
            var result = basicCommand.Name;

            //Assert
            Assert.Equal(BASIC_COMMAND_NAME, result);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns corrent string")]
        public void SyntaxHelpReturnsCorrectString()
        {
            //Arrange & Act
            var result = basicCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Equal(BASIC_COMMAND_SYNTAX_HELP, result);
        }

        [Fact(DisplayName = "GeneralHelp => Returns correct string")]
        public void GeneralHelpReturnsCorrectString()
        {
            //Arrange & Act
            var result = basicCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Equal(BASIC_COMMAND_GENERAL_HELP, result);
        }

        [Fact(DisplayName = "InvalidSyntaxErrorText => Returns a string with the name of the command and the syntax help")]
        public void InvalidSyntaxErrorTextReturnsAStringWithTheNameOfTheCommandAndTheSyntaxHelp()
        {
            //Arrange & Act
            var result = basicCommand.InvalidSyntaxErrorText;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains(BASIC_COMMAND_NAME, result, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(BASIC_COMMAND_SYNTAX_HELP, result);
            Assert.Contains("Invalid Syntax", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ThrowInvalidCommandSyntaxExceptionIfTooManyParameters (through proxy) => Does not throw error if parameter count is equal to maximum count")]
        public void ThrowInvalidCommandSyntaxExceptionIfTooManyParametersThroughProxyDoesNotThrowErrorIfParameterCountIsEqualToMaximumCount()
        {
            //Arrange
            string[] parametersThatMatchMaximumCount = new string[2] { "one", "two" };

            //Act & Assert
            basicCommand.CheckForTooManyParameters(parametersThatMatchMaximumCount);
        }

        [Fact(DisplayName = "ThrowInvalidCommandSyntaxExceptionIfTooManyParameters (through proxy) => Throws error if parameter count is greater than to maximum count")]
        public void ThrowInvalidCommandSyntaxExceptionIfTooManyParametersThroughProxyThrowsErrorIfParameterCountIsGreaterThanToMaximumCount()
        {
            //Arrange
            string[] parametersThatMatchMaximumCount = new string[3] { "one", "two", "three" };

            //Act & Assert
            var exception = Assert.Throws<InvalidCommandSyntaxException>(() => basicCommand.CheckForTooManyParameters(parametersThatMatchMaximumCount));
            Assert.Contains(BASIC_COMMAND_NAME, exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(BASIC_COMMAND_SYNTAX_HELP, exception.Message);
            Assert.Contains("Invalid Syntax", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}