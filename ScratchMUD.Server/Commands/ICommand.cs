using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal interface ICommand
    {
        Task<string> ExecuteAsync(params string[] parameters);
    }
}