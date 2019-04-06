using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        //TODO: This will be replaced with a method that sends some sort of event object.
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        //Throwaway method that is here to test connection between client and server.
        public async Task StartRandomMessages(int countOfMessages)
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