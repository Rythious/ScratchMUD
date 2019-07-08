using ScratchMUD.Server.EntityFramework;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ScratchMUDContext context;

        public PlayerRepository(ScratchMUDContext context)
        {
            this.context = context;
        }

        public PlayerCharacter GetPlayerCharacter(int playerCharacterId)
        {
            return context.PlayerCharacter.Single(pc => pc.PlayerCharacterId == playerCharacterId);
        }

        public async Task UpdateRoomId(int playerCharacterId, int roomId)
        {
            var player = GetPlayerCharacter(playerCharacterId);
            player.RoomId = roomId;
            await context.SaveChangesAsync();
        }
    }
}