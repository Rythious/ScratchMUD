using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class RoomNpc
    {
        public int RoomId { get; set; }
        public int NpcId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Npc Npc { get; set; }
        public virtual Room Room { get; set; }
    }
}
