using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Infrastructure;
using System.Linq;

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

        protected ConnectedPlayer AttemptToGetTargetFromPlayersInTheRoom(string targetSelector, RoomContext roomContext)
        {
            if (targetSelector.ToLower() == "self")
            {
                return roomContext.CurrentCommandingPlayer;
            }

            var firstTarget = roomContext.AllPlayersInTheRoom.FirstOrDefault(pc => pc.Name.StartsWith(targetSelector));

            return firstTarget;
        }

        protected Models.Npc AttemptToGetTargetFromNpcsInTheRoom(string targetSelector, RoomContext roomContext)
        {
            var firstTarget = roomContext.NpcsInTheRoom.FirstOrDefault(n => n.ShortDescription.Contains(targetSelector));

            return firstTarget;
        }
    }
}