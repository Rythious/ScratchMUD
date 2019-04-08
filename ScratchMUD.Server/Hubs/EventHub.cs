using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ScratchMUD.Server.DataObjects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly IConfiguration _configuration;

        public EventHub(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //TODO: This will be replaced with a method that sends some sort of event object.
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task RelayClientMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} says \"{message}\"");
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(() => SendMessage($"A new client has connected on {Context.ConnectionId}."));

            var room = new Room();
            _configuration.Bind("Room", room);

            Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", $"{room.Title}\n{room.Description}");

            return base.OnConnectedAsync();
        }

        //Throwaway method that is here to test connection between client and server.
        public async Task StartTestMessages(int countOfMessages)
        {
            for (int i = 1; i <= countOfMessages; i++)
            {
                Thread.Sleep(1500);
                var message = $"Test message {i}";
                Console.WriteLine(message);
                await SendMessage(message);
            }
        }
    }
}