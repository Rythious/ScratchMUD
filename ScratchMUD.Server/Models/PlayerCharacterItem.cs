using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class PlayerCharacterItem
    {
        public int PlayerCharacterId { get; set; }
        public int ItemId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Item Item { get; set; }
        public virtual PlayerCharacter PlayerCharacter { get; set; }
    }
}
