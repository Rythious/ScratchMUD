using ScratchMUD.Server.Infrastructure;
using System;
using Xunit;

namespace ScratchMUD.Server.UnitTests.Infrastructure
{
    public class PlayerConnectionsUnitTests
    {
        private readonly PlayerConnections playerConnections;

        public PlayerConnectionsUnitTests()
        {
            playerConnections = new PlayerConnections();
        }

        [Fact(DisplayName = "GetAvailablePlayerCharacterId => When constructed, there are five preset player ids")]
        public void GetAvailablePlayerCharacterIdWhenConstructedThereAreFivePresetPlayerIds()
        {
            //Arrange
            for (int i = 0; i < 5; i++)
            {
                playerConnections.GetAvailablePlayerCharacterId();
            }

            //Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => playerConnections.GetAvailablePlayerCharacterId());
            Assert.Contains("queue empty", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact(DisplayName = "GetAvailablePlayerCharacterId => Does not produce the same id twice in a row")]
        public void GetAvailablePlayerCharacterIdDoesNotProductTheSameIdTwiceInARow()
        {
            //Arrange & Act
            var firstId = playerConnections.GetAvailablePlayerCharacterId();
            var secondId = playerConnections.GetAvailablePlayerCharacterId();

            //Assert
            Assert.NotEqual(firstId, secondId);
        }
    }
}
