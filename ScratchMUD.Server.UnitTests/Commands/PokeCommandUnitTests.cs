using ScratchMUD.Server.Commands;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Commands
{
    public class PokeCommandUnitTests
    {
        private readonly PokeCommand pokeCommand;
        private readonly ConnectedPlayer commandingPlayer;
        private readonly ConnectedPlayer possibleTargetPlayer;
        private readonly ConnectedPlayer witnessingPlayer;
        private readonly Models.Npc scaryNpc;
        private readonly RoomContext roomContext;

        public PokeCommandUnitTests()
        {
            pokeCommand = new PokeCommand();

            commandingPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                Name = "commandingPlayer"
            });

            possibleTargetPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                Name = "possibleTargetPlayer"
            });

            witnessingPlayer = new ConnectedPlayer(new PlayerCharacter
            {
                Name = "witnessingPlayer"
            });

            scaryNpc = new Models.Npc { ShortDescription = "Scary Npc" };

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = commandingPlayer,
                OtherPlayersInTheRoom = new List<ConnectedPlayer> { possibleTargetPlayer, witnessingPlayer },
                NpcsInTheRoom = new List<Models.Npc> { scaryNpc }
            };
        }

        [Fact(DisplayName = "Name => Returns Poke")]
        public void NameReturnsPoke()
        {
            //Arrange & Act
            var result = pokeCommand.Name;

            //Assert
            Assert.Equal("poke", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes Poke")]
        public void SyntaxHelpReturnsAStringThatIncludesPoke()
        {
            //Arrange & Act
            var result = pokeCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("poke", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange & Act
            var result = pokeCommand.GeneralHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact(DisplayName = "ExecuteAsync => When provided with too many parameters, throws InvalidCommandSyntaxException")]
        public async void ExecuteAsyncWhenProvidedWithTooManyParametersThrowsInvalidCommandSyntaxException()
        {
            //Arrange
            var tooManyParameters = new string[2] { "one", "two" };

            //Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidCommandSyntaxException>(() => pokeCommand.ExecuteAsync(new RoomContext(), tooManyParameters));
        }

        [Fact(DisplayName = "ExecuteAsync => When no parameters are sent in, a message to the player is returned indicating no words")]
        public async void ExecuteAsyncWhenNoParametersAreSentInAMessageToThePlayerIsReturnedIndicatingNoWords()
        {
            //Arrange & Act
            var result = await pokeCommand.ExecuteAsync(roomContext);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(commandingPlayer.MessageQueueCount == 1);
            Assert.Contains("Invalid Syntax", commandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When poking target is self, the commanding player has the approprite queued message")]
        public async void ExecuteAsyncWhenPokingTargetIsSelfTheCommandingPlayerHasTheAppropriateQueuedMessage()
        {
            //Arrange & Act
            var result = await pokeCommand.ExecuteAsync(roomContext, "self");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(commandingPlayer.MessageQueueCount == 1);
            var message = commandingPlayer.DequeueMessage();
            Assert.Contains("you", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("yourself", message, StringComparison.OrdinalIgnoreCase);
            Assert.True(witnessingPlayer.MessageQueueCount == 1);
            var witnessMessage = witnessingPlayer.DequeueMessage();
            Assert.Contains(commandingPlayer.Name, witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("themselves", witnessMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When poking target is the name of the commanding player, it is treated like them poking themselves")]
        public async void ExecuteAsyncWhenPokingTargetIsTheNameOfTheCommandingPlayerItIsTreatedLikeThemPokingThemselves()
        {
            //Arrange & Act
            var result = await pokeCommand.ExecuteAsync(roomContext, commandingPlayer.Name);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(commandingPlayer.MessageQueueCount == 1);
            var message = commandingPlayer.DequeueMessage();
            Assert.Contains("you", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("yourself", message, StringComparison.OrdinalIgnoreCase);
            Assert.True(witnessingPlayer.MessageQueueCount == 1);
            var witnessMessage = witnessingPlayer.DequeueMessage();
            Assert.Contains(commandingPlayer.Name, witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("themselves", witnessMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When poking target is not valid, only the commanding player receives a message")]
        public async void ExecuteAsyncWhenPokingTargetIsNotValidOnlyTheCommandingPlayerReceivesAMessage()
        {
            //Arrange & Act
            var result = await pokeCommand.ExecuteAsync(roomContext, "zzxcvadf");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(commandingPlayer.MessageQueueCount == 1);
            var message = commandingPlayer.DequeueMessage();
            Assert.Contains("poke", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("not be found", message, StringComparison.OrdinalIgnoreCase);
            Assert.True(witnessingPlayer.MessageQueueCount == 0);
            Assert.True(possibleTargetPlayer.MessageQueueCount == 0);
        }

        [Fact(DisplayName = "ExecuteAsync => When poking target is another player, each player receives an appropriate message")]
        public async void ExecuteAsyncWhenPokingTargetIsAnotherPlayerEachPlayerReceivesAnAppropriateMessage()
        {
            //Arrange & Act
            var result = await pokeCommand.ExecuteAsync(roomContext, possibleTargetPlayer.Name);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(commandingPlayer.MessageQueueCount == 1);
            var message = commandingPlayer.DequeueMessage();
            Assert.Contains("you", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(possibleTargetPlayer.Name, message, StringComparison.OrdinalIgnoreCase);
            Assert.True(possibleTargetPlayer.MessageQueueCount == 1);
            var targetMessage = possibleTargetPlayer.DequeueMessage();
            Assert.Contains(commandingPlayer.Name, targetMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", targetMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("you", targetMessage, StringComparison.OrdinalIgnoreCase);
            Assert.True(witnessingPlayer.MessageQueueCount == 1);
            var witnessMessage = witnessingPlayer.DequeueMessage();
            Assert.Contains(commandingPlayer.Name, witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(possibleTargetPlayer.Name, witnessMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When poking target is an NPC, the player and witnesses receive appropriate messages")]
        public async void ExecuteAsyncWhenPokingTargetIsAnNpcThePlayerAndWitnessesReceiveAppropriateMessages()
        {
            //Arrange & Act
            var result = await pokeCommand.ExecuteAsync(roomContext, scaryNpc.ShortDescription);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.True(commandingPlayer.MessageQueueCount == 1);
            var message = commandingPlayer.DequeueMessage();
            Assert.Contains("you", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(scaryNpc.ShortDescription, message, StringComparison.OrdinalIgnoreCase);
            Assert.True(witnessingPlayer.MessageQueueCount == 1);
            var witnessMessage = witnessingPlayer.DequeueMessage();
            Assert.Contains(commandingPlayer.Name, witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("poked", witnessMessage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(scaryNpc.ShortDescription, witnessMessage, StringComparison.OrdinalIgnoreCase);
        }
    }
}
