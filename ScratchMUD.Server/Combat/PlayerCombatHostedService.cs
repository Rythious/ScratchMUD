using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using ScratchMUD.Server.Hubs;
using ScratchMUD.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Combat
{
    public class PlayerCombatHostedService : IPlayerCombatHostedService, IHostedService, IDisposable
    {
        private readonly IHubContext<EventHub> hubContext;
        private readonly IPlayerConnections playerConnections;
        private Timer _timer;
        private readonly List<Altercation> Altercations = new List<Altercation>();
        private bool isRunning = false;

        public PlayerCombatHostedService(
            IHubContext<EventHub> hubContext, 
            IPlayerConnections playerConnections
        )
        {
            this.hubContext = hubContext;
            this.playerConnections = playerConnections;
        }

        public async Task StartTrackingAltercation(Altercation altercation)
        {
            Altercations.Add(altercation);

            if (Altercations.Count > 0 && !isRunning)
            {
                await StartAsync(new CancellationToken());
            }
        }

        private async void UpdateAltercations(object state)
        {
            foreach (var altercation in Altercations)
            {
                var playersInTheRoom = playerConnections.GetConnectedPlayersInARoom(altercation.RoomId);

                var witnessPlayersOfAltercation = playersInTheRoom.Except(altercation.Combatants.Where(c => c is ConnectedPlayer).Cast<ConnectedPlayer>());
                var connectionIdsOfWitnessingPlayers = witnessPlayersOfAltercation.Select(wp => wp.SignalRConnectionId).ToList();

                foreach (var combatant in altercation.Combatants)
                {
                    if (combatant.IsReadyWithAttack() && !altercation.IsOver())
                    {
                        ICombatAction action = combatant.DequeueCombatAction();

                        if (combatant is ConnectedPlayer connectedPlayer)
                        {
                            var updateMessage = $"You used {action.Description} on {action.Target.Name}";
                            await hubContext.Clients.Client(connectedPlayer.SignalRConnectionId).SendAsync("ReceiveServerCreatedMessage", updateMessage);

                            if (action.Target is ConnectedPlayer targetedConnectedPlayer)
                            {
                                updateMessage = $"{combatant.Name} used {action.Description} on you.";
                                await hubContext.Clients.Client(targetedConnectedPlayer.SignalRConnectionId).SendAsync("ReceiveServerCreatedMessage", updateMessage);
                            }

                            if (witnessPlayersOfAltercation.Any())
                            {
                                updateMessage = $"{combatant.Name} used {action.Description} on {action.Target.Name}";
                                await hubContext.Clients.Clients(connectionIdsOfWitnessingPlayers).SendAsync("ReceiveServerCreatedMessage", updateMessage);
                            }
                        }

                        action.Act();

                        if (combatant.Target.IsDone())
                        {
                            altercation.End();
                        }
                    }
                }
            }

            Altercations.RemoveAll(a => a.IsOver());

            if (Altercations.Count == 0)
            {
                await StopAsync(new CancellationToken());
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            isRunning = true;

            _timer = new Timer(UpdateAltercations, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            isRunning = false;

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}