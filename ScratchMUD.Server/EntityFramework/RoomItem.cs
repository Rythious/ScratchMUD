using System;

namespace ScratchMUD.Server.EntityFramework
{
    public partial class RoomItem
    {
        public int RoomId { get; set; }
        public int ItemId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Item Item { get; set; }
        public virtual Room Room { get; set; }
    }
}