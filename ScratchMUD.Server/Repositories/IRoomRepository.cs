using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public interface IRoomRepository
    {
        string GetRoomFullDescription(int roomId);
        Task UpdateTitle(int roomId, string title);
        Task UpdateShortDescription(int roomId, string shortDescription);
        Task UpdateFullDescription(int roomId, string fullDescription);
        Task CreateNorthRoom(int roomId);
    }
}