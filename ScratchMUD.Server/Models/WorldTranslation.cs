using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class WorldTranslation
    {
        public short WorldId { get; set; }
        public short LanguageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Language Language { get; set; }
        public virtual World World { get; set; }
    }
}
