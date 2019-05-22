using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.EntityFramework
{
    public partial class Item
    {
        public Item()
        {
            ItemTranslation = new HashSet<ItemTranslation>();
            NpcItem = new HashSet<NpcItem>();
            PlayerCharacterItem = new HashSet<PlayerCharacterItem>();
            RoomItem = new HashSet<RoomItem>();
        }

        public int ItemId { get; set; }
        public int AreaId { get; set; }
        public int CreatedByPlayerId { get; set; }
        public short VirtualNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Area Area { get; set; }
        public virtual PlayerCharacter CreatedByPlayer { get; set; }
        public virtual ICollection<ItemTranslation> ItemTranslation { get; set; }
        public virtual ICollection<NpcItem> NpcItem { get; set; }
        public virtual ICollection<PlayerCharacterItem> PlayerCharacterItem { get; set; }
        public virtual ICollection<RoomItem> RoomItem { get; set; }
    }
}