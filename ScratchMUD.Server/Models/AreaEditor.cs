using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class AreaEditor
    {
        public int AreaId { get; set; }
        public int PlayerCharacterId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual Area Area { get; set; }
        public virtual PlayerCharacter PlayerCharacter { get; set; }
    }
}
