using ScratchMUD.Server.EntityFramework;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public interface IPlayerRepository
    {
        PlayerCharacter GetPlayerCharacter(int playerCharacterId);
        Task UpdateRoomId(int playerCharacterId, int roomId);
    }
}