using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class PokeCommand : Command, ICommand
    {
        internal const string NAME = "poke";

        public PokeCommand()
        {
            Name = NAME;
            SyntaxHelp = "poke <TARGET>";
            GeneralHelp = "Your character pokes another character in the room.";
            MaximumParameterCount = 1;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(RoomContext roomContext, params string[] parameters)
        {
            ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(parameters);

            var playerDoingThePoking = roomContext.CurrentCommandingPlayer;

            if (parameters.Length == 0)
            {
                playerDoingThePoking.QueueMessage(InvalidSyntaxErrorText);
            }

            var targetOfPoke = base.AttemptToGetTargetFromPlayersInTheRoom(parameters[0], roomContext);

            if (targetOfPoke == null)
            {
                var npc = base.AttemptToGetTargetFromNpcsInTheRoom(parameters[0], roomContext);

                if (npc == null)
                {
                    playerDoingThePoking.QueueMessage("Could not poke the target. The target could not be found.");
                }
                else
                {
                    playerDoingThePoking.QueueMessage($"You poke {npc.ShortDescription}.");

                    QueueMessagesForWitnessingPlayers(roomContext.OtherPlayersInTheRoom, $"{playerDoingThePoking.Name} poked {npc.ShortDescription}.");
                }
            }
            else if (targetOfPoke == playerDoingThePoking)
            {
                playerDoingThePoking.QueueMessage("You poke yourself. Ouch!");

                QueueMessagesForWitnessingPlayers(roomContext.OtherPlayersInTheRoom, $"{playerDoingThePoking.Name} poked themselves.");
            }
            else
            {
                playerDoingThePoking.QueueMessage($"You poked {targetOfPoke.Name}");

                targetOfPoke.QueueMessage($"{playerDoingThePoking.Name} poked you.  Ouch!");

                QueueMessagesForWitnessingPlayers(roomContext.OtherPlayersInTheRoom.Except(new List<ConnectedPlayer> { targetOfPoke }), $"{playerDoingThePoking.Name} poked {targetOfPoke.Name}.");
            }

            return Task.Run(() => new List<(CommunicationChannel, string)>());
        }

        private void QueueMessagesForWitnessingPlayers(IEnumerable<ConnectedPlayer> witnessingPlayers, string message)
        {
            foreach (var player in witnessingPlayers)
            {
                player.QueueMessage(message);
            }
        }
    }
}
