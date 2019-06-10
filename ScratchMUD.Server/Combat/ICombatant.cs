namespace ScratchMUD.Server.Combat
{
    public interface ICombatant
    {
        ICombatant Target { get; set; }
        string Name { get; }

        bool IsReadyWithAttack();
        ICombatAction DequeueAction();
        bool IsDone();
        void EvaluateDamage(int v);
    }
}