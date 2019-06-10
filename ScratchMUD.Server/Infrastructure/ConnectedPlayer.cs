using ScratchMUD.Server.Combat;
using ScratchMUD.Server.EntityFramework;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class ConnectedPlayer : ICombatant, ITargetable
    {
        internal PlayerCharacter PlayerCharacter { get; set; }
        private readonly Queue<string> commandQueue = new Queue<string>();
        private readonly Queue<string> messageQueue = new Queue<string>();

        public ConnectedPlayer(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;
        }

        public string Name => PlayerCharacter.Name;
        internal int RoomId => PlayerCharacter.RoomId;
        internal int CommandQueueCount => commandQueue.Count;
        internal int MessageQueueCount => messageQueue.Count;
        internal int PlayerCharacterId => PlayerCharacter.PlayerCharacterId;
        internal int Health { get; private set; } = 50;

        public ICombatant Target { get; set; }

        internal void QueueCommand(string commandName)
        {
            commandQueue.Enqueue(commandName);
        }

        internal string DequeueCommand()
        {
            return commandQueue.Dequeue();
        }

        internal void QueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        internal string DequeueMessage()
        {
            return messageQueue.Dequeue();
        }

        public bool IsReadyWithAttack()
        {
            return true;
        }

        public ICombatAction DequeueAction()
        {
            return new BasicAttack(Target);
        }

        public bool IsDone()
        {
            return Health <= 0;
        }

        public void EvaluateDamage(int damage)
        {
            Health -= damage;
        }
    }
}