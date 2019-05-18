﻿using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class NpcItem
    {
        public int NpcId { get; set; }
        public int ItemId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Item Item { get; set; }
        public virtual Npc Npc { get; set; }
    }
}