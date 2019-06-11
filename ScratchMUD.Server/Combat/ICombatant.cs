namespace ScratchMUD.Server.Combat
{
    public interface ICombatant
    {
        ICombatant Target { get; set; }
        string Name { get; }

        bool IsReadyWithAttack();
        void QueueCombatAction(ICombatAction combatAction);
        ICombatAction DequeueCombatAction();
        bool IsDone();
        void EvaluateDamage(int v);
    }
}