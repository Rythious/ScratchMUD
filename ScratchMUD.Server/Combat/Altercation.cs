using System.Collections.Generic;
using System.Linq;

namespace ScratchMUD.Server.Combat
{
    public class Altercation
    {
        public IEnumerable<ICombatant> Combatants { get; set; }
        public int RoomId { get; }

        public Altercation(int roomId)
        {
            RoomId = roomId;
        }

        internal void End()
        {

        }

        internal bool IsOver()
        {
            return Combatants.Where(c => c.IsDone()).Any();
        }
    }
}