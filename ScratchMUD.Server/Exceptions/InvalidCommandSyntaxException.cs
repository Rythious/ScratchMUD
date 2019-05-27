using System;

namespace ScratchMUD.Server.Exceptions
{
    public class InvalidCommandSyntaxException : Exception
    {
        public InvalidCommandSyntaxException(string message) : base(message)
        {
        }
    }
}