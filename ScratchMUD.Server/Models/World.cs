using System;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public partial class World
    {
        public World()
        {
            Area = new HashSet<Area>();
            PlayerCharacter = new HashSet<PlayerCharacter>();
            WorldTranslation = new HashSet<WorldTranslation>();
        }

        public short WorldId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<Area> Area { get; set; }
        public virtual ICollection<PlayerCharacter> PlayerCharacter { get; set; }
        public virtual ICollection<WorldTranslation> WorldTranslation { get; set; }
    }
}
