using ScratchMUD.Server.Models;
using System;
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
            return context.RoomTranslation.Single(rt => rt.RoomId == roomId).FullDescription;
        }

        public async Task UpdateTitle(int roomId, string title)
        {
            var room = context.RoomTranslation.Single(rt => rt.RoomId == roomId);
            room.Title = title;
            room.ModifiedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task UpdateShortDescription(int roomId, string shortDescription)
        {
            var room = context.RoomTranslation.Single(rt => rt.RoomId == roomId);
            room.ShortDescription = shortDescription;
            room.ModifiedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task UpdateFullDescription(int roomId, string fullDescription)
        {
            var room = context.RoomTranslation.Single(rt => rt.RoomId == roomId);
            room.FullDescription = fullDescription;
            room.ModifiedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task CreateNorthRoom(int roomId)
        {
            var currentRoom = context.Room.First(r => r.RoomId == roomId);

            if (currentRoom.NorthRoom.HasValue)
            {
                throw new ArgumentException("Current room already has a north exit defined.");
            }

            var highestVirtualNumberForARoomInThisArea = context.Room.Where(r => r.AreaId == currentRoom.AreaId).OrderByDescending(r => r.VirtualNumber).First().VirtualNumber;

            var newRoom = new Room
            {
                SouthRoom = currentRoom.VirtualNumber,
                VirtualNumber = ++highestVirtualNumberForARoomInThisArea,
                CreatedByPlayerId = 1,
                AreaId = currentRoom.AreaId,
                CreatedOn = DateTime.Now
            };

            context.Add(newRoom);

            await context.SaveChangesAsync();

            currentRoom.NorthRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }
    }
}