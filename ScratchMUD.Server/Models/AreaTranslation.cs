using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class AreaTranslation
    {
        public int AreaId { get; set; }
        public short LanguageId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Area Area { get; set; }
        public virtual Language Language { get; set; }
    }
}
