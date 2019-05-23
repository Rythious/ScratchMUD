using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using ScratchMUD.Server.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class LookCommand : Command, ICommand
    {
        internal const string NAME = "look";
        private readonly IRoomRepository roomRepository;

        public LookCommand(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;

            Name = NAME;
            SyntaxHelp = "LOOK";
            GeneralHelp = "Looks at the details of the current room.";
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(PlayerContext playerContext, params string[] parameters)
        {
            var roomDetails = roomRepository.GetRoomWithTranslatedValues(playerContext.CurrentRoomId);

            var exitsOutputString = BuildExitsString(roomDetails.Exits);

            return Task.Run(() => new List<(CommunicationChannel, string)>
            {
                (CommunicationChannel.Self, roomDetails.Title),
                (CommunicationChannel.Self, roomDetails.FullDescription),
                (CommunicationChannel.Self, exitsOutputString)
            });
        }

        private string BuildExitsString(HashSet<(Directions, int)> exits)
        {
            var availableExits = exits.Select(e => e.Item1.ToString().ToLower()).ToList();

            if (availableExits.Count == 0)
            {
                availableExits.Add("none");
            }

            return $"[Exits: {string.Join(", ", availableExits)}]";
        }
    }
}