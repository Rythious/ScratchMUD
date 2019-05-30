using System;

namespace ScratchMUD.Server.EntityFramework
{
    public partial class PlayerCharacterItem
    {
        public int PlayerCharacterItemId { get; set; }
        public int PlayerCharacterId { get; set; }
        public int ItemId { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Item Item { get; set; }
        public virtual PlayerCharacter PlayerCharacter { get; set; }
    }
}