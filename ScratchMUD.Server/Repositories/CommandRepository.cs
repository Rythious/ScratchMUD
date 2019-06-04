using ScratchMUD.Server.Commands;
using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public class CommandRepository : ICommandRepository
    {
        private Dictionary<string, ICommand> CommandDictionary { get; }

        public CommandRepository(
            IRoomRepository roomRepository,
            EditingState editingState,
            IPlayerRepository playerRepository
        )
        {
            CommandDictionary = new Dictionary<string, ICommand>
            {
                [RoomEditCommand.NAME] = new RoomEditCommand(editingState, roomRepository),
                [SayCommand.NAME] = new SayCommand(),
                [LookCommand.NAME] = new LookCommand(roomRepository),
                [Directions.North.ToString().ToLower()] = new MoveCommand(Directions.North, roomRepository, playerRepository),
                [Directions.East.ToString().ToLower()] = new MoveCommand(Directions.East, roomRepository, playerRepository),
                [Directions.South.ToString().ToLower()] = new MoveCommand(Directions.South, roomRepository, playerRepository),
                [Directions.West.ToString().ToLower()] = new MoveCommand(Directions.West, roomRepository, playerRepository),
                [Directions.Up.ToString().ToLower()] = new MoveCommand(Directions.Up, roomRepository, playerRepository),
                [Directions.Down.ToString().ToLower()] = new MoveCommand(Directions.Down, roomRepository, playerRepository)
            };

            CommandDictionary[HelpCommand.NAME] = new HelpCommand(CommandDictionary);
        }

        public async Task<IEnumerable<(CommunicationChannel, string)>> ExecuteCommandAsync(ConnectedPlayer connectedPlayer, string command, params string[] parameters)
        {
            var output = new List<(CommunicationChannel, string)>();

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentNullException($"{nameof(command)} cannot be null");
            }

            command = command.ToLower();

            if (!CommandDictionary.ContainsKey(command))
            {
                throw new ArgumentException($"'{command}' is not a valid command");
            }

            output.AddRange(await CommandDictionary[command].ExecuteAsync(connectedPlayer, parameters));

            if (connectedPlayer.CommandQueueCount > 0)
            {
                command = connectedPlayer.DequeueCommand();
                parameters = new string[0];

                output.AddRange(await ExecuteCommandAsync(connectedPlayer, command, parameters));
            }

            return output;
        }
    }
}