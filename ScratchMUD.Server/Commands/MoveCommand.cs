using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class MoveCommand : Command, ICommand
    {
        private Directions Direction { get; }
        private readonly IRoomRepository roomRepository;
        private readonly IPlayerRepository playerRepository;

        public MoveCommand(
            Directions direction,
            IRoomRepository roomRepository,
            IPlayerRepository playerRepository
        )
        {
            Direction = direction;
            this.roomRepository = roomRepository;
            this.playerRepository = playerRepository;

            Name = direction.ToString();
            SyntaxHelp = direction.ToString().ToUpper();
            GeneralHelp = $"If there is an exit in the direction of {direction.ToString().ToLower()}, moves you to that room.";
            MaximumParameterCount = 0;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(ConnectedPlayer connectedPlayer, IEnumerable<ConnectedPlayer> playersInTheRoom, params string[] parameters)
        {
            ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(parameters);

            var room = roomRepository.GetRoomWithTranslatedValues(connectedPlayer.RoomId);

            if (!room.Exits.Select(e => e.Item1).Contains(Direction))
            {
                connectedPlayer.QueueMessage($"There is no room to the {Direction.ToString().ToLower()}.");
            }
            else
            {
                var virtualNumberOfNewRoom = room.Exits.Where(e => e.Item1 == Direction).Select(e => e.Item2).Single();

                var newRoomId = roomRepository.GetRoomIdByAreaAndVirtualNumber(room.AreaId, virtualNumberOfNewRoom);

                playerRepository.UpdateRoomId(connectedPlayer.PlayerCharacterId, newRoomId).GetAwaiter().GetResult();
                connectedPlayer.PlayerCharacter = playerRepository.GetPlayerCharacter(connectedPlayer.PlayerCharacterId);

                connectedPlayer.QueueCommand(LookCommand.NAME);
            }

            return Task.Run(() => new List<(CommunicationChannel, string)>());
        }
    }
}