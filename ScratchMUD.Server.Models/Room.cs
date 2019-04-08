using ScratchMUD.Server.Constants;
using System.Collections.Generic;

namespace ScratchMUD.Server.DataObjects
{
    public class Room
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public HashSet<(Directions, int)> Exits { get; set; }
    }
}