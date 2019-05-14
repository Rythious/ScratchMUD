using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Infrastructure
{
    public class EditingStateUnitTests
    {
        [Fact(DisplayName = "IsPlayerCurrentlyEditing => When the provided player is not in the dictionary returns false")]
        public void IsPlayerCurrentlyEditingWhenTheProvidedPlayerIsNotInTheDictionaryReturnsFalse()
        {
            //Arrange
            var editingState = new EditingState();

            //Act
            var result = editingState.IsPlayerCurrentlyEditing("testPlayer", out var outputEditType);

            //Assert
            Assert.False(result);
            Assert.Null(outputEditType);
        }

        [Fact(DisplayName = "IsPlayerCurrentlyEditing => When the provided player is in the dictionary returns true with EditType")]
        public void IsPlayerCurrentlyEditingWhenTheProvidedPlayerIsInTheDictionaryReturnsTrueWithEditType()
        {
            //Arrange
            var testPlayer = "testPlayer";
            var editType = EditType.Room;

            var editingState = new EditingState();

            editingState.AddPlayerEditor(testPlayer, editType);

            //Act
            var result = editingState.IsPlayerCurrentlyEditing(testPlayer, out var outputEditType);

            //Assert
            Assert.True(result);
            Assert.Equal(editType, outputEditType);
        }

        [Fact(DisplayName = "AddPlayerEditor => When the provided player is already in the dictionary, throw PlayerAlreadyEditingException with the player name and EditType")]
        public void AddPlayerEditorWhenTheProvidedPlayerIsAlreadyInTheDictionaryThrowPlayerAlreadyEditingExceptionWithThePlayerNameAndEditType()
        {
            //Arrange
            var testPlayer = "testPlayer";
            var editType = EditType.Room;

            var editingState = new EditingState();

            editingState.AddPlayerEditor(testPlayer, editType);

            //Act & Assert
            var exception = Assert.Throws<PlayerAlreadyEditingException>(() => editingState.AddPlayerEditor(testPlayer, editType));
            Assert.Contains(testPlayer, exception.Message);
            Assert.Contains(editType.ToString(), exception.Message);
        }

        [Fact(DisplayName = "RemovePlayerEditor => When the provided player was in editing, they are no longer editing afterward")]
        public void RemovePlayerEditorWhenTheProvidedPlayerWasInEditingTheyAreNoLongerEditingAfterward()
        {
            //Arrange
            var testPlayer = "testPlayer";
            var editType = EditType.Room;

            var editingState = new EditingState();

            editingState.AddPlayerEditor(testPlayer, editType);
            Assert.True(editingState.IsPlayerCurrentlyEditing(testPlayer, out var _));

            //Act
            editingState.RemovePlayerEditor(testPlayer);

            //Assert
            Assert.False(editingState.IsPlayerCurrentlyEditing(testPlayer, out var _));
        }
    }
}