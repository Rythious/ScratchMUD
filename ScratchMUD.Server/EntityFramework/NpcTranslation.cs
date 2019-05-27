using System;

namespace ScratchMUD.Server.EntityFramework
{
    public partial class NpcTranslation
    {
        public int NpcId { get; set; }
        public short LanguageId { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Language Language { get; set; }
        public virtual Npc Npc { get; set; }
    }
}