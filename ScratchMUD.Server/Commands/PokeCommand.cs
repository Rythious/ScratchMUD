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
            SyntaxHelp = $"{NAME.ToUpper()} <TARGET>";
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
            else
            {
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
                        var npcTarget = (Npc)npc;

                        playerDoingThePoking.QueueMessage($"You poked {npcTarget.ShortDescription}.");

                        QueueMessagesForWitnessingPlayers(roomContext.OtherPlayersInTheRoom, $"{playerDoingThePoking.Name} poked {npcTarget.ShortDescription}.");
                    }
                }
                else if (targetOfPoke == playerDoingThePoking)
                {
                    playerDoingThePoking.QueueMessage("You poked yourself. Ouch!");

                    QueueMessagesForWitnessingPlayers(roomContext.OtherPlayersInTheRoom, $"{playerDoingThePoking.Name} poked themselves.");
                }
                else
                {
                    var playerTarget = (ConnectedPlayer)targetOfPoke;

                    playerDoingThePoking.QueueMessage($"You poked {playerTarget.Name}");

                    playerTarget.QueueMessage($"{playerDoingThePoking.Name} poked you.  Ouch!");

                    QueueMessagesForWitnessingPlayers(roomContext.OtherPlayersInTheRoom.Except(new List<ConnectedPlayer> { playerTarget }), $"{playerDoingThePoking.Name} poked {playerTarget.Name}.");
                }
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
