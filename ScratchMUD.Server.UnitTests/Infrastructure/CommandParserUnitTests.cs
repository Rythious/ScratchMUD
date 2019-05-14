using ScratchMUD.Server.Infrastructure;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Infrastructure
{
    public class CommandParserUnitTests
    {
        [Fact(DisplayName = "SplitCommandFromParameters => When passed an empty string, command is empty and an empty array is returned")]
        public void SplitCommandFromParametersWhenPassedAnEmptyStringCommandIsEmptyAndAnEmptyArrayIsReturned()
        {
            //Arrange & Act
            var result = CommandParser.SplitCommandFromParameters(string.Empty, out var resultArray);

            //Assert
            Assert.True(string.IsNullOrEmpty(result));
            Assert.Empty(resultArray);
        }

        [Fact(DisplayName = "SplitCommandFromParameters => When passed a single word, command is equal to the the lowercase word and an empty array is returned")]
        public void SplitCommandFromParametersWhenPassedASingleWordCommandIsEqualToTheLowercaseWordAndAnEmptyArrayIsReturned()
        {
            //Arrange
            var testString = "TeStCoMMand";

            //Act
            var result = CommandParser.SplitCommandFromParameters(testString, out var resultArray);

            //Assert
            Assert.Equal(testString.ToLower(), result);
            Assert.Empty(resultArray);
        }

        [Fact(DisplayName = "SplitCommandFromParameters => When passed multiple words, command is equal to the the lowercase first word and an array with the other words is returned")]
        public void SplitCommandFromParametersWhenPassedMultipleWordsCommandIsEqualToTheLowercaseFirstWordAndAnArrayWithTheOtherWordsIsReturned()
        {
            //Arrange
            var testCommand = "TEST";
            var firstCommandParameter = "first";
            var secondCommandParameter = "second";
            var testString = $"{testCommand} {firstCommandParameter} {secondCommandParameter}";

            //Act
            var result = CommandParser.SplitCommandFromParameters(testString, out var resultArray);

            //Assert
            Assert.Equal(testCommand.ToLower(), result);
            Assert.True(resultArray.Length == 2);
            Assert.Equal(firstCommandParameter, resultArray[0]);
            Assert.Equal(secondCommandParameter, resultArray[1]);
        }
    }
}
