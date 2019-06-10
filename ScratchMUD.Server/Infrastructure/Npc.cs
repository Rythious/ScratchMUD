using ScratchMUD.Server.Combat;

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

        public ICombatAction DequeueAction()
        {
            return new BasicAttack(Target);
        }

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