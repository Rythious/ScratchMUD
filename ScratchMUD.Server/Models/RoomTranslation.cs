using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class RoomTranslation
    {
        public int RoomId { get; set; }
        public short LanguageId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Language Language { get; set; }
        public virtual Room Room { get; set; }
    }
}
