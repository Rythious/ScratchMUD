using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public interface IRoomRepository
    {
        Room GetRoomWithTranslatedValues(int roomId);
        Task UpdateTitle(int roomId, string title);
        Task UpdateShortDescription(int roomId, string shortDescription);
        Task UpdateFullDescription(int roomId, string fullDescription);
        Task CreateNewRoom(int originatingRoomId, Directions directionOfNewRoom);
        int GetRoomIdByAreaAndVirtualNumber(int areaId, int virtualNumber);
    }
}