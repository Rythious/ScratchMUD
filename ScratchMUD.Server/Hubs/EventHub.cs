using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ScratchMUD.Server.Commands;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Hubs
{
    public class EventHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly PlayerContext playerContext;
        private readonly IDictionary<string,ICommand> commandDictionary;

        public EventHub(
            IConfiguration configuration,
            PlayerContext playerContext
        )
        {
            _configuration = configuration;
            this.playerContext = playerContext;

            commandDictionary = new Dictionary<string, ICommand>
            {
                [SayCommand.NAME] = new SayCommand(playerContext)
            };
        }

        //TODO: This will be replaced with a method that sends some sort of event object.
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveServerCreatedMessage", message);
        }

        public async Task RelayClientMessage(string message)
        {
            playerContext.Name = Context.ConnectionId;

            var command = SplitCommandFromParameters(message, out string[] parameters);

            string output;

            if (commandDictionary.ContainsKey(command))
            {
                output = await commandDictionary[command].ExecuteAsync(parameters);
            }
            else
            {
                output = $"'{command}' is not a valid command";
            }

            await Clients.All.SendAsync("ReceiveClientCreatedMessage", output);
        }

        private string SplitCommandFromParameters(string input, out string[] parameters)
        {
            var stringParts = input.Split(" ");

            if (stringParts.Length > 1)
            {
                parameters = new string[stringParts.Length - 1];

                Array.Copy(stringParts, 1, parameters, 0, stringParts.Length - 1);
            }
            else
            {
                parameters = new string[0];
            }

            return stringParts[0];
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