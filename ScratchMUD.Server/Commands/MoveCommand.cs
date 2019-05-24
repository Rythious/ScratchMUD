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
            GeneralHelp = $"If there is an exit to the {direction.ToString().ToLower()}, moves you to that room.";
            MaximumParameterCount = 0;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(ConnectedPlayer connectedPlayer, params string[] parameters)
        {
            ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(parameters);

            var output = new List<(CommunicationChannel, string)>();

            var room = roomRepository.GetRoomWithTranslatedValues(connectedPlayer.RoomId);

            if (!room.Exits.Select(e => e.Item1).Contains(Direction))
            {
                output.Add((CommunicationChannel.Self, $"There is no room to the {Direction.ToString().ToLower()}."));
            }
            else
            {
                var virtualNumberOfNewRoom = room.Exits.Where(e => e.Item1 == Direction).Select(e => e.Item2).Single();

                var newRoomId = roomRepository.GetRoomIdByAreaAndVirtualNumber(room.AreaId, virtualNumberOfNewRoom);

                playerRepository.UpdateRoomId(connectedPlayer.PlayerCharacterId, newRoomId).GetAwaiter().GetResult();
                connectedPlayer.PlayerCharacter = playerRepository.GetPlayerCharacter(connectedPlayer.PlayerCharacterId);

                connectedPlayer.QueueCommand(LookCommand.NAME);
            }

            return Task.Run(() => output);
        }
    }
}