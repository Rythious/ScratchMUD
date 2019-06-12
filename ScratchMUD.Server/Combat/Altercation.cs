using System;
using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Combat
{
    public class Altercation
    {
        public IEnumerable<ICombatant> Combatants { get; set; }

        internal void End()
        {

        }

        internal bool IsOver()
        {
            return Combatants.Where(c => c.IsDone()).Any();
        }
    }
}
