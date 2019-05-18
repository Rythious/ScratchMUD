using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class PlayerCharacter
    {
        public PlayerCharacter()
        {
            AreaCreatedByPlayer = new HashSet<Area>();
            AreaEditor = new HashSet<AreaEditor>();
            AreaOwnerPlayer = new HashSet<Area>();
            Item = new HashSet<Item>();
            Npc = new HashSet<Npc>();
            PlayerCharacterItem = new HashSet<PlayerCharacterItem>();
            Room = new HashSet<Room>();
        }

        public int PlayerCharacterId { get; set; }
        public short WorldId { get; set; }
        public string Name { get; set; }
        public short Level { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual World World { get; set; }
        public virtual ICollection<Area> AreaCreatedByPlayer { get; set; }
        public virtual ICollection<AreaEditor> AreaEditor { get; set; }
        public virtual ICollection<Area> AreaOwnerPlayer { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Npc> Npc { get; set; }
        public virtual ICollection<PlayerCharacterItem> PlayerCharacterItem { get; set; }
        public virtual ICollection<Room> Room { get; set; }
    }
}
