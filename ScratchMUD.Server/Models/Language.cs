using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class Language
    {
        public Language()
        {
            AreaTranslation = new HashSet<AreaTranslation>();
            ItemTranslation = new HashSet<ItemTranslation>();
            NpcTranslation = new HashSet<NpcTranslation>();
            RoomTranslation = new HashSet<RoomTranslation>();
            WorldTranslation = new HashSet<WorldTranslation>();
        }

        public short LanguageId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<AreaTranslation> AreaTranslation { get; set; }
        public virtual ICollection<ItemTranslation> ItemTranslation { get; set; }
        public virtual ICollection<NpcTranslation> NpcTranslation { get; set; }
        public virtual ICollection<RoomTranslation> RoomTranslation { get; set; }
        public virtual ICollection<WorldTranslation> WorldTranslation { get; set; }
    }
}
