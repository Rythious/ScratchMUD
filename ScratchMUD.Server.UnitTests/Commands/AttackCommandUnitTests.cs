using Moq;
using ScratchMUD.Server.Combat;
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
    public class AttackCommandUnitTests
    {
        private readonly Mock<IPlayerCombatHostedService> mockPlayerCombatHostedService;
        private readonly RoomContext roomContext;
        private readonly AttackCommand attackCommand;

        public AttackCommandUnitTests()
        {
            mockPlayerCombatHostedService = new Mock<IPlayerCombatHostedService>(MockBehavior.Strict);

            roomContext = new RoomContext
            {
                CurrentCommandingPlayer = new ConnectedPlayer(new PlayerCharacter
                {
                    Name = "Tester Jones"
                }),
                OtherPlayersInTheRoom = new List<ConnectedPlayer>
                {
                    new ConnectedPlayer(new PlayerCharacter { Name = "OtherPlayer" })
                },
                NpcsInTheRoom = new List<Server.Infrastructure.Npc>
                {
                    new Server.Infrastructure.Npc { ShortDescription = "Npc" } 
                }
            };

            attackCommand = new AttackCommand(mockPlayerCombatHostedService.Object);
        }

        [Fact(DisplayName = "Name => Returns Attack")]
        public void NameReturnsAttack()
        {
            //Arrange & Act
            var result = attackCommand.Name;

            //Assert
            Assert.Equal("attack", result, ignoreCase: true);
        }

        [Fact(DisplayName = "SyntaxHelp => Returns a string that includes Attack")]
        public void SyntaxHelpReturnsAStringThatIncludesAttack()
        {
            //Arrange & Act
            var result = attackCommand.SyntaxHelp;

            //Assert
            Assert.IsAssignableFrom<string>(result);
            Assert.Contains("attack", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GeneralHelp => Returns a non-empty string")]
        public void GeneralHelpReturnsANonEmptyString()
        {
            //Arrange & Act
            var result = attackCommand.GeneralHelp;

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
            var exception = await Assert.ThrowsAsync<InvalidCommandSyntaxException>(() => attackCommand.ExecuteAsync(roomContext, tooManyParameters));
        }

        [Fact(DisplayName = "ExecuteAsync => When no parameters are sent in, the player receives an invaild syntax message")]
        public async void ExecuteAsyncWhenNoParametersAreSentInThePlayerReceivesAnInvalidSyntaxMessage()
        {
            //Arrange & Act
            var result = await attackCommand.ExecuteAsync(roomContext);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.Contains("Invalid Syntax", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When targeting self, the player is told they cannot attack themselves")]
        public async void ExecuteAsyncWhenTargetSelfThePlayerIsToldTheyCannotAttackThemselves()
        {
            //Arrange
            var targetParameter = "self";

            //Act
            var result = await attackCommand.ExecuteAsync(roomContext, targetParameter);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.Contains("cannot attack yourself", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When targeting another player in the room, the player is told they cannot attack players")]
        public async void ExecuteAsyncWhenTargetingAnotherPlayerInTheRoomThePlayerIsToldTheyCannotAttackPlayers()
        {
            //Arrange
            var targetParameter = "OtherPlayer";

            //Act
            var result = await attackCommand.ExecuteAsync(roomContext, targetParameter);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.Contains("cannot attack other players", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When targeting something that is not in the room, the player is told their target could not be found")]
        public async void ExecuteAsyncWhenTargetingSomethingThatIsNotInTheRoomThePlayerIsToldTheirTargetCouldNotBeFound()
        {
            //Arrange
            var targetParameter = "NotThere";

            //Act
            var result = await attackCommand.ExecuteAsync(roomContext, targetParameter);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
            Assert.Contains("target could not be found", roomContext.CurrentCommandingPlayer.DequeueMessage(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "ExecuteAsync => When targeting an npc in the room, an altercation is started")]
        public async void ExecuteAsyncWhenTargetingAnNpcInTheRoomAnAltercationIsStarted()
        {
            //Arrange
            mockPlayerCombatHostedService.Setup(hs => hs.StartTrackingAltercation(It.IsAny<Altercation>())).Returns(Task.CompletedTask);

            var targetParameter = "Npc";

            //Act
            var result = await attackCommand.ExecuteAsync(roomContext, targetParameter);

            //Assert
            mockPlayerCombatHostedService.VerifyAll();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }
    }
}
