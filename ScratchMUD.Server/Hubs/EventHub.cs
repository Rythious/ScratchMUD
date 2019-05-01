using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly IConfiguration _configuration;

        public EventHub(
            IConfiguration configuration
        )
        {
            _configuration = configuration;
        }

        //TODO: This will be replaced with a method that sends some sort of event object.
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveServerCreatedMessage", message);
        }

        public async Task RelayClientMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveClientCreatedMessage", $"{Context.ConnectionId} says \"{message}\"");
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(() => SendMessage($"A new client has connected on {Context.ConnectionId}."));

            var connectionString = _configuration.GetValue<string>("ConnectionStrings:ScratchMudServer");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT TOP 1 FullDescription FROM ScratchMUD.dbo.RoomTranslation", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomMessage", $"{reader.GetString(0)}");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Clients.Client(Context.ConnectionId).SendAsync("ReceiveRoomMessage", $"{ex.ToString()}");
            }

            return base.OnConnectedAsync();
        }

        //Throwaway method that is here to test connection between client and server.
        public async Task StartTestMessages(int countOfMessages)
        {
            for (int i = 1; i <= countOfMessages; i++)
            {
                Thread.Sleep(1000);
                var message = $"Test message {i}";
                Console.WriteLine(message);
                await SendMessage(message);
            }
        }
    }
}