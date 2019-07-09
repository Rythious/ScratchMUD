using ScratchMUD.Server.Combat;
using ScratchMUD.Server.EntityFramework;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class ConnectedPlayer : ICombatant, ITargetable
    {
        internal PlayerCharacter PlayerCharacter { get; set; }
        internal int Health { get; private set; } = 50;
        public ICombatant Target { get; set; }
        public string SignalRConnectionId { get; set; }

        public ConnectedPlayer(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;
        }

        #region Passthrough Get Methods on PlayerCharacter
        public string Name => PlayerCharacter.Name;
        internal int RoomId => PlayerCharacter.RoomId;
        internal int PlayerCharacterId => PlayerCharacter.PlayerCharacterId;
        #endregion

        #region Command Queue
        private readonly Queue<string> commandQueue = new Queue<string>();

        internal int CommandQueueCount => commandQueue.Count;

        internal void QueueCommand(string commandName)
        {
            commandQueue.Enqueue(commandName);
        }

        internal string DequeueCommand()
        {
            return commandQueue.Dequeue();
        }
        #endregion

        #region Message Queue
        private readonly Queue<string> messageQueue = new Queue<string>();

        internal int MessageQueueCount => messageQueue.Count;

        internal void QueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        internal string DequeueMessage()
        {
            return messageQueue.Dequeue();
        }
        #endregion

        #region CombatAction Queue
        private readonly Queue<ICombatAction> combatActionQueue = new Queue<ICombatAction>();

        internal int CombatActionQueueCount => combatActionQueue.Count;

        public void QueueCombatAction(ICombatAction combatAction)
        {
            combatActionQueue.Enqueue(combatAction);
        }

        public ICombatAction DequeueCombatAction()
        {
            if (CombatActionQueueCount > 0)
            {
                return combatActionQueue.Dequeue();
            }

            return new BasicAttack(Target);
        }
        #endregion

        public bool IsReadyWithAttack()
        {
            return true;
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