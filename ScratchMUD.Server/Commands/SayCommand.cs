using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class SayCommand : ICommand
    {
        internal const string NAME = "say";

        #region Syntax, Help, and Name
        public string Name { get; } = NAME;

        public string SyntaxHelp => "SAY <VALUE>";

        public string GeneralHelp => "Your character speaks to the other characters in the room.";
        #endregion

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(PlayerContext playerContext, params string[] parameters)
        {
            var output = (CommunicationChannel.Self, string.Empty);

            if (parameters.Length > 0)
            {
                output = (CommunicationChannel.Everyone, $"{playerContext.Name} says \"{string.Join(" ", parameters)}\"");
            }
            else
            {
                output = (CommunicationChannel.Self, $"You open your mouth as if to say something, but there are no words.");
            }

            return Task.Run(() => new List<(CommunicationChannel, string)> { output });
        }
    }
}