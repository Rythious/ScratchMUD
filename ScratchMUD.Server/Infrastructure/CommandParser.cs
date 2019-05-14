using System;

namespace ScratchMUD.Server.Infrastructure
{
    public static class CommandParser
    {
        public static string SplitCommandFromParameters(string input, out string[] parameters)
        {
            var stringParts = input.Split(" ");

            if (stringParts.Length > 1)
            {
                parameters = new string[stringParts.Length - 1];

                Array.Copy(stringParts, 1, parameters, 0, stringParts.Length - 1);
            }
            else
            {
                parameters = new string[0];
            }

            return stringParts[0].ToLower();
        }
    }
}