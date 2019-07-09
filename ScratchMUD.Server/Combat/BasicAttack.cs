namespace ScratchMUD.Server.Combat
{
    internal class BasicAttack : ICombatAction
    {
        private const string DESCRIPTION = "basic attack";

        public string Description => DESCRIPTION;

        public ICombatant Target { get; }

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