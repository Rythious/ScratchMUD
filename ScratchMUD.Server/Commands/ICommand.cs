using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal interface ICommand
    {
        string Name { get; }
        Task<List<(CommunicationChannel, string)>> ExecuteAsync(ConnectedPlayer connectedPlayer, params string[] parameters);
        string SyntaxHelp { get; }
        string GeneralHelp { get; }
    }
}