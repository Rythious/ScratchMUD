using ScratchMUD.Server.Models;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    public interface ICommandRepository
    {
        Task<IEnumerable<(CommunicationChannel, string)>> ExecuteAsync(PlayerContext playerContext, string command, params string[] parameters);
    }
}