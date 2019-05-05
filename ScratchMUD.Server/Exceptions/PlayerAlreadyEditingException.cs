using System;
using ScratchMUD.Server.Models;

namespace ScratchMUD.Server.Exceptions
{
    public class PlayerAlreadyEditingException : Exception
    {
        public PlayerAlreadyEditingException(EditType editType) : base($"Player is already editing a {editType}")
        {
        }
    }
}
