using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class Room
    {
        public Room()
        {
            PlayerCharacter = new HashSet<PlayerCharacter>();
            RoomItem = new HashSet<RoomItem>();
            RoomNpc = new HashSet<RoomNpc>();
            RoomTranslation = new HashSet<RoomTranslation>();
        }

        public int RoomId { get; set; }
        public int AreaId { get; set; }
        public int CreatedByPlayerId { get; set; }
        public short VirtualNumber { get; set; }
        public short? NorthRoom { get; set; }
        public short? EastRoom { get; set; }
        public short? SouthRoom { get; set; }
        public short? WestRoom { get; set; }
        public short? UpRoom { get; set; }
        public short? DownRoom { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Area Area { get; set; }
        public virtual PlayerCharacter CreatedByPlayer { get; set; }
        public virtual ICollection<PlayerCharacter> PlayerCharacter { get; set; }
        public virtual ICollection<RoomItem> RoomItem { get; set; }
        public virtual ICollection<RoomNpc> RoomNpc { get; set; }
        public virtual ICollection<RoomTranslation> RoomTranslation { get; set; }
    }
}
