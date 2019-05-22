using ScratchMUD.Server.Models;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public interface IRoomRepository
    {
        Room GetRoomWithTranslatedValues(int roomId);
        Task UpdateTitle(int roomId, string title);
        Task UpdateShortDescription(int roomId, string shortDescription);
        Task UpdateFullDescription(int roomId, string fullDescription);
        Task CreateNorthRoom(int roomId);
        Task CreateEastRoom(int roomId);
        Task CreateSouthRoom(int roomId);
        Task CreateWestRoom(int roomId);
        Task CreateUpRoom(int roomId);
        Task CreateDownRoom(int roomId);
    }
}