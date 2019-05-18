using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class ItemTranslation
    {
        public int ItemId { get; set; }
        public short LanguageId { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Item Item { get; set; }
        public virtual Language Language { get; set; }
    }
}
