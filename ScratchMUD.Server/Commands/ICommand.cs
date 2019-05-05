using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal interface ICommand
    {
        string Name { get; }
        Task<List<string>> ExecuteAsync(params string[] parameters);
        string SyntaxHelp();
        string GeneralHelp();
    }
}