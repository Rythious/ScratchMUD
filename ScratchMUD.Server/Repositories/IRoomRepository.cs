using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public interface IRoomRepository
    {
        string GetRoomFullDescription(int roomId);
        Task UpdateTitle(string title);
        Task UpdateShortDescription(string shortDescription);
        Task UpdateFullDescription(string fullDescription);
    }
}