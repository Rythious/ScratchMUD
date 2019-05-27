namespace ScratchMUD.Server.Models
{
    public struct PlayerContext
    {
        public int PlayerCharacterId { get; set; }
        public string Name { get; set; }
        public int CurrentRoomId { get; set; }
    }
}