﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScratchMUD.Server.Hubs;
using ScratchMUD.Server.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server.HostedServices
{
    public class ServerTimeHostedService : IHostedService, IDisposable
    {
        private readonly IHubContext<EventHub> _hubContext;
        private Timer _timer;

        public ServerTimeHostedService(
            IHubContext<EventHub> hubContext,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _hubContext = hubContext;

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var areaCacheManager = scope.ServiceProvider.GetRequiredService<IAreaCacheManager>();
                areaCacheManager.LoadArea(1);
            }
        }

        public async void TrackMinutes(object state)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveServerCreatedMessage", "A calm breeze passes over you.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TrackMinutes, null, TimeSpan.FromMilliseconds(1000 - DateTime.Now.Millisecond), TimeSpan.FromMinutes(15));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}