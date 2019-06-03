using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class SayCommand : Command, ICommand
    {
        internal const string NAME = "say";

        public SayCommand()
        {
            Name = NAME;
            SyntaxHelp = "SAY <VALUE>";
            GeneralHelp = "Your character speaks to the other characters in the room.";
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(ConnectedPlayer connectedPlayer, params string[] parameters)
        {
            var output = (CommunicationChannel.Self, string.Empty);

            if (parameters.Length > 0)
            {
                output = (CommunicationChannel.Room, $"{connectedPlayer.Name} says \"{string.Join(" ", parameters)}\"");
            }
            else
            {
                output = (CommunicationChannel.Self, $"You open your mouth as if to say something, but there are no words.");
            }

            return Task.Run(() => new List<(CommunicationChannel, string)> { output });
        }
    }
}