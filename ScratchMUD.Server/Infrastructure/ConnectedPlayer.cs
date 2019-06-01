using ScratchMUD.Server.EntityFramework;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class ConnectedPlayer
    {
        internal PlayerCharacter PlayerCharacter { get; set; }
        private readonly Queue<string> commandQueue = new Queue<string>();

        public ConnectedPlayer(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;
        }

        internal string Name => PlayerCharacter.Name;
        internal int RoomId => PlayerCharacter.RoomId;
        internal int CommandQueueCount => commandQueue.Count;
        internal int PlayerCharacterId => PlayerCharacter.PlayerCharacterId;

        internal void QueueCommand(string commandName)
        {
            commandQueue.Enqueue(commandName);
        }

        internal string DequeueCommand()
        {
            return commandQueue.Dequeue();
        }
    }
}