using System;

namespace ScratchMUD.Server.EntityFramework
{
    public partial class NpcItem
    {
        public int NpcItemId { get; set; }
        public int NpcId { get; set; }
        public int ItemId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Item Item { get; set; }
        public virtual Npc Npc { get; set; }
    }
}