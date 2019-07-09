using ScratchMUD.Server.Combat;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class Npc : ICombatant, ITargetable
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public int Health { get; set; }
        public ICombatant Target { get; set; }

        public string Name => ShortDescription;

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

        public void EvaluateDamage(int damage)
        {
            Health -= damage;
        }

        public bool IsDone()
        {
            return Health <= 0;
        }

        public bool IsReadyWithAttack()
        {
            return true;
        }
    }
}