﻿using ScratchMUD.Server.EntityFramework;
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

            return new Models.Room
            {
                FullDescription = roomTranslation?.FullDescription,
                ShortDescription = roomTranslation?.ShortDescription,
                Id = roomId,
                AreaId = room.AreaId,
                Exits = BuildExitsHashSetFromRoomData(room),
                Author = authoringPlayerCharacter.Name,
                Title = roomTranslation?.Title
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
            Room currentRoom = ValidateDirectionOfNewRoom(roomId, Directions.North);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, Directions.South);

            currentRoom.NorthRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }

        public async Task CreateEastRoom(int roomId)
        {
            Room currentRoom = ValidateDirectionOfNewRoom(roomId, Directions.East);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, Directions.West);

            currentRoom.EastRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }

        public async Task CreateSouthRoom(int roomId)
        {
            Room currentRoom = ValidateDirectionOfNewRoom(roomId, Directions.South);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, Directions.North);

            currentRoom.SouthRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }

        public async Task CreateWestRoom(int roomId)
        {
            Room currentRoom = ValidateDirectionOfNewRoom(roomId, Directions.West);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, Directions.East);

            currentRoom.WestRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }

        public async Task CreateUpRoom(int roomId)
        {
            Room currentRoom = ValidateDirectionOfNewRoom(roomId, Directions.Up);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, Directions.Down);

            currentRoom.UpRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }

        public async Task CreateDownRoom(int roomId)
        {
            Room currentRoom = ValidateDirectionOfNewRoom(roomId, Directions.Down);

            Room newRoom = await CreateNewRoom(currentRoom.AreaId, currentRoom.VirtualNumber, Directions.Up);

            currentRoom.DownRoom = newRoom.VirtualNumber;

            await context.SaveChangesAsync();
        }

        private Room ValidateDirectionOfNewRoom(int roomId, Directions newRoomDirection)
        {
            var currentRoom = context.Room.First(r => r.RoomId == roomId);

            bool isDirectionAvailable = false;

            switch (newRoomDirection)
            {
                case Directions.North:
                    isDirectionAvailable = !currentRoom.NorthRoom.HasValue;
                    break;
                case Directions.East:
                    isDirectionAvailable = !currentRoom.EastRoom.HasValue;
                    break;
                case Directions.South:
                    isDirectionAvailable = !currentRoom.SouthRoom.HasValue;
                    break;
                case Directions.West:
                    isDirectionAvailable = !currentRoom.WestRoom.HasValue;
                    break;
                case Directions.Up:
                    isDirectionAvailable = !currentRoom.UpRoom.HasValue;
                    break;
                case Directions.Down:
                    isDirectionAvailable = !currentRoom.DownRoom.HasValue;
                    break;
                default:
                    break;
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
                case Directions.North:
                    newRoom.NorthRoom = roomNumberOfOrigin;
                    break;
                case Directions.East:
                    newRoom.EastRoom = roomNumberOfOrigin;
                    break;
                case Directions.South:
                    newRoom.SouthRoom = roomNumberOfOrigin;
                    break;
                case Directions.West:
                    newRoom.WestRoom = roomNumberOfOrigin;
                    break;
                case Directions.Up:
                    newRoom.UpRoom = roomNumberOfOrigin;
                    break;
                case Directions.Down:
                    newRoom.DownRoom = roomNumberOfOrigin;
                    break;
                default:
                    break;
            }

            context.Add(newRoom);

            await context.SaveChangesAsync();

            return newRoom;
        }

        public int GetRoomIdByAreaAndVirtualNumber(int areaId, int virtualNumber)
        {
            return context.Room.Single(r => r.AreaId == areaId && r.VirtualNumber == virtualNumber).RoomId;
        }
    }
}