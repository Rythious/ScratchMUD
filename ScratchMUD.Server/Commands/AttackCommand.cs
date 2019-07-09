using ScratchMUD.Server.Combat;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class AttackCommand : Command, ICommand
    {
        internal const string NAME = "attack";
        private readonly IPlayerCombatHostedService playerCombatHostedService;

        public AttackCommand(IPlayerCombatHostedService playerCombatHostedService)
        {
            Name = NAME;
            SyntaxHelp = $"{NAME.ToUpper()} <TARGET>";
            GeneralHelp = "Attacks an NPC in the current room.";
            MaximumParameterCount = 1;
            this.playerCombatHostedService = playerCombatHostedService;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(RoomContext roomContext, params string[] parameters)
        {
            ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(parameters);

            var playerInitiatingTheAttack = roomContext.CurrentCommandingPlayer;

            if (parameters.Length == 0)
            {
                playerInitiatingTheAttack.QueueMessage(InvalidSyntaxErrorText);
            }
            else
            {
                var targetOfAttack = base.AttemptToGetTargetFromPlayersInTheRoom(parameters[0], roomContext);

                if (targetOfAttack == null)
                {
                    var npc = base.AttemptToGetTargetFromNpcsInTheRoom(parameters[0], roomContext);

                    if (npc == null)
                    {
                        playerInitiatingTheAttack.QueueMessage("Could not attack the target. The target could not be found.");
                    }
                    else
                    {
                        var npcCombatant = (ICombatant)npc;

                        playerInitiatingTheAttack.Target = npcCombatant;
                        npcCombatant.Target = playerInitiatingTheAttack;

                        var altercation = new Altercation(playerInitiatingTheAttack.RoomId)
                        {
                            Combatants = new List<ICombatant> { playerInitiatingTheAttack, npcCombatant }
                        };

                        playerCombatHostedService.StartTrackingAltercation(altercation);
                    }
                }
                else if (targetOfAttack == playerInitiatingTheAttack)
                {
                    playerInitiatingTheAttack.QueueMessage("You cannot attack yourself.");
                }
                else
                {
                    playerInitiatingTheAttack.QueueMessage("You cannot attack other players.");
                }
            }

            return Task.Run(() => new List<(CommunicationChannel, string)>());
        }
    }
}
