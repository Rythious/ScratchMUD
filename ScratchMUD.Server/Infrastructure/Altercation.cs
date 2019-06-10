using ScratchMUD.Server.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Infrastructure
{
    public class Altercation
    {
        public IEnumerable<ICombatant> Combatants { get; set; }

        internal void End()
        {
            throw new NotImplementedException();
        }

        internal bool IsOver()
        {
            return Combatants.Where(c => c.IsDone()).Any();
        }
    }
}
