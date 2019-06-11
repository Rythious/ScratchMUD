using ScratchMUD.Server.Combat;
using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Infrastructure;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Infrastructure
{
    public class ConnectedPlayerUnitTests
    {
        private readonly ConnectedPlayer connectedPlayer;

        public ConnectedPlayerUnitTests()
        {
            connectedPlayer = new ConnectedPlayer(new PlayerCharacter());
        }

        [Fact(DisplayName = "Name => Returns the player character Name property")]
        public void NameReturnsThePlayerCharacterNameProperty()
        {
            //Arrange
            var playerCharacter = new PlayerCharacter { Name = "PlayerName" };

            var singleUseConnectedPlayer = new ConnectedPlayer(playerCharacter);

            //Act
            var result = singleUseConnectedPlayer.Name;

            //Assert
            Assert.Equal(playerCharacter.Name, result);
        }

        [Fact(DisplayName = "RoomId => Returns the player character RoomId property")]
        public void RoomIdReturnsThePlayerCharacterRoomIdProperty()
        {
            //Arrange
            var playerCharacter = new PlayerCharacter { RoomId = 123 };

            var singleUseConnectedPlayer = new ConnectedPlayer(playerCharacter);

            //Act
            var result = singleUseConnectedPlayer.RoomId;

            //Assert
            Assert.Equal(playerCharacter.RoomId, result);
        }

        [Fact(DisplayName = "PlayerCharacterId => Returns the player character PlayerCharacterId property")]
        public void PlayerCharacterIdReturnsThePlayerCharacterPlayerCharacterIdProperty()
        {
            //Arrange
            var playerCharacter = new PlayerCharacter { PlayerCharacterId = 333 };

            var singleUseConnectedPlayer = new ConnectedPlayer(playerCharacter);

            //Act
            var result = singleUseConnectedPlayer.PlayerCharacterId;

            //Assert
            Assert.Equal(playerCharacter.PlayerCharacterId, result);
        }

        [Fact(DisplayName = "QueueCommand => Adds to the CommandQueueCount")]
        public void QueueCommandAddsToTheCommandQueueCount()
        {
            //Arrange
            var startingCountOfCommands = connectedPlayer.CommandQueueCount;

            //Act
            connectedPlayer.QueueCommand("newCommand");

            //Assert
            Assert.Equal(startingCountOfCommands + 1, connectedPlayer.CommandQueueCount);
        }

        [Fact(DisplayName = "DequeueCommand => Reduces to the CommandQueueCount and returns the correct command")]
        public void DequeueCommandReducesTheCommandQueueCountAndReturnsTheCorrectCommand()
        {
            //Arrange
            const string FIRST_COMMAND = "first";

            connectedPlayer.QueueCommand(FIRST_COMMAND);
            connectedPlayer.QueueCommand("second");

            var startingCountOfCommands = connectedPlayer.CommandQueueCount;

            //Act
            var result = connectedPlayer.DequeueCommand();

            //Assert
            Assert.Equal(startingCountOfCommands - 1, connectedPlayer.CommandQueueCount);
            Assert.Equal(FIRST_COMMAND, result);
        }

        [Fact(DisplayName = "QueueMessage => Adds to the MessageQueueCount")]
        public void QueueMessageAddsToTheMessageQueueCount()
        {
            //Arrange
            var startingCountOfMessages = connectedPlayer.MessageQueueCount;

            //Act
            connectedPlayer.QueueMessage("newMessage");

            //Assert
            Assert.Equal(startingCountOfMessages + 1, connectedPlayer.MessageQueueCount);
        }

        [Fact(DisplayName = "DequeueMessage => Reduces to the MessageQueueCount and returns the correct message")]
        public void DequeueMessageReducesTheMessageQueueCountAndReturnsTheCorrectMessage()
        {
            //Arrange
            const string FIRST_MESSAGE = "first";

            connectedPlayer.QueueMessage(FIRST_MESSAGE);
            connectedPlayer.QueueMessage("second");

            var startingCountOfMessages = connectedPlayer.MessageQueueCount;

            //Act
            var result = connectedPlayer.DequeueMessage();

            //Assert
            Assert.Equal(startingCountOfMessages - 1, connectedPlayer.MessageQueueCount);
            Assert.Equal(FIRST_MESSAGE, result);
        }

        [Fact(DisplayName = "QueueCombatAction => Adds to the CombatActionQueueCount")]
        public void QueueCombatActionAddsToTheCombatActionQueueCount()
        {
            //Arrange
            var startingCountOfCombatActions = connectedPlayer.CombatActionQueueCount;

            //Act
            connectedPlayer.QueueCombatAction(new BasicAttack(new ConnectedPlayer(new PlayerCharacter())));

            //Assert
            Assert.Equal(startingCountOfCombatActions + 1, connectedPlayer.CombatActionQueueCount);
        }

        [Fact(DisplayName = "DequeueCombatAction => Reduces to the CombatActionQueueCount and returns the correct combat action")]
        public void DequeueCombatActionReducesTheCombatActionQueueCountAndReturnsTheCorrectCombatAction()
        {
            //Arrange
            var firstTarget = new ConnectedPlayer(new PlayerCharacter { Name = "First" });
            var secondTarget = new ConnectedPlayer(new PlayerCharacter { Name = "Second" });

            var firstCombatAction = new BasicAttack(firstTarget);
            var secondCombatAction = new BasicAttack(secondTarget);

            connectedPlayer.QueueCombatAction(firstCombatAction);
            connectedPlayer.QueueCombatAction(secondCombatAction);

            var startingCountOfCombatActions = connectedPlayer.CombatActionQueueCount;

            //Act
            var result = connectedPlayer.DequeueCombatAction();

            //Assert
            Assert.Equal(startingCountOfCombatActions - 1, connectedPlayer.CombatActionQueueCount);
            Assert.Equal(firstCombatAction, result);
        }
    }
}