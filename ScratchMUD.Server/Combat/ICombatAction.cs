namespace ScratchMUD.Server.Combat
{
    public interface ICombatAction
    {
        string Description { get; }
        ICombatant Target { get; }

        void Act();
    }
}