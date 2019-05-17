using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class Npc
    {
        public Npc()
        {
            NpcItem = new HashSet<NpcItem>();
            NpcTranslation = new HashSet<NpcTranslation>();
            RoomNpc = new HashSet<RoomNpc>();
        }

        public int NpcId { get; set; }
        public int AreaId { get; set; }
        public int CreatedByPlayerId { get; set; }
        public short VirtualNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Area Area { get; set; }
        public virtual PlayerCharacter CreatedByPlayer { get; set; }
        public virtual ICollection<NpcItem> NpcItem { get; set; }
        public virtual ICollection<NpcTranslation> NpcTranslation { get; set; }
        public virtual ICollection<RoomNpc> RoomNpc { get; set; }
    }
}
