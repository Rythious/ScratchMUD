using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;

namespace ScratchMUD.Server.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public string Title { get; set; }
        public string FullDescription { get; set; }
        public string Author { get; set; }
        public HashSet<(Directions, int)> Exits { get; set; }
        public string ShortDescription { get; set; }
    }
}