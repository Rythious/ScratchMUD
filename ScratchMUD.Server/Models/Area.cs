using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class Area
    {
        public Area()
        {
            AreaEditor = new HashSet<AreaEditor>();
            AreaTranslation = new HashSet<AreaTranslation>();
            Item = new HashSet<Item>();
            Npc = new HashSet<Npc>();
            Room = new HashSet<Room>();
        }

        public int AreaId { get; set; }
        public short WorldId { get; set; }
        public short VirtualNumber { get; set; }
        public int CreatedByPlayerId { get; set; }
        public int OwnerPlayerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual PlayerCharacter CreatedByPlayer { get; set; }
        public virtual PlayerCharacter OwnerPlayer { get; set; }
        public virtual World World { get; set; }
        public virtual ICollection<AreaEditor> AreaEditor { get; set; }
        public virtual ICollection<AreaTranslation> AreaTranslation { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Npc> Npc { get; set; }
        public virtual ICollection<Room> Room { get; set; }
    }
}
