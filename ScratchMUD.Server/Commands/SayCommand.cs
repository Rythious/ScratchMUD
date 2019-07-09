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
            SyntaxHelp = $"{NAME.ToUpper()} <VALUE>";
            GeneralHelp = "Your character speaks to the other characters in the room.";
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(RoomContext roomContext, params string[] parameters)
        {
            if (parameters.Length > 0)
            {
                roomContext.CurrentCommandingPlayer.QueueMessage($"You say \"{string.Join(" ", parameters)}\"");

                foreach (var player in roomContext.OtherPlayersInTheRoom)
                {
                    player.QueueMessage($"{roomContext.CurrentCommandingPlayer.Name} says \"{string.Join(" ", parameters)}\"");
                }
            }
            else
            {
                roomContext.CurrentCommandingPlayer.QueueMessage($"You open your mouth as if to say something, but there are no words.");
            }

            return Task.Run(() => new List<(CommunicationChannel, string)>());
        }
    }
}