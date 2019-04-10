using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using ScratchMUD.Server.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server
{
    public class ServerTimeHostedService : IHostedService, IDisposable
    {
        private readonly IHubContext<EventHub> _hubContext;
        private Timer _timer;

        public ServerTimeHostedService(IHubContext<EventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async void TrackMinutes(object state)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveServerCreatedMessage", DateTime.Now.ToString());
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TrackMinutes, null, TimeSpan.FromMilliseconds(1000 - DateTime.Now.Millisecond), TimeSpan.FromMinutes(1));

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