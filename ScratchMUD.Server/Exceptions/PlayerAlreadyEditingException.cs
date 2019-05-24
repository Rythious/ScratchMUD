using System;
using ScratchMUD.Server.Models.Constants;

namespace ScratchMUD.Server.Exceptions
{
    public class PlayerAlreadyEditingException : Exception
    {
        public PlayerAlreadyEditingException(string playerName, EditType editType) : base($"{playerName} is already editing a {editType}")
        {
        }
    }
}