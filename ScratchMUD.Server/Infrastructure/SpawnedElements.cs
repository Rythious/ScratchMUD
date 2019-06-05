using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class SpawnedElements : ISpawnedElements
    {
        private Dictionary<int, AreaSpawnedNpcs> AreaSpawnedEnemiesCollection { get; set; }
    }
}