using ScratchMUD.Server.Infrastructure;
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
            SyntaxHelp = $"{NAME.ToUpper()}";
            GeneralHelp = "Looks at the details of the current room.";
            MaximumParameterCount = 0;
        }

        public Task<List<(CommunicationChannel, string)>> ExecuteAsync(RoomContext roomContext, params string[] parameters)
        {
            ThrowInvalidCommandSyntaxExceptionIfTooManyParameters(parameters);

            var roomDetails = roomRepository.GetRoomWithTranslatedValues(roomContext.CurrentCommandingPlayer.RoomId);

            var exitsOutputString = BuildExitsString(roomDetails.Exits);

            List<string> output = BuildOutputMessages(roomDetails, exitsOutputString);

            if (roomContext.NpcsInTheRoom != null && roomContext.NpcsInTheRoom.Any())
            {
                output.AddRange(roomContext.NpcsInTheRoom.Select(n => $"{n.ShortDescription} is here.").ToList());
            }

            foreach (var message in output)
            {
                roomContext.CurrentCommandingPlayer.QueueMessage(message);
            }

            return Task.Run(() => new List<(CommunicationChannel, string)>());
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

        private static List<string> BuildOutputMessages(Models.Room roomDetails, string exitsOutputString)
        {
            var output = new List<string>
            {
                roomDetails.Title,
                roomDetails.FullDescription,
                exitsOutputString
            };

            return output;
        }
    }
}