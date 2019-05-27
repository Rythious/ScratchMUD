using ScratchMUD.Server.Exceptions;

namespace ScratchMUD.Server.Commands
{
    internal abstract class Command
    {
        public string Name { get; protected set; }
        public string SyntaxHelp { get; protected set; }
        public string GeneralHelp { get; protected set; }
        protected int? MaximumParameterCount { get; set; }

        public string InvalidSyntaxErrorText => $"Invalid syntax of {Name.ToUpper()} command: " + SyntaxHelp;

        protected void ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(params string[] parameters)
        {
            if (parameters.Length > MaximumParameterCount)
            {
                throw new InvalidCommandSyntaxException(InvalidSyntaxErrorText);
            }
        }
    }
}