using ScratchMUD.Server.EntityFramework;
using ScratchMUD.Server.Models.Constants;
using System;
using System.Collections.Generic;
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

        public Models.Room GetRoomWithTranslatedValues(int roomId)
        {
            var room = context.Room.Single(r => r.RoomId == roomId);
            var roomTranslation = context.RoomTranslation.SingleOrDefault(rt => rt.LanguageId == 1 && rt.RoomId == roomId);
            var authoringPlayerCharacter = context.PlayerCharacter.Single(pc => pc.PlayerCharacterId == room.CreatedByPlayerId);
            var npcsInTheRoom = context.RoomNpc.Where(rn => rn.RoomId == roomId).Select(rn => rn.NpcId).ToList();
            var npcTranslationRecords = context.NpcTranslation.Where(nt => npcsInTheRoom.Distinct().Contains(nt.NpcId)).ToList();

            var npcModels = new List<Models.Npc>();

            foreach (var npcId in npcsInTheRoom)
            {
                npcModels.Add(new Models.Npc
                {
                    Id = npcId,
                    ShortDescription = npcTranslationRecords.Single(n => n.NpcId == npcId).ShortDescription,
                    FullDescription = npcTranslationRecords.Single(n => n.NpcId == npcId).FullDescription
                });
            }

            return new Models.Room
            {
                FullDescription = roomTranslation?.FullDescription,
                ShortDescription = roomTranslation?.ShortDescription,
                Id = roomId,
                AreaId = room.AreaId,
                Exits = BuildExitsHashSetFromRoomData(room),
                Author = authoringPlayerCharacter.Name,
                Title = roomTranslation?.Title,
                Npcs = npcModels
            };
        }

        private HashSet<(Directions, int)> BuildExitsHashSetFromRoomData(Room room)
        {
            var exits = new HashSet<(Directions, int)>();

            if (room.NorthRoom.HasValue)
            {
                exits.Add((Directions.North, room.NorthRoom.Value));
            }

            if (room.EastRoom.HasValue)
            {
                exits.Add((Directions.East, room.EastRoom.Value));
            }

            if (room.SouthRoom.HasValue)
            {
                exits.Add((Directions.South, room.SouthRoom.Value));
            }

            if (room.WestRoom.HasValue)
            {
                exits.Add((Directions.West, room.WestRoom.Value));
            }

            if (room.UpRoom.HasValue)
            {
                exits.Add((Directions.Up, room.UpRoom.Value));
            }

            if (room.DownRoom.HasValue)
            {
                exits.Add((Directions.Down, room.DownRoom.Value));
            }

            return exits;
        }

        public async Task UpdateTitle(int roomId, string title)
        {
            RoomTranslation roomTranslation = await GetRoomTranslationRecord(roomId, createIfMissing: true);
            roomTranslation.Title = title;
            roomTranslation.ModifiedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task UpdateShortDescription(int roomId, string shortDescription)
        {
            RoomTranslation roomTranslation = await GetRoomTranslationRecord(roomId, createIfMissing: true);
            roomTranslation.ShortDescription = shortDescription;
            roomTranslation.ModifiedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        public async Task UpdateFullDescription(int roomId, string fullDescription)
        {
            RoomTranslation roomTranslation = await GetRoomTranslationRecord(roomId, createIfMissing: true);
            roomTranslation.FullDescription = fullDescription;
            roomTranslation.ModifiedOn = DateTime.Now;
            await context.SaveChangesAsync();
        }

        private async Task<RoomTranslation> GetRoomTranslationRecord(int roomId, bool createIfMissing)
        {
            var roomTranslation = context.RoomTranslation.SingleOrDefault(rt => rt.RoomId == roomId);

            if (roomTranslation == null && createIfMissing)
            {
                roomTranslation = await CreateRoomTranslationRecord(roomId);
            }

            return roomTranslation;
        }

        private async Task<RoomTranslation> CreateRoomTranslationRecord(int roomId)
        {
            var newRoomTranslation = new RoomTranslation
            {
                FullDescription = "<FullDescription not set>",
                ShortDescription = "<ShortDescription not set>",
                Title = "<Title not set>",
                CreatedOn = DateTime.Now,
                RoomId = roomId,
                LanguageId = 1
            };

            context.Add(newRoomTranslation);
            await context.SaveChangesAsync();

            return newRoomTranslation;
        }

        public async Task CreateNewRoom(int originatingRoomId, Directions directionOfNewRoom)
        {
            Room currentRoom = ValidateDirectionOfNewRoom(originatingRoomId, directionOfNewRoom);

            var oppositeDirection = GetOppositeDirection(directionOfNewRoom);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, oppositeDirection);

            switch (directionOfNewRoom)
            {
                case Directions.North: currentRoom.NorthRoom = newRoom.VirtualNumber; break;
                case Directions.South: currentRoom.SouthRoom = newRoom.VirtualNumber; break;
                case Directions.East: currentRoom.EastRoom = newRoom.VirtualNumber; break;
                case Directions.West: currentRoom.WestRoom = newRoom.VirtualNumber; break;
                case Directions.Up: currentRoom.UpRoom = newRoom.VirtualNumber; break;
                case Directions.Down: currentRoom.DownRoom = newRoom.VirtualNumber; break;
            }

            await context.SaveChangesAsync();
        }

        private Directions GetOppositeDirection(Directions directionOfNewRoom)
        {
            switch (directionOfNewRoom)
            {
                case Directions.North: return Directions.South;
                case Directions.South: return Directions.North;
                case Directions.East: return Directions.West;
                case Directions.West: return Directions.East;
                case Directions.Up: return Directions.Down;
                case Directions.Down: return Directions.Up;
                default: throw new ArgumentException(nameof(directionOfNewRoom));
            }
        }

        private Room ValidateDirectionOfNewRoom(int roomId, Directions newRoomDirection)
        {
            var currentRoom = context.Room.First(r => r.RoomId == roomId);

            bool isDirectionAvailable = false;

            switch (newRoomDirection)
            {
                case Directions.North: isDirectionAvailable = !currentRoom.NorthRoom.HasValue; break;
                case Directions.South: isDirectionAvailable = !currentRoom.SouthRoom.HasValue; break;
                case Directions.East: isDirectionAvailable = !currentRoom.EastRoom.HasValue; break;
                case Directions.West: isDirectionAvailable = !currentRoom.WestRoom.HasValue; break;
                case Directions.Up: isDirectionAvailable = !currentRoom.UpRoom.HasValue; break;
                case Directions.Down: isDirectionAvailable = !currentRoom.DownRoom.HasValue; break;
            }

            if (!isDirectionAvailable)
            {
                throw new ArgumentException($"Current room already has a {newRoomDirection.ToString().ToLower()} exit defined.");
            }

            return currentRoom;
        }

        private async Task<Room> CreateNewRoom(int areaId, short roomNumberOfOrigin, Directions originDirection)
        {
            var highestVirtualNumberForARoomInThisArea = context.Room.Where(r => r.AreaId == areaId).OrderByDescending(r => r.VirtualNumber).First().VirtualNumber;

            var newRoom = new Room
            {
                VirtualNumber = ++highestVirtualNumberForARoomInThisArea,
                CreatedByPlayerId = 1,
                AreaId = areaId,
                CreatedOn = DateTime.Now
            };

            switch (originDirection)
            {
                case Directions.North: newRoom.NorthRoom = roomNumberOfOrigin; break;
                case Directions.South: newRoom.SouthRoom = roomNumberOfOrigin; break;
                case Directions.East: newRoom.EastRoom = roomNumberOfOrigin; break;
                case Directions.West: newRoom.WestRoom = roomNumberOfOrigin; break;
                case Directions.Up: newRoom.UpRoom = roomNumberOfOrigin; break;
                case Directions.Down: newRoom.DownRoom = roomNumberOfOrigin; break;
            }

            context.Add(newRoom);

            await context.SaveChangesAsync();

            await CreateRoomTranslationRecord(newRoom.RoomId);

            return newRoom;
        }

        public int GetRoomIdByAreaAndVirtualNumber(int areaId, int virtualNumber)
        {
            return context.Room.Single(r => r.AreaId == areaId && r.VirtualNumber == virtualNumber).RoomId;
        }
    }
}