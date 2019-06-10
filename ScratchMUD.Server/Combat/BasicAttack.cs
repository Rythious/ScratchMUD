namespace ScratchMUD.Server.Combat
{
    internal class BasicAttack : ICombatAction
    {
        public string Description => DESCRIPTION;

        public ICombatant Target { get; }

        public const string DESCRIPTION = "basic attack";

        public BasicAttack(ICombatant target)
        {
            Target = target;
        }

        public void Act()
        {
            Target.EvaluateDamage(5);
        }
    }
}