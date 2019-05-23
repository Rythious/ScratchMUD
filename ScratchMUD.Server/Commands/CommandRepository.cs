using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public class CommandRepository : ICommandRepository
    {
        private Dictionary<string, ICommand> CommandDictionary { get; }

        public CommandRepository(
            IRoomRepository roomRepository,
            EditingState editingState
        )
        {
            CommandDictionary = new Dictionary<string, ICommand>
            {
                [RoomEditCommand.NAME] = new RoomEditCommand(editingState, roomRepository),
                [SayCommand.NAME] = new SayCommand(),
                [LookCommand.NAME] = new LookCommand(roomRepository),
                [Directions.North.ToString().ToLower()] = new MoveCommand(Directions.North),
                [Directions.East.ToString().ToLower()] = new MoveCommand(Directions.East),
                [Directions.South.ToString().ToLower()] = new MoveCommand(Directions.South),
                [Directions.West.ToString().ToLower()] = new MoveCommand(Directions.West),
                [Directions.Up.ToString().ToLower()] = new MoveCommand(Directions.Up),
                [Directions.Down.ToString().ToLower()] = new MoveCommand(Directions.Down)
            };

            CommandDictionary[HelpCommand.NAME] = new HelpCommand(CommandDictionary);
        }

        public async Task<IEnumerable<(CommunicationChannel, string)>> ExecuteAsync(PlayerContext playerContext, string command, params string[] parameters)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentNullException($"{nameof(command)} cannot be null");
            }

            if (!CommandDictionary.ContainsKey(command))
            {
                throw new ArgumentException($"'{command}' is not a valid command");
            }

            return await CommandDictionary[command].ExecuteAsync(playerContext, parameters);
        }
    }
}