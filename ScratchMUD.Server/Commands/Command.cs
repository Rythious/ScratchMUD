namespace ScratchMUD.Server.Commands
{
    internal abstract class Command
    {
        public string Name { get; protected set; }
        public string SyntaxHelp { get; protected set; }
        public string GeneralHelp { get; protected set; }

        public string InvalidSyntaxErrorText => $"Invalid syntax of {Name.ToUpper()} command: " + SyntaxHelp;
    }
}