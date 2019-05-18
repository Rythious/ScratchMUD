using ScratchMUD.Server.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ScratchMUDContext context;

        public RoomRepository(ScratchMUDContext context)
        {
            this.context = context;
        }

        public string GetRoomFullDescription(int roomId)
        {
            return context.RoomTranslation.First(rt => rt.RoomId == roomId).FullDescription;
        }

        public async Task UpdateTitle(string title)
        {
            var room = context.RoomTranslation.First(rt => rt.RoomId == 1);
            room.Title = title;
            await context.SaveChangesAsync();
        }

        public async Task UpdateShortDescription(string shortDescription)
        {
            var room = context.RoomTranslation.First(rt => rt.RoomId == 1);
            room.ShortDescription = shortDescription;
            await context.SaveChangesAsync();
        }

        public async Task UpdateFullDescription(string fullDescription)
        {
            var room = context.RoomTranslation.First(rt => rt.RoomId == 1);
            room.FullDescription = fullDescription;
            await context.SaveChangesAsync();
        }
    }
}