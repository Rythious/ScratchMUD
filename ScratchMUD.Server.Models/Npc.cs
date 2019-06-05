namespace ScratchMUD.Server.Models
{
    public class Npc
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
    }
}